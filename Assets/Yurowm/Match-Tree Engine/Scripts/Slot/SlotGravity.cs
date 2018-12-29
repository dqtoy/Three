using UnityEngine;
using System.Collections;

public class SlotGravity : MonoBehaviour {
	
	
	public Slot slot;
	public SlotForChip slotForChip;
	public Chip chip;
	
	public bool  shadow;
	
	void  Awake (){
		slot = GetComponent<Slot>();
		slotForChip = GetComponent<SlotForChip>(); 
	}
	
	void  Start (){
		Shading();
	}
	
	public static void  Reshading (){
		SlotGravity[] sgs = GameObject.FindObjectsOfType<SlotGravity>();
		foreach(SlotGravity sg in sgs) sg.Shading();
	}
	
	public void  Shading (){
		int yMax = FieldAssistant.height;
		Slot s;
		string key;
		
		if (!slot[Side.Top] && slot.y != yMax-1) {
			shadow = true;
			return;
		}
		for (int y = slot.y + 1; y < yMax; y++) {
			key = slot.x + "x" + y;
			s = FieldAssistant.main.slots[key];
			if (!s || !s.gravity || (!s[Side.Top] && s.y != yMax-1)) {
				shadow = true;
				return;
			}
		}
		shadow = false;
	}
	
	void  Update (){
		GravityReaction();
	}
	
	void  GravityReaction (){
		if (!SessionAssistant.main.CanIGravity()) return;
		
		chip = slotForChip.GetChip();
		if (!chip) return;
		
		if (transform.position != chip.transform.position) return;
		
		if (slot[Side.Bottom] && slot[Side.Bottom].gravity) {
			if (!slot[Side.Bottom].GetChip()) {
				slot[Side.Bottom].SetChip(chip);
				GravityReaction();
			} else if (Random.value > 0.5f) {
				SlideLeft();
				SlideRight();
			} else {
				SlideRight();
				SlideLeft();	
			}
		}
	}



	void  SlideLeft (){
		if (slot[Side.BottomLeft]
		    && slot[Side.BottomLeft].gravity
		    && ((slot[Side.Bottom] && slot[Side.Bottom][Side.Left]) || (slot[Side.Left] && slot[Side.Left][Side.Bottom]))
		    && !slot[Side.BottomLeft].GetChip()
		    && slot[Side.BottomLeft].GetShadow()
		    && !slot[Side.BottomLeft].GetChipShadow()
		    && (!shadow || !slot[Side.Top] || !slot[Side.Top].GetChip()) 
		    && !(slot[Side.Bottom] && slot[Side.Bottom].gravity && !slot[Side.Bottom].GetChip())) {
			slot[Side.BottomLeft].SetChip(chip);
		}
	}
	
	void  SlideRight (){
		if (slot[Side.BottomRight]
		    && slot[Side.BottomRight].gravity
		    && ((slot[Side.Bottom] && slot[Side.Bottom][Side.Right]) || (slot[Side.Right] && slot[Side.Right][Side.Bottom]))
		    && !slot[Side.BottomRight].GetChip()
		    && slot[Side.BottomRight].GetShadow()
		    && !slot[Side.BottomRight].GetChipShadow()
		    && (!shadow || !slot[Side.Top] || !slot[Side.Top].GetChip()) 
		    && !(slot[Side.Bottom] && slot[Side.Bottom].gravity && !slot[Side.Bottom].GetChip())) {
			slot[Side.BottomRight].SetChip(chip);
		}
	}
}