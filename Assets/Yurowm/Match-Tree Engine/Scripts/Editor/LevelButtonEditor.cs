using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (LevelButton))]
public class LevelButtonEditor : Editor {

	LevelProfile profile;
	LevelButton button;

	Rect rect;
	string help = "";

	enum EditMode {Slot, Chip, PowerUp, Jelly, Block, Generator, Wall};
	EditMode currentMode = EditMode.Slot;

	static Color defaultColor;
	static Color transparentColor = new Color (1, 1, 1, 0.1f);
	static Color unpressedColor = new Color (0.7f, 0.7f, 0.7f, 1);
	static Color[] chipColor = {new Color(1,0.6f,0.6f,1),
		new Color(0.6f,1,0.6f,1),
		new Color(0.6f,0.8f,1,1),
		new Color(1,1,0.6f,1),
		new Color(1,0.6f,1,1),
		new Color(1,0.8f,0.6f,1)};
	
	static int slotOffect = 4;

	static GUIStyle mHelpStyle;
	static GUIStyle helpStyle {
		get {
			if (mHelpStyle == null) {
				mHelpStyle = new GUIStyle ();
				mHelpStyle.wordWrap = true;
				mHelpStyle.fontSize = 10;
				mHelpStyle.padding = new RectOffset (3, 3, 3, 3);
			}
			return mHelpStyle;
		}
	}

	static GUIStyle mSlotStyle;
	static GUIStyle slotStyle {
		get {
			if (mSlotStyle == null) {
				mSlotStyle = new GUIStyle (GUI.skin.button);
				mSlotStyle.wordWrap = true;
				mSlotStyle.fontSize = 8;
				mSlotStyle.padding = new RectOffset (3, 3, 3, 3);
			}
			return mSlotStyle;
		}
	}

	static string[] powerupLabel = {"XB", "B", "CB"};

	public override void OnInspectorGUI () {
		button = (LevelButton)target;
		profile = button.profile;

		if (profile.levelID == 0 || profile.levelID != target.GetInstanceID ()) {
			if (profile.levelID != target.GetInstanceID ())
				profile = profile.GetClone();
			profile.levelID = target.GetInstanceID ();
		}

		button.name = (button.transform.GetSiblingIndex () + 1).ToString();

		profile.width = Mathf.RoundToInt (EditorGUILayout.Slider ("Width", 1f * profile.width, 5f, 12f));
		profile.height = Mathf.RoundToInt (EditorGUILayout.Slider ("Height", 1f * profile.height, 5f, 12f));
		profile.chipCount = Mathf.RoundToInt (EditorGUILayout.Slider ("Chip Count", 1f * profile.chipCount, 3f, 6f));
		
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Score Stars", GUILayout.Width (150));
		profile.firstStarScore = Mathf.Max(EditorGUILayout.IntField (profile.firstStarScore, GUILayout.Width (90)), 1);
		profile.secondStarScore = Mathf.Max(EditorGUILayout.IntField (profile.secondStarScore, GUILayout.Width (90)), profile.firstStarScore+1);
		profile.thirdStarScore = Mathf.Max(EditorGUILayout.IntField (profile.thirdStarScore, GUILayout.ExpandWidth(true)), profile.secondStarScore+1);
		EditorGUILayout.EndHorizontal ();
		
		
		profile.target = (FieldTarget) EditorGUILayout.EnumPopup ("Target", profile.target);
		if(profile.target == FieldTarget.Timer)
			profile.duraction = Mathf.Max(0, EditorGUILayout.IntField("Session duration", profile.duraction));
		else
			profile.moveCount = Mathf.Clamp(EditorGUILayout.IntField("Move Count", profile.moveCount), 10, 50);
		
		EditorGUILayout.Space ();
		EditorGUILayout.BeginHorizontal (EditorStyles.toolbar, GUILayout.ExpandWidth(true));
		
		defaultColor = GUI.color;
		GUI.color = currentMode == EditMode.Slot ? unpressedColor : defaultColor;
		if (GUILayout.Button("Slot", EditorStyles.toolbarButton, GUILayout.Width(40)))
			currentMode = EditMode.Slot;
		GUI.color = currentMode == EditMode.Chip ? unpressedColor : defaultColor;
		if (GUILayout.Button("Chip", EditorStyles.toolbarButton, GUILayout.Width(40)))
			currentMode = EditMode.Chip;
		GUI.color = currentMode == EditMode.PowerUp ? unpressedColor : defaultColor;
		if (GUILayout.Button("PowerUp", EditorStyles.toolbarButton, GUILayout.Width(70)))
			currentMode = EditMode.PowerUp;
		GUI.color = currentMode == EditMode.Jelly ? unpressedColor : defaultColor;
		if (GUILayout.Button("Jelly", EditorStyles.toolbarButton, GUILayout.Width(50)))
			currentMode = EditMode.Jelly;
		GUI.color = currentMode == EditMode.Block ? unpressedColor : defaultColor;
		if (GUILayout.Button("Block", EditorStyles.toolbarButton, GUILayout.Width(50)))
			currentMode = EditMode.Block;
		GUI.color = currentMode == EditMode.Wall ? unpressedColor : defaultColor;
		if (GUILayout.Button("Wall", EditorStyles.toolbarButton, GUILayout.Width(40)))
			currentMode = EditMode.Wall;
		GUI.color = defaultColor;
		
		GUILayout.FlexibleSpace ();
		
		EditorGUILayout.EndVertical ();

		rect = GUILayoutUtility.GetRect (profile.width * (30 + slotOffect), profile.height * (30 + slotOffect));
		rect.x += slotOffect; 
		rect.y += slotOffect;
		
		switch (currentMode) {
		case EditMode.Slot: DrawSlot(); break;
		case EditMode.Chip: DrawChip(); break;
		case EditMode.PowerUp: DrawPowerUp(); break;
		case EditMode.Jelly: DrawJelly(); break;
		case EditMode.Block: DrawBlock(); break;
		case EditMode.Wall: DrawWall(); break;
		}
		
		EditorGUILayout.LabelField (help, helpStyle);
		
		EditorGUILayout.BeginHorizontal ();
		DrawModeTools ();
		EditorGUILayout.EndHorizontal ();

		
		button.profile = profile; 
		EditorUtility.SetDirty (button);
	}

	void DrawModeTools ()
	{
		switch (currentMode) {
		case EditMode.Slot:
			if (GUILayout.Button("Show all", GUILayout.Width(70))) 
				ShowAllSlots();		
			break;
		case EditMode.Chip:
			if (GUILayout.Button("Clear", GUILayout.Width(50))) 
				SetAllChips(-1);
			if (GUILayout.Button("Randomize", GUILayout.Width(90))) 
				SetAllChips(0);
			break;
		case EditMode.PowerUp:
			if (GUILayout.Button("Clear", GUILayout.Width(50))) 
				PowerUpClear();
			break;
		case EditMode.Jelly:
			if (GUILayout.Button("Clear", GUILayout.Width(50))) 
				JellyClear();
			break;
		case EditMode.Block:
			if (GUILayout.Button("Clear", GUILayout.Width(50))) 
				BlockClear();
			break;
		case EditMode.Wall:
			if (GUILayout.Button("Clear", GUILayout.Width(50))) 
				WallClear();
			break;	
		}
	}

