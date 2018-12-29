using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Button))]
public class LevelButton : MonoBehaviour {

	public static Dictionary<int, LevelButton> all = new Dictionary<int, LevelButton> ();

	private Text label;
	[SerializeField]
	public LevelProfile profile;
	
	static bool selected = false;

	public static void PlayLevel(int i) {
		if (!all.ContainsKey(i)) return;
		all [i].OnClick ();
	}

	void Awake() {
		Button btn = GetComponent<Button> ();
		profile.level = GetNumber ();
		all.Add (profile.level, this);
		if (btn != null)
			btn.onClick.AddListener(() => OnClick());
		label = GetComponentInChildren<Text> ();
		gameObject.name = GetNumber ().ToString ();
		label.text = gameObject.name;
	}

	public void OnClick () {
		if (selected) return;
		selected = true;
		LoadLevel ();
	}

	public void LoadLevel () {
		LevelProfile.main = profile;
		Field field = new Field (profile.width, profile.height);
		field.chipCount = profile.chipCount;
		FieldAssistant.main.CreateField (field);
		SessionAssistant.main.StartSession (profile.target);
		FindObjectOfType<FieldCamera>().ShowField();
		UIServer.main.ShowPage ("Field");
	}

	void OnDisable() {
		selected = false;
	}

	void OnEnable() {
		selected = false;
	}

	public int GetNumber ()
	{
		return transform.GetSiblingIndex () + 1;
	}
}

[System.Serializable]
public class LevelProfile {
	
	public static LevelProfile main;
	const int maxSize = 12;

	public int levelID = 0;
	public int level = 0;
	public int width = 9;
	public int height = 9;
	public int chipCount = 6;
	public int moveCount = 30;
	public int firstStarScore = 100;
	public int secondStarScore = 200;
	public int thirdStarScore = 300;
	
	public FieldTarget target = FieldTarget.Score;
	// Target score in score mode = firstStarScore;
	// Count of jellies in jelly mode colculate automaticly via jellyData array;
	// Session duration in timer mode = duration value (sec);
	public int duraction = 100;

	public LevelProfile() {
		for (int x = 0; x < maxSize; x++) {
			for (int y = 0; y < maxSize; y++) {
//				slotK += "1";
				SetSlot(x, y, true);
			}
		}
	}
	
	// Slot
	public bool[] slot = new bool[maxSize * maxSize];
	public bool GetSlot(int x, int y) {
		return slot [y * maxSize + x];
	}
	public void SetSlot(int x, int y, bool v) {
		slot[y * maxSize + x] = v;
	}
	
	// Chip
	public int[] chip = new int[maxSize * maxSize];
	public int GetChip(int x, int y) {
		return chip [y * maxSize + x];
	}
	public void SetChip(int x, int y, int v) {
		chip[y * maxSize + x] = v;
	}
	
	// Jelly
	public int[] jelly = new int[maxSize * maxSize];
	public int GetJelly(int x, int y) {
		return jelly [y * maxSize + x];
	}
	public void SetJelly(int x, int y, int v) {
		jelly[y * maxSize + x] = v;
	}
	
	// Block
	public int[] block = new int[maxSize * maxSize];
	public int GetBlock(int x, int y) {
		return block [y * maxSize + x];
	}
	public void SetBlock(int x, int y, int v) {
		block[y * maxSize + x] = v;
	}
	
	// Powerup
	public int[] powerup = new int[maxSize * maxSize];
	public int GetPowerup(int x, int y) {
		return powerup [y * maxSize + x];
	}
	public void SetPowerup(int x, int y, int v) {
		powerup[y * maxSize + x] = v;
	}
	
	// Wall
	public bool[] wallV = new bool[maxSize * maxSize];
	public bool[] wallH = new bool[maxSize * maxSize];
	public bool GetWallV(int x, int y) {
		return wallV [y * maxSize + x];
	}
	public bool GetWallH(int x, int y) {
		return wallH [y * maxSize + x];
	}
	public void SetWallV(int x, int y, bool v) {
		wallV [y * maxSize + x] = v;
	}
	public void SetWallH(int x, int y, bool v) {
		wallH [y * maxSize + x] = v;
	}

	public LevelProfile GetClone() {
		LevelProfile clone = new LevelProfile ();
		clone.level = level;

		clone.width = width;
		clone.height = height;
		clone.chipCount = chipCount;

		clone.firstStarScore = firstStarScore;
		clone.secondStarScore = secondStarScore;
		clone.thirdStarScore = thirdStarScore;

		clone.target = target;

		clone.duraction = duraction;
		clone.moveCount = moveCount;

		clone.slot = slot;
		clone.chip = chip;
		clone.jelly = jelly;
		clone.block = block;
		clone.powerup = powerup;
		clone.wallV = wallV;
		clone.wallH = wallH;

		return clone;
	}
}