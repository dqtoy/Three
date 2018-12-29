using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Soomla.Store;

public class BoosterFinger : MonoBehaviour {
	
	void OnEnable () {
		TurnController (false);
		StartCoroutine (Finger());
	}

	void OnDisable () {
		TurnController (true);
	}

	void TurnController(bool b) {
		if (ControlAssistant.main == null) return;
		ControlAssistant.main.enabled = b;
	}

	IEnumerator Finger ()
	{
		yield return StartCoroutine (Utils.WaitFor (SessionAssistant.main.CanIWait, 0.1f));

		Chip targetChip = null;
		while (true) {
			if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
				targetChip = ControlAssistant.main.GetChipFromTouch();
			if (targetChip != null) {
				if (targetChip.gameObject.GetComponent<SimpleChip>() != null) {
					Slot slot = targetChip.parentSlot.slot;
					FieldAssistant.main.AddPowerup(slot.x, slot.y, Powerup.SimpleBomb);
					SessionAssistant.main.MatchingCounter();
					break;
				}
			}
			yield return 0;
		}

		StoreInventory.TakeItem ("finger", 1);
		UIServer.main.ShowPage ("Field");
	}
}
