using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Soomla.Store;

public class BoosterShake : MonoBehaviour {
	
	public static bool busy = false;
	
	public void BoosterActivate () {
		if (busy) return;
		StartCoroutine (CreatingBombs ());
	}
	
	IEnumerator CreatingBombs () {
		busy = true;
		yield return StartCoroutine (Utils.WaitFor(SessionAssistant.main.CanIWait, 0.1f));
		yield return StartCoroutine (SessionAssistant.main.Shuffle (true));
		StoreInventory.TakeItem ("shake", 1);
		busy = false;
	}
}
