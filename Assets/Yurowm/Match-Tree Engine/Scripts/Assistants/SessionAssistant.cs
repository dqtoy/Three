using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;

public class SessionAssistant : MonoBehaviour {

	public static SessionAssistant main;
	List<Solution> solutions = new List<Solution>();

	[HideInInspector]
	public int animate = 0;
	[HideInInspector]
	public int matching = 0;
	[HideInInspector]
	public int gravity = 0;
	[HideInInspector]
	public int lastMovementId;
	[HideInInspector]
	public int movesCount;
	[HideInInspector]
	public float timeLeft;
	[HideInInspector]
	public int eventCount;
	[HideInInspector]
	public int score = 0;
	[HideInInspector]
	public bool isPlaying = false;
	[HideInInspector]
	public string levelMessage = "";
	
	bool wait = false;
	public static int scoreC = 10;
	
	void  Awake (){
		main = this;
	}

	void Start () {
		AudioAssistant.main.PlayMusic ("Menu");
	}
	
	public static void  Reset (){
		main.animate = 0;
		main.gravity = 0;
		main.matching = 0;
		
		main.eventCount = 0;
		main.score = 0;
		
		main.isPlaying  = true;
		main.movesCount = LevelProfile.main.moveCount;
	}

	public void AddExtraMoves ()
	{
		if (!isPlaying) return;
		if (StoreInventory.GetItemBalance("move") == 0) return;
		StoreInventory.TakeItem ("move", 1);
		movesCount += 5;
		UIServer.main.ShowPage ("Field");
		wait = false;
	}

	public void AddExtraTime ()
	{
		if (!isPlaying) return;
		if (StoreInventory.GetItemBalance("time") == 0) return;
		StoreInventory.TakeItem ("time", 1);
		timeLeft += 15;
		UIServer.main.ShowPage ("Field");
		wait = false;
	}

	public void Continue ()
	{
		UIServer.main.ShowPage ("Field");
		wait = false;
	}

	public void PlayNextLevel() {
		if (!LevelButton.all.ContainsKey(LevelProfile.main.level + 1)) return;
		LevelButton.all[LevelProfile.main.level + 1].LoadLevel ();
	}

	public void RestartLevel() {
		LevelButton.all[LevelProfile.main.level].LoadLevel ();
	}

	void  Update (){
		DebugPanel.Log ("Animate", "Session", animate);
		DebugPanel.Log ("Gravity", "Session", gravity);
		DebugPanel.Log ("Matching", "Session", matching);
	}

	public void StartSession(FieldTarget sessionType) {
		StopAllCoroutines ();
		AudioAssistant.main.PlayMusic ("Field");
		timeLeft = LevelProfile.main.duraction;
		isPlaying = true;
		score = 0;
		switch (sessionType) {
		case FieldTarget.Score: StartCoroutine(ScoreSession()); break;
		case FieldTarget.Timer: StartCoroutine(TimerSession()); break;
		case FieldTarget.Jelly: StartCoroutine(JellySession());break;
		}
		StartCoroutine (ShowingHintRoutine());
		StartCoroutine (ShuffleRoutine());
		StartCoroutine (FindingSolutionsRoutine());
	}
	
	IEnumerator ScoreSession() {
		while (movesCount > 0 && score < LevelProfile.main.thirdStarScore) {
			yield return StartCoroutine(Utils.WaitFor(CanIWait, 1f));
			if (movesCount == 0 && score < LevelProfile.main.firstStarScore) {
				UIServer.main.ShowPage("NoMoreMoves");
				wait = true;
				while (wait) yield return new WaitForSeconds(0.5f);
			}
		}

		yield return StartCoroutine(BurnLastMovesToPowerups());
		
		if (score < LevelProfile.main.firstStarScore) {
			yield return StartCoroutine(FieldCamera.main.HideFieldRoutine());
			FieldAssistant.main.RemoveField();
//			levelMessage = ""
			ShowLosePopup();
			yield break;
		}

		yield return StartCoroutine(Utils.WaitFor(CanIWait, 1f));
		
		yield return StartCoroutine(FieldCamera.main.HideFieldRoutine());
		ShowWinPopup ();
		FieldAssistant.main.RemoveField();
	}

	IEnumerator TimerSession() {
		while (timeLeft > 0) {
			timeLeft -= 0.1f;
			yield return new WaitForSeconds(0.1f);
			if (timeLeft <= 0 && score < LevelProfile.main.firstStarScore) {
				yield return StartCoroutine(Utils.WaitFor(CanIWait, 1f));
				if (score < LevelProfile.main.firstStarScore) {
					UIServer.main.ShowPage("NoMoreTime");
					wait = true;
					while (wait) yield return new WaitForSeconds(0.5f);
				}
			}
		}
		
		if (score < LevelProfile.main.firstStarScore) {
			yield return StartCoroutine(FieldCamera.main.HideFieldRoutine());
			ShowLosePopup();
			FieldAssistant.main.RemoveField();
			yield break;
		}

		yield return StartCoroutine(Utils.WaitFor(CanIWait, 1f));
		
		yield return StartCoroutine(CollapseAllPowerups ());
		
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 1f));
		
		yield return StartCoroutine(FieldCamera.main.HideFieldRoutine());
		ShowWinPopup ();
		FieldAssistant.main.RemoveField();
	}


	IEnumerator JellySession() {
		while (movesCount > 0 && GameObject.FindObjectsOfType<Jelly>().Length > 0) {
			yield return StartCoroutine(Utils.WaitFor(CanIWait, 1f));
			if (movesCount == 0 && GameObject.FindObjectsOfType<Jelly>().Length > 0) {
				UIServer.main.ShowPage("NoMoreMoves");
				wait = true;
				while (wait) yield return new WaitForSeconds(0.5f);
			}
		}
		
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 1f));

		if (GameObject.FindObjectsOfType<Jelly>().Length > 0) {
			yield return StartCoroutine(FieldCamera.main.HideFieldRoutine());
			ShowLosePopup();
			FieldAssistant.main.RemoveField();
			yield break;
		}
		
		yield return StartCoroutine(BurnLastMovesToPowerups());

		yield return StartCoroutine(Utils.WaitFor(CanIWait, 1f));
		
		yield return StartCoroutine(FieldCamera.main.HideFieldRoutine());
		ShowWinPopup ();
		FieldAssistant.main.RemoveField();
	}

	IEnumerator FindingSolutionsRoutine () {
		List<Solution> solutions;
		Solution bestSolution;

		while (true) {
			if (isPlaying) {
				solutions = FindSolutions();
				if (solutions.Count > 0) {
					bestSolution = solutions[0];
					foreach(Solution solution in solutions)
						if (solution.potencial > bestSolution.potencial)
							bestSolution = solution;
					MatchSolution(bestSolution);
				} else 
					yield return StartCoroutine(Utils.WaitFor(CanIMatch, 0.1f));
			} else
				yield return 0;
		}
	}

	IEnumerator BurnLastMovesToPowerups ()
	{
		yield return StartCoroutine(CollapseAllPowerups ());
		int count = 0;
		while (movesCount > 0) {
			count = Mathf.Min(movesCount, 6);
			while (count > 0) {
				count --;
				movesCount --;
				switch (Random.Range(0, 2)) {
				case 0: FieldAssistant.main.AddPowerup(Powerup.SimpleBomb); break;
				case 1: FieldAssistant.main.AddPowerup(Powerup.CrossBomb); break;
				}
				yield return new WaitForSeconds(0.1f);
			}
			yield return new WaitForSeconds(0.5f);
			yield return StartCoroutine(CollapseAllPowerups ());
		}
	}

	public void Quit() {
		StopAllCoroutines ();
		StartCoroutine(QuitCoroutine());
		}

	IEnumerator QuitCoroutine() {
		isPlaying = false;
		UIServer.main.HideAll ();
		yield return StartCoroutine(FieldCamera.main.HideFieldRoutine());
		FieldAssistant.main.RemoveField();
		UIServer.main.ShowPage ("LevelList");
		AudioAssistant.main.PlayMusic ("Menu");
	}

	IEnumerator CollapseAllPowerups () {
		while(!CanIWait()) yield return 0;
		List<Chip> powerUp = FindPowerups ();
		while (powerUp.Count > 0) {
			SessionAssistant.main.MatchingCounter();
			powerUp[Random.Range(0, powerUp.Count - 1)].DestroyChip();
			while(!CanIWait()) yield return 0;
			yield return new WaitForSeconds(0.1f);
			powerUp = FindPowerups ();
		}
		yield return StartCoroutine(Utils.WaitFor(CanIWait, 0.1f));
	}

	List<Chip> FindPowerups ()
	{
		List<Chip> pu = new List<Chip>();

		foreach (ColorBomb bomb in GameObject.FindObjectsOfType<ColorBomb> ())
			pu.Add(bomb.gameObject.GetComponent<Chip>());
		foreach (CrossBomb bomb in GameObject.FindObjectsOfType<CrossBomb> ())
			pu.Add(bomb.gameObject.GetComponent<Chip>());
		foreach (SimpleBomb bomb in GameObject.FindObjectsOfType<SimpleBomb> ())
			pu.Add(bomb.gameObject.GetComponent<Chip>());

		return pu;
	}

	void ShowLosePopup ()
	{
		AudioAssistant.Shot ("YouLose");
		AudioAssistant.main.PlayMusic ("Menu");
		isPlaying = false;
		FindObjectOfType<FieldCamera>().HideField();
		UIServer.main.ShowPage ("YouLose");
	}

	void ShowWinPopup ()
	{
		AudioAssistant.Shot ("YouWin");
		AudioAssistant.main.PlayMusic ("Menu");
		isPlaying = false;
		string key = "Best_Score_" + LevelProfile.main.level.ToString ();
		if (PlayerPrefs.GetInt(key) < score)
			PlayerPrefs.SetInt (key, score);
		PlayerPrefs.SetInt ("Complete_" + LevelProfile.main.level.ToString (), 1);
		FindObjectOfType<FieldCamera>().HideField();
		UIServer.main.ShowPage ("YouWin");
	}

	public bool CanIAnimate (){
		return gravity == 0 && matching == 0;
	}
	
	public bool CanIMatch (){
		return animate == 0 && gravity == 0;
	}
	
	public bool CanIGravity (){
		return (animate == 0 && matching == 0) || gravity > 0;
	}
	
	public bool CanIWait (){
		return animate == 0 && matching == 0 && gravity == 0;
	}

	void  AddSolution ( Solution s  ){
		solutions.Add(s);
	}
	
	public void  MatchingCounter (){
		eventCount ++;
	}
	
	List<Move> FindMoves (){
		List<Move> moves = new List<Move>();
		if (!FieldAssistant.main.gameObject.activeSelf) return moves;
		if (LevelProfile.main == null) return moves;

		int x;
		int y;
		int width = LevelProfile.main.width;
		int height = LevelProfile.main.height;
		Move move;
		Solution solution;
		SlotForChip slot;
		string chipTypeA = "";
		string chipTypeB = "";
		
		// horizontal
		for (x = 0; x < width - 1; x++)
		for (y = 0; y < height; y++) {
			if (!FieldAssistant.main.field.slots[x,y]) continue;
			if (!FieldAssistant.main.field.slots[x+1,y]) continue;
			if (FieldAssistant.main.field.blocks[x,y] > 0) continue;
			if (FieldAssistant.main.field.blocks[x+1,y] > 0) continue;
			if (FieldAssistant.main.field.chips[x,y] == FieldAssistant.main.field.chips[x+1,y]) continue;
			if (FieldAssistant.main.field.wallsV[x,y]) continue;
			move = new Move();
			move.fromX = x;
			move.fromY = y;
			move.toX = x + 1;
			move.toY = y;
			AnalizSwap(move);
			slot = FieldAssistant.main.GetSlot(move.fromX, move.fromY).GetComponent<SlotForChip>();
			chipTypeA = slot.chip == null ? "SimpleChip" : slot.chip.chipType;
			solution = slot.MatchAnaliz();
			if (solution != null) {
				move.potencial += solution.potencial;
				move.solution = solution;
			}
			slot = FieldAssistant.main.GetSlot(move.toX, move.toY).GetComponent<SlotForChip>();
			solution = slot.MatchAnaliz();
			chipTypeB = slot.chip == null ? "SimpleChip" : slot.chip.chipType;
			if (solution != null && (move.potencial < solution.potencial || move.solution == null)) move.solution = solution;
			if (solution != null)
				move.potencial += solution.potencial;
			AnalizSwap(move);
			if (move.potencial != 0 || (chipTypeA != "SimpleChip" &&  chipTypeB != "SimpleChip")) 
				moves.Add(move);		
		}
		
		// vertical
		for (x = 0; x < width; x++)
		for (y = 0; y < height - 1; y++) {
			if (!FieldAssistant.main.field.slots[x,y]) continue;
			if (!FieldAssistant.main.field.slots[x,y+1]) continue;
			if (FieldAssistant.main.field.blocks[x,y] > 0) continue;
			if (FieldAssistant.main.field.blocks[x,y+1] > 0) continue;
			if (FieldAssistant.main.field.chips[x,y] == FieldAssistant.main.field.chips[x,y+1]) continue;
			if (FieldAssistant.main.field.wallsH[x,y]) continue;
			move = new Move();
			move.fromX = x;
			move.fromY = y;
			move.toX = x;
			move.toY = y + 1;

			AnalizSwap(move);
			slot = FieldAssistant.main.GetSlot(move.fromX, move.fromY).GetComponent<SlotForChip>();
			chipTypeA = slot.chip == null ? "SimpleChip" : slot.chip.chipType;
			solution = slot.MatchAnaliz();
			if (solution != null) {
				move.potencial += solution.potencial;
				move.solution = solution;
			}
			slot = FieldAssistant.main.GetSlot(move.toX, move.toY).GetComponent<SlotForChip>();
			solution = slot.MatchAnaliz();
			chipTypeB = slot.chip == null ? "SimpleChip" : slot.chip.chipType;
			if (solution != null && (move.potencial < solution.potencial || move.solution == null)) move.solution = solution;
			if (solution != null)
				move.potencial += solution.potencial;
			AnalizSwap(move);
			if (move.potencial != 0 || (chipTypeA != "SimpleChip" &&  chipTypeB != "SimpleChip")) 
				moves.Add(move);		
		}

		return moves;
	}
	
	void  AnalizSwap ( Move move  ){
		SlotForChip slot;
		Chip fChip = GameObject.Find("Slot_" + move.fromX + "x" + move.fromY).GetComponent<Slot>().GetChip();
		Chip tChip = GameObject.Find("Slot_" + move.toX + "x" + move.toY).GetComponent<Slot>().GetChip();
		if (!fChip || !tChip) return;
		slot = tChip.parentSlot;
		fChip.parentSlot.SetChip(tChip);
		slot.SetChip(fChip);
	}
	
	void  MatchSolution ( Solution solution  ){

		//	solution = FieldAssistant.main.GetSlot(solution.x, solution.y).GetComponent<SlotForChip>().MatchAnaliz();
		//	if (!solution) return;
		MatchingCounter ();
		
		int sx = solution.x;
		int sy = solution.y;
		
		int x;
		int y;
		
		Chip chip = ((GameObject)GameObject.Find("Slot_" + sx + "x" + sy)).GetComponent<SlotForChip>().GetChip();
		
		int width = FieldAssistant.main.field.width;
		int height = FieldAssistant.main.field.height;
		
		Slot s;
		GameObject j;
		GameObject o;
		Chip c;
		
		int puX = -1;
		int puY = -1;
		int puID = -1;
		
		if (!chip.IsMatcheble()) return;
		
		if (solution.h)
		for (x = sx + 1; x < width; x++) {
			o = GameObject.Find("Slot_" + x + "x" + sy);
			if (!o) break;
			s = o.GetComponent<Slot>();
			if (!s) continue;
			if (!s.GetChip()) break;
			c = s.GetChip();
			if (c.id == chip.id) {
				if (!c.IsMatcheble()) break;
				if (c.movementID > puID) {
					puID = c.movementID;
					puX = x;
					puY = sy;
				}
				s.GetChip().SetScore(Mathf.Pow(2, solution.count-3)/solution.count);
				FieldAssistant.main.BlockCrush(x, sy, true);
				s.GetChip().DestroyChip();
				j = GameObject.Find("Jelly_" + x + "x" + sy);
				if (j) j.SendMessage("JellyCrush", SendMessageOptions.DontRequireReceiver);				
			}
			else break;
		}
		
		if (solution.h)
		for (x = sx - 1; x >= 0; x--) {
			o = GameObject.Find("Slot_" + x + "x" + sy);
			if (!o) break;
			s = o.GetComponent<Slot>();
			if (!s) continue;
			if (!s.GetChip()) break;
			c = s.GetChip();
			if (c.id == chip.id) {
				if (!c.IsMatcheble()) break;
				if (c.movementID > puID) {
					puID = c.movementID;
					puX = x;
					puY = sy;
				}
				s.GetChip().SetScore(Mathf.Pow(2, solution.count-3)/solution.count);
				FieldAssistant.main.BlockCrush(x, sy, true);
				s.GetChip().DestroyChip();
				j = GameObject.Find("Jelly_" + x + "x" + sy);
				if (j) j.SendMessage("JellyCrush", SendMessageOptions.DontRequireReceiver);	
			}
			else break;
		}
		
		if (solution.v)
		for (y = sy + 1; y < height; y++) {
			o = GameObject.Find("Slot_" + sx + "x" + y);
			if (!o) break;
			s = o.GetComponent<Slot>();
			if (!s) continue;
			if (!s.GetChip()) break;
			c = s.GetChip();
			if (c.id == chip.id) {
				if (!c.IsMatcheble()) break;
				if (c.movementID > puID) {
					puID = c.movementID;
					puX = sx;
					puY = y;
				}
				s.GetChip().SetScore(Mathf.Pow(2, solution.count-3)/solution.count);
				FieldAssistant.main.BlockCrush(sx, y, true);
				s.GetChip().DestroyChip();
				j = GameObject.Find("Jelly_" + sx + "x" + y);
				if (j) j.SendMessage("JellyCrush", SendMessageOptions.DontRequireReceiver);	
			}
			else break;
		}
		
		if (solution.v)
		for (y = sy - 1; y >= 0; y--) {
			o = GameObject.Find("Slot_" + sx + "x" + y);
			if (!o) break;
			s = o.GetComponent<Slot>();
			if (!s) continue;
			if (!s.GetChip()) break;
			c = s.GetChip();
			if (c.id == chip.id) {
				if (!c.IsMatcheble()) break;
				if (c.movementID > puID) {
					puID = c.movementID;
					puX = sx;
					puY = y;
				}
				s.GetChip().SetScore(Mathf.Pow(2, solution.count-3)/solution.count);
				FieldAssistant.main.BlockCrush(sx, y, true);
				s.GetChip().DestroyChip();
				j = GameObject.Find("Jelly_" + sx + "x" + y);
				if (j) j.SendMessage("JellyCrush", SendMessageOptions.DontRequireReceiver);	
			}
			else break;
		}
		
		if (chip.movementID > puID) {
			puID = chip.movementID;
			puX = sx;
			puY = sy;
		}
		chip.SetScore(Mathf.Pow(2, solution.count-3)/solution.count);
		FieldAssistant.main.BlockCrush(sx, sy, true);
		chip.DestroyChip();
		j = GameObject.Find("Jelly_" + solution.x + "x" + solution.y);
		if (j) j.SendMessage("JellyCrush", SendMessageOptions.DontRequireReceiver);	

		// I5 analiz
		if (solution.count >= 5) {
			if ((solution.v && !solution.h) || (!solution.v && solution.h)) {
				FieldAssistant.main.GetNewColorBomb(puX, puY, FieldAssistant.main.GetSlot(puX, puY).transform.position, solution.id);
				return;
			}
		}
		
		// I4 analiz
		if (solution.count >= 4) {
			if ((solution.v && !solution.h) || (!solution.v && solution.h)) {
				FieldAssistant.main.GetNewBomb(puX, puY, FieldAssistant.main.GetSlot(puX, puY).transform.position, solution.id);
				return;
			}
		}
		
		// T4 analiz
		if (solution.count >= 4) {
			if (solution.v && solution.h) {
				FieldAssistant.main.GetNewCrossBomb(puX, puY, FieldAssistant.main.GetSlot(puX, puY).transform.position, solution.id);
				return;
			}
		}
		
	}
	
	public int GetMovementID (){
		lastMovementId ++;
		return lastMovementId;
	}
	
	public int GetMovesCount (){
		return movesCount;
	}

	IEnumerator ShuffleRoutine ()
	{
		int shuffleOrder = 0;
		float delay = 1;
		while (true) {
			yield return StartCoroutine(Utils.WaitFor(CanIWait, delay));
			if (eventCount > shuffleOrder) {
				shuffleOrder = eventCount;
				yield return StartCoroutine(Shuffle(false));
			}
		}
	}

	public IEnumerator Shuffle (bool f)
	{
		bool force = f;

		List<Move> moves = FindMoves();
		if (moves.Count > 0 && !force)
			yield break;

		isPlaying = false;

		Slot[] slots = GameObject.FindObjectsOfType<Slot> ();
		Dictionary<Slot, Vector3> positions = new Dictionary<Slot, Vector3> ();
		foreach (Slot slot in slots)
			positions.Add (slot, slot.transform.position);

		float t = 0;
		while (t < 1) {
			t += Time.deltaTime * 3;
			for (int i = 0; i < slots.Length; i++) {
				slots[i].transform.position = Vector3.Lerp(positions[slots[i]], Vector3.zero, t);
			}
			yield return 0;
		}


		List<Solution> solutions = FindSolutions ();


		while (f || moves.Count == 0 || solutions.Count > 0) {
			f = false;
			MatchingCounter();
			SlotForChip[] sc = GameObject.FindObjectsOfType<SlotForChip> ();
			for (int j = 0; j < sc.Length; j++)
				AnimationAssistant.main.SwapTwoItemNow(sc[j].chip, sc[Random.Range(0, j-1)].chip);
			yield return 0;
			moves = FindMoves();
			solutions = FindSolutions ();
			yield return 0;
		}


		t = 0;
		while (t < 1) {
			t += Time.deltaTime * 3;
			for (int i = 0; i < slots.Length; i++) {
				slots[i].transform.position = Vector3.Lerp(Vector3.zero, positions[slots[i]], t);
			}
			yield return 0;
		}
		
		isPlaying = true;

	}

	List<Solution> FindSolutions() {
		List<Solution> solutions = new List<Solution> ();
		Solution zsolution;
		foreach(SlotForChip slot in GameObject.FindObjectsOfType<SlotForChip>()) {
			zsolution = slot.MatchAnaliz();
			if (zsolution != null) solutions.Add(zsolution);
		}
		return solutions;
	}

	IEnumerator ShowingHintRoutine () {
		int hintOrder = 0;
		float delay = 5;
		while (true) {
			yield return StartCoroutine(Utils.WaitFor(CanIWait, delay));
			if (eventCount > hintOrder) {
				hintOrder = eventCount;
				ShowHint();
			}
		}
	}
	
	void  ShowHint (){
		List<Move> moves = FindMoves();
		if (moves.Count == 0) {
			return;
		}

		Move bestMove = moves[ Random.Range(0, moves.Count) ];
//		
		int x;
		int y;
		
		if (bestMove.solution.h)
			for (x = bestMove.solution.x - bestMove.solution.negH; x <= bestMove.solution.x + bestMove.solution.posH; x++)
				if (x != bestMove.solution.x)
					GameObject.Find("Slot_" + x + "x" + bestMove.solution.y).GetComponent<Slot>().GetChip().Flashing(eventCount);
//		
		if (bestMove.solution.v)	
			for (y = bestMove.solution.y - bestMove.solution.negV; y <= bestMove.solution.y + bestMove.solution.posV; y++)
				if (y != bestMove.solution.y)
					GameObject.Find("Slot_" + bestMove.solution.x + "x" + y).GetComponent<Slot>().GetChip().Flashing(eventCount);
//		
		if (bestMove.fromX != bestMove.solution.x || bestMove.fromY != bestMove.solution.y)
			GameObject.Find("Slot_" + bestMove.fromX + "x" + bestMove.fromY).GetComponent<Slot>().GetChip().Flashing(eventCount);
		
		if (bestMove.toX != bestMove.solution.x || bestMove.toY != bestMove.solution.y)
			GameObject.Find("Slot_" + bestMove.toX + "x" + bestMove.toY).GetComponent<Slot>().GetChip().Flashing(eventCount);
	}
	
	public class Solution {
		public int count;
		public int potencial;
		public int id;
		public int x;
		public int y;
		public bool v;
		public bool h;
		public int posV;
		public int negV;
		public int posH;
		public int negH;
	}
	
	public class Move {
		public int fromX;
		public int fromY;
		public int toX;
		public int toY;
		public Solution solution;
		public int potencial;
	}
}