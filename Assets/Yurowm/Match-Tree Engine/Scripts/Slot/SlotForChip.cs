using UnityEngine;
using System.Collections;

public class SlotForChip : MonoBehaviour {
	
	
	public Chip chip;
	public Slot slot;

	public Slot this[Side index] {
		get {
			return slot.nearSlot[index];
		}
	}

	void  Awake (){
		slot = GetComponent<Slot>();
	}
	
	public Chip GetChip (){
		return chip;
	}
	
	public void  SetChip ( Chip c  ){
		if (chip) chip.parentSlot = null;
		chip = c;
		chip.transform.parent = transform;
		if (chip.parentSlot) {
			chip.parentSlot.chip = null;
			FieldAssistant.main.field.chips[chip.parentSlot.slot.x, chip.parentSlot.slot.y] = -1;
		}
		chip.parentSlot = this;
		FieldAssistant.main.field.chips[slot.x, slot.y] = chip.id;
	}
	
	public void  CrushChip (){
		chip.DestroyChip();
		chip = null;
	}
	
	public SessionAssistant.Solution MatchAnaliz (){
		
		if (!GetChip()) return null;
		if (!GetChip().IsMatcheble()) return null;
		
		
		int t = 0;
		int r = 0;
		int b = 0;
		int l = 0;
		int potencialV = 0;
		int potencialH = 0;
		
		int x = slot.x;
		int y = slot.y;
		
		int width = FieldAssistant.main.field.width;
		int height = FieldAssistant.main.field.height;
		
		Slot s;
		GameObject o;
		
		for (x = slot.x + 1; x < width; x++) {
			o = GameObject.Find("Slot_" + x + "x" + slot.y);
			if (!o) break;
			s = o.GetComponent<Slot>();
			if (!s) break;
			if (!s.GetChip()) break;
			if (s.GetChip().id != chip.id) break;
			if (!s.GetChip().IsMatcheble()) break;
			potencialH += s.GetChip().GetPotencial();
			r++;
		}
		
		for (x = slot.x - 1; x >= 0; x--) {
			o = GameObject.Find("Slot_" + x + "x" + slot.y);
			if (!o) break;
			s = o.GetComponent<Slot>();
			if (!s) break;
			if (!s.GetChip()) break;
			if (s.GetChip().id != chip.id) break;
			if (!s.GetChip().IsMatcheble()) break;
			potencialH += s.GetChip().GetPotencial();
			l++;
		}
		
		for (y = slot.y + 1; y < height; y++) {
			o = GameObject.Find("Slot_" + slot.x + "x" + y);
			if (!o) break;
			s = o.GetComponent<Slot>();
			if (!s) break;
			if (!s.GetChip()) break;
			if (s.GetChip().id != chip.id) break;
			if (!s.GetChip().IsMatcheble()) break;
			potencialV += s.GetChip().GetPotencial();
			t++;
		}
		
		for (y = slot.y - 1; y >= 0; y--) {
			o = GameObject.Find("Slot_" + slot.x + "x" + y);
			if (!o) break;
			s = o.GetComponent<Slot>();
			if (!s) break;
			if (!s.GetChip()) break;
			if (s.GetChip().id != chip.id) break;
			if (!s.GetChip().IsMatcheble()) break;
			potencialV += s.GetChip().GetPotencial();
			b++;
		}
		
		if (r + l >= 2 || t + b >= 2) {
			SessionAssistant.Solution solution = new SessionAssistant.Solution();
			solution.h = r + l >= 2;
			solution.v = t + b >= 2;
			solution.x = slot.x;
			solution.y = slot.y;
			solution.count = 1;
			solution.count += solution.v ? t + b : 0;
			solution.count += solution.h ? r + l : 0;
			solution.id = chip.id;
			solution.posH = r;
			solution.negH = l;
			solution.posV = t;
			solution.negV = b;
			if (solution.v) solution.potencial += potencialV;
			if (solution.h) solution.potencial += potencialH;
			solution.potencial += chip.GetPotencial();
//			if (solution.v && !solution.h && solution.count == 5) solution.potencial += Chip.GetPotencial(3);
//			if (!solution.v && solution.h && solution.count == 5) solution.potencial += Chip.GetPotencial(3);
//			if (solution.v && solution.h && solution.count >= 5) solution.potencial += Chip.GetPotencial(2);
//			if (solution.v && !solution.h && solution.count == 4) solution.potencial += Chip.GetPotencial(1);
//			if (!solution.v && solution.h && solution.count == 4) solution.potencial += Chip.GetPotencial(1);
			return solution;
		}
		return null;
	}
}