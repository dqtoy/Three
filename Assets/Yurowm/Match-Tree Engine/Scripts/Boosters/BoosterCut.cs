﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Soomla.Store;

public class BoosterCut : MonoBehaviour {
	
	void OnEnable () {
		TurnController (false);
		StartCoroutine (Cut());
	}

	void OnDisable () {
		TurnController (true);
	}

	void TurnController(bool b) {
		if (ControlAssistant.main == null) return;
		ControlAssistant.main.enabled = b;
	}

	IEnumerator Cut ()
	{
		yield return StartCoroutine (Utils.WaitFor (SessionAssistant.main.CanIWait, 0.1f));

		Chip targetChip = null;
		while (true) {
			if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
				targetChip = ControlAssistant.main.GetChipFromTouch();
			if (targetChip != null) {
				Slot slot = targetChip.parentSlot.slot;
				FieldAssistant.main.BlockCrush(slot.x, slot.y, true);
				FieldAssistant.main.JellyCrush(slot.x, slot.y);
				StoreInventory.TakeItem ("cut", 1);

				SessionAssistant.main.MatchingCounter();
				SessionAssistant.main.animate ++;

				float t = 1;
				float velocity = 0;
				Vector3 impuls = new Vector3(Random.Range(-3f, 3f), Random.Range(1f, 5f), 0);
				SpriteRenderer sprite = targetChip.gameObject.GetComponent<SpriteRenderer>();
				sprite.sortingLayerName = "Foreground";
				while (t > 0) {
					t -= Time.deltaTime;
					velocity += Time.deltaTime * 20;
					velocity = Mathf.Min(velocity, 40);
					targetChip.transform.position += impuls * Time.deltaTime;
					targetChip.transform.position -= Vector3.up * Time.deltaTime * velocity;
					yield return 0;
				}
				SessionAssistant.main.animate --;

				Destroy(targetChip.gameObject);
				break;
			}
			yield return 0;
		}

		UIServer.main.ShowPage ("Field");
	}
}
