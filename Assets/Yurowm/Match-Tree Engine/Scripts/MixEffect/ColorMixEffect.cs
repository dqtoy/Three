using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorMixEffect : BombMixEffect {

	public Powerup targetBomb;

	void Start() {
		StartCoroutine (MixEffect ());
	}
	
	IEnumerator MixEffect (){
		yield return 0;

		Chip chip = GetChip ();
		while (chip.parentSlot == null) yield return 0;
		transform.position = chip.parentSlot.transform.position;

		while (!SessionAssistant.main.CanIWait()) yield return 0;
		
		SessionAssistant.main.matching ++;
		
		AudioAssistant.Shot("ColorBombCrush");

		int sx = chip.parentSlot.slot.x;
		int sy = chip.parentSlot.slot.y;
		
		SimpleChip[] allChips = GameObject.FindObjectsOfType<SimpleChip> ();
		List<SimpleChip>[] sorted = new List<SimpleChip>[6];
		int[] count = new int[6];
		
		foreach (SimpleChip c in allChips) {
			if (c.chip.destroing) continue;
			count[c.chip.id]++;
			if (sorted[c.chip.id] == null) sorted[c.chip.id] = new List<SimpleChip>();
			sorted[c.chip.id].Add(c);
		}
		
		List<Chip> target = new List<Chip>();
		
		int i;
		for (i = 0; i < 6; i++)
			if (sorted[i] != null && sorted[i].Count > 0)
				target.Add(sorted[i][Random.Range(0, sorted[i].Count)].chip);
		int x;
		int y;
		for (i = 0; i < target.Count; i++) {
			x = target[i].parentSlot.slot.x;
			y = target[i].parentSlot.slot.y;
			target[i] = FieldAssistant.main.AddPowerup(x, y, targetBomb);
			Lightning.CreateLightning(3, transform, target[i].transform, Chip.colors[i]);
			yield return new WaitForSeconds(0.1f);
			
		}
		yield return new WaitForSeconds(0.3f);
		
		SessionAssistant.main.eventCount ++;
		
		for (i = 0; i < target.Count; i++) {
			if (target[i])
				target[i].DestroyChip();
			yield return new WaitForSeconds(0.2f);
		}
		yield return new WaitForSeconds(0.4f);
		
		SessionAssistant.main.matching --;

		FieldAssistant.main.BlockCrush(sx, sy, false);
		FieldAssistant.main.JellyCrush(sx, sy);
		
		destroingLock = false;
		DestroyChipFunction ();
	}
}
