using UnityEngine;
using System.Collections;

public class AnimationAssistant : MonoBehaviour {
	
	
	public static AnimationAssistant main;
	bool swaping = false;
	public bool forceSwap = false;

	float swapDuration = 0.2f;
	
	void  Awake (){
		main = this;
	}

	void Start() {
		BombMixEffect.Initialize ();
	}

	public void SwapTwoItemNow (Chip a, Chip b) {
		if (!a || !b) return;
		if (a == b) return;

		Vector3 posA = a.parentSlot.transform.position;
		Vector3 posB = b.parentSlot.transform.position;
		
		a.transform.position = posB;
		b.transform.position = posA;
		
		a.movementID = SessionAssistant.main.GetMovementID();
		b.movementID = SessionAssistant.main.GetMovementID();
		
		SlotForChip slotA = a.parentSlot;
		SlotForChip slotB = b.parentSlot;
		
		slotB.SetChip(a);
		slotA.SetChip(b);
	}

	public void SwapTwoItem (Chip a, Chip b) {
		if (!SessionAssistant.main.isPlaying) return;
		StartCoroutine (SwapTwoItemRoutine(a, b));
	}

	IEnumerator SwapTwoItemRoutine (Chip a, Chip b){
		if (swaping) yield break;
		if (!a || !b) yield break;
		if (!SessionAssistant.main.CanIAnimate()) yield break;
		switch (LevelProfile.main.target) {
			case FieldTarget.Jelly:
			case FieldTarget.Score:
				if (SessionAssistant.main.movesCount <= 0) yield break; break;
			case FieldTarget.Timer:
				if (SessionAssistant.main.timeLeft <= 0) yield break; break;
		}

		bool mix = false;
		if (BombMixEffect.ContainsPair(a.chipType, b.chipType)) mix = true;

		int move = 0;
		
		SessionAssistant.main.animate++;
		swaping = true;
		
		Vector3 posA = a.parentSlot.transform.position;
		Vector3 posB = b.parentSlot.transform.position;
		
		float progress = 0;
		
		while (progress < swapDuration) {
			a.transform.position = Vector3.Lerp(posA, posB, progress/swapDuration);
			if (!mix) b.transform.position = Vector3.Lerp(posB, posA, progress/swapDuration);
			
			progress += Time.deltaTime;
			
			yield return 0;
		}
		
		a.transform.position = posB;
		if (!mix) b.transform.position = posA;
		
		a.movementID = SessionAssistant.main.GetMovementID();
		b.movementID = SessionAssistant.main.GetMovementID();
		
		if (mix) {
			swaping = false;
			BombPair pair = new BombPair(a.chipType, b.chipType);
			SlotForChip slot = b.parentSlot;
			a.HideChip();
			b.HideChip();
			BombMixEffect.Mix(pair, slot);
			SessionAssistant.main.movesCount--;
			SessionAssistant.main.animate--;
			yield break;
		}

		SlotForChip slotA = a.parentSlot;
		SlotForChip slotB = b.parentSlot;
		
		slotB.SetChip(a);
		slotA.SetChip(b);
		
		
		move++;
		
		int count = 0; 
		SessionAssistant.Solution solution;
		
		solution = slotA.MatchAnaliz();
		if (solution != null) count += solution.count;
		
		solution = slotB.MatchAnaliz();
		if (solution != null) count += solution.count;
		
		
		if (count == 0 && !forceSwap) {
			AudioAssistant.Shot("SwapFailed");
			while (progress > 0) {
				a.transform.position = Vector3.Lerp(posA, posB, progress/swapDuration);
				b.transform.position = Vector3.Lerp(posB, posA, progress/swapDuration);
				
				progress -= Time.deltaTime;
				
				yield return 0;
			}
			
			a.transform.position = posA;
			b.transform.position = posB;
			
			a.movementID = SessionAssistant.main.GetMovementID();
			b.movementID = SessionAssistant.main.GetMovementID();
			
			slotB.SetChip(b);
			slotA.SetChip(a);
			
			move--;
		} else
			AudioAssistant.Shot("SwapSuccess");
		
		SessionAssistant.main.movesCount -= move;
		SessionAssistant.main.MatchingCounter ();
		
		SessionAssistant.main.animate--;
		swaping = false;
	}
	
	public void  Explode ( Vector3 center ,   float radius ,   float force  ){
		Chip[] chips = GameObject.FindObjectsOfType<Chip>();
		Vector3 impuls;
		foreach(Chip chip in chips) {
			if ((chip.transform.position - center).magnitude > radius) continue;
			impuls = (chip.transform.position - center) * force;
			impuls *= Mathf.Pow((radius - (chip.transform.position - center).magnitude) / radius, 2);
			chip.impuls += impuls;
		}
	}
	
	public IEnumerator  FadeOut ( Chip c ,   float delay  ){
		Transform t = c.transform;
		float progress = delay;
		Vector3 start = t.localScale;
		while (progress > 0) {
			t.localScale = Vector3.Lerp(Vector3.zero, start, progress/delay);
			progress -= Time.deltaTime;
			yield return 0;
		}
		Destroy(c.gameObject);
	}
}