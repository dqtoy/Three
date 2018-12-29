using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Soomla.Store;

public class BoosterFreeBombs : MonoBehaviour {

	public static bool busy = false;

	public void BoosterActivate () {
		if (busy) return;
		if (GameObject.FindObjectsOfType<SimpleChip>().Length == 0) return;
		StartCoroutine (CreatingBombs ());
	}

	IEnumerator CreatingBombs () {
		busy = true;
		StoreInventory.TakeItem ("freebombs", 1);
		yield return StartCoroutine (Utils.WaitFor (SessionAssistant.main.CanIWait, 0.1f));
		SessionAssistant.main.MatchingCounter ();
		FieldAssistant.main.AddPowerup(Powerup.SimpleBomb);
		yield return new WaitForSeconds (0.1f);
		FieldAssistant.main.AddPowerup(Powerup.SimpleBomb);
		yield return new WaitForSeconds (0.1f);
		FieldAssistant.main.AddPowerup(Powerup.SimpleBomb);
		yield return new WaitForSeconds (0.1f);
		FieldAssistant.main.AddPowerup(Powerup.CrossBomb);
		yield return new WaitForSeconds (0.1f);
		FieldAssistant.main.AddPowerup(Powerup.CrossBomb);
		yield return new WaitForSeconds (0.1f);
		FieldAssistant.main.AddPowerup(Powerup.ColorBomb);
		busy = false;
	}
}
