using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Soomla.Store;

public class BoosterSwitch : MonoBehaviour {

	
	void OnEnable () {
		StartCoroutine (Switch());
	}

	void OnDisable () {
		AnimationAssistant.main.forceSwap = false;
	}

	IEnumerator Switch ()
	{
		yield return StartCoroutine (Utils.WaitFor (SessionAssistant.main.CanIWait, 0.1f));
		int currentEventCount = SessionAssistant.main.eventCount;
		int currentMovesCount = SessionAssistant.main.movesCount;

		AnimationAssistant.main.forceSwap = true;

		while (currentEventCount == SessionAssistant.main.eventCount || currentMovesCount == SessionAssistant.main.movesCount)
			yield return 0;
		
		AnimationAssistant.main.forceSwap = false;
		SessionAssistant.main.movesCount ++;
		
		StoreInventory.TakeItem ("switch", 1);

		UIServer.main.ShowPage ("Field");
	}
}
