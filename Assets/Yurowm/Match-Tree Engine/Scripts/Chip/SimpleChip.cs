using UnityEngine;
using System.Collections;

public class SimpleChip : MonoBehaviour {
	
	
	public Chip chip;
	bool mMatching = false;
	public bool matching {
		set {
			if (value == mMatching) return;
			mMatching = value;
			if (mMatching)
				SessionAssistant.main.matching ++;
			else
				SessionAssistant.main.matching --;
		}
		
		get {
			return mMatching;
		}
	}
	void OnDestroy () {
		matching = false;
	}

	void  Awake (){
		chip = GetComponent<Chip>();
		chip.chipType = "SimpleChip";
	}
	
	IEnumerator  DestroyChipFunction (){
		
		matching = true;
		GetComponent<Animation>().Play("Minimizing");
		AudioAssistant.Shot("ChipCrush");
		
		yield return new WaitForSeconds(0.1f);
		matching = false;
		
		chip.ParentRemove();
		
		while (GetComponent<Animation>().isPlaying) yield return 0;
		Destroy(gameObject);
	}
}