	static bool DrawSlotButton (int x, int y, Rect r, LevelProfile lp)
	{
		defaultColor = GUI.color;
		Color color = defaultColor;
		string label = "";
		bool btn = false;
		if (!lp.GetSlot(x, y)) color *= transparentColor;
		else {
			if (lp.GetBlock(x, y) == 0 && lp.GetChip(x, y) > 0) {
				if (lp.GetChip(x, y) > lp.chipCount)
					lp.SetChip(x, y, lp.chipCount);
				color *= chipColor[lp.GetChip(x, y) - 1];
			}
			if (lp.GetBlock(x, y) == 0 && lp.GetChip(x, y) == -1 && lp.GetPowerup(x, y) == 0) {
				color *= unpressedColor;
			}
			if (lp.GetBlock(x, y) == 0 && lp.GetPowerup(x, y) > 0) {
				label += (label.Length == 0 ? "" : "\n");
				label += powerupLabel[lp.GetPowerup(x, y) - 1];
			}
			if (lp.GetBlock(x, y) > 0)
				label += (label.Length == 0 ? "" : "\n") + "B:" + lp.GetBlock(x, y).ToString();
			if (lp.GetJelly(x, y) > 0)
				label += (label.Length == 0 ? "" : "\n") + "J:" + lp.GetJelly(x, y).ToString();
		}
		GUI.color = color;
		btn = GUI.Button(new Rect(r.xMin + x * (30 + slotOffect), r.yMin + y * (30 + slotOffect), 30, 30), label, slotStyle);
		GUI.color = defaultColor;
		return btn;
	}

	static bool DrawWallButton (int x, int y, string t, Rect r, LevelProfile lp)
	{
		bool btn = false;
		if (t == "H") btn = lp.GetWallH(x,y);
		if (t == "V") btn = lp.GetWallV(x,y);
		
		defaultColor = GUI.color;
		Color color = defaultColor;
		
		if (btn)
			color *= Color.black;
		GUI.color = color;
		
		if (t == "V") btn = GUI.Button(new Rect(r.xMin + (x + 1) * (30 + slotOffect) - 5 - slotOffect / 2,
		                                        r.yMin + y * (30 + slotOffect) - 10 + 15, 10, 20), "", slotStyle);
		if (t == "H") btn = GUI.Button(new Rect(r.xMin + x * (30 + slotOffect) - 10 + 15,
		                                        r.yMin + (y + 1) * (30 + slotOffect) - 5 - slotOffect / 2, 20, 10), "", slotStyle);
		GUI.color = defaultColor;
		return btn;
	}

	public static void DrawWallPreview (Rect r, LevelProfile lp) {
		int x;
		int y;
		GUI.enabled = false;
		for (x = 0; x < lp.width-1; x++)
			for (y = 0; y < lp.height; y++)
				if (lp.GetWallV(x,y) && lp.GetSlot(x,y) && lp.GetSlot(x+1,y))
					DrawWallButton(x, y, "V", r, lp);
		for (x = 0; x < lp.width; x++)
			for (y = 0; y < lp.height-1; y++)
				if (lp.GetWallH(x,y) && lp.GetSlot(x,y) && lp.GetSlot(x,y+1))
					DrawWallButton(x, y, "H", r, lp);
		GUI.enabled = true;
	}
	
	public static void DrawSlotPreview (Rect r, LevelProfile lp) {
		int x;
		int y;
		GUI.enabled = false;
		for (x = 0; x < lp.width; x++)
			for (y = 0; y < lp.height; y++)
				if (lp.GetSlot(x, y))
					DrawSlotButton(x, y, r, lp);
		GUI.enabled = true;
	}

	void DrawSlot () {
		help = "Transparent slot - slot which is absent";
		for (int x = 0; x < profile.width; x++)
			for (int y = 0; y < profile.height; y++)
				if (DrawSlotButton(x, y, rect, profile))
					profile.SetSlot(x, y, !profile.GetSlot(x,y));
		DrawWallPreview (rect, profile);
	}
	
	void DrawPowerUp () {
		help = "XB - Cross bomb." +
			"\nB - Simple bomb." +
				"\nCB - Color bomb." +
				"\n Slot with block can not contain any chip";
		for (int x = 0; x < profile.width; x++)
			for (int y = 0; y < profile.height; y++)
			if (DrawSlotButton(x, y, rect, profile)) {
				profile.SetPowerup(x, y, profile.GetPowerup(x, y) + 1);
				if (profile.GetPowerup(x, y) > 3)
					profile.SetPowerup(x, y, 0);
			}
		DrawWallPreview (rect, profile);
	}
	
	void DrawJelly () {
		help = "J:X, where X - level of jelly.";
		for (int x = 0; x < profile.width; x++)
			for (int y = 0; y < profile.height; y++)
			if (DrawSlotButton(x, y, rect, profile)) {
				profile.SetJelly(x,y, profile.GetJelly(x, y) + 1);
				if (profile.GetJelly(x,y) > 3)
					profile.SetJelly(x, y, 0);
			}
		DrawWallPreview (rect, profile);
	}
	
	void DrawBlock() {
		help = "B:X, where X - level of block.";
		for (int x = 0; x < profile.width; x++)
			for (int y = 0; y < profile.height; y++)
			if (DrawSlotButton(x, y, rect, profile)) {
				profile.SetBlock(x, y, profile.GetBlock(x, y) + 1);
				if (profile.GetBlock(x, y) > 3)
					profile.SetBlock(x, y, 0);
			}
		DrawWallPreview (rect, profile);
	}
	
	void DrawChip () {
		help = "You specify a group of colors. The final color will be determined by chance during the level generating in accordance with these groups." +
			"\nColored slots - chip with the random color of the corresponding group." +
				"\nWhite slot - chip with random color." +
				"\nGrey slot - slot without chip.";
		for (int x = 0; x < profile.width; x++)
			for (int y = 0; y < profile.height; y++)
			if (DrawSlotButton(x, y, rect, profile)) {
				profile.SetChip(x, y, profile.GetChip(x, y) + 1);
				if (profile.GetChip(x, y) > profile.chipCount)
				    profile.SetChip(x, y, -1);
			}
		DrawWallPreview (rect, profile);
	}
	
	void DrawWall () {
		help = "White wall - wall which is absent";
		int x;
		int y;
		DrawSlotPreview (rect, profile);
		for (x = 0; x < profile.width-1; x++)
			for (y = 0; y < profile.height; y++)
				if (profile.GetSlot(x,y) && profile.GetSlot(x+1,y))
					if (DrawWallButton(x, y, "V", rect, profile))
						profile.SetWallV(x, y, !profile.GetWallV(x, y));
		for (x = 0; x < profile.width; x++)
			for (y = 0; y < profile.height-1; y++)
				if (profile.GetSlot(x,y) && profile.GetSlot(x,y+1))
					if (DrawWallButton(x, y, "H", rect, profile))
						profile.SetWallH(x, y, !profile.GetWallH(x, y));
	}

	void ShowAllSlots () {
		for (int x = 0; x < 12; x++)
			for (int y = 0; y < 12; y++)
				profile.SetSlot(x, y, true);
	}
	
	void SetAllChips (int c) {
		for (int x = 0; x < 12; x++)
			for (int y = 0; y < 12; y++)
				profile.SetChip(x, y, c);
	}
	
	void PowerUpClear ()
	{
		for (int x = 0; x < 12; x++)
			for (int y = 0; y < 12; y++)
				profile.SetPowerup(x, y, 0);
	}
	
	void JellyClear ()
	{
		for (int x = 0; x < 12; x++)
			for (int y = 0; y < 12; y++)
				profile.SetJelly(x, y, 0);
	}
	
	void BlockClear ()
	{
		for (int x = 0; x < 12; x++)
			for (int y = 0; y < 12; y++)
				profile.SetBlock(x, y, 0);
	}
	
	void WallClear ()
	{
		for (int x = 0; x < 12; x++)
		for (int y = 0; y < 12; y++) {
			profile.SetWallH (x, y, false);
			profile.SetWallV (x, y, false);
		}
	}

//	void OnInspectorUpdate () {
//		Repaint ();
//	}
}
