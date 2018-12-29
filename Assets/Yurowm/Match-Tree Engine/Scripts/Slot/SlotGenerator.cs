using UnityEngine;
using System.Collections;

public class SlotGenerator : MonoBehaviour {
	
	
	public Slot slot;
	public SlotForChip slotForChip;
	public Chip chip;

	bool  auto = false;
	
	float lastTime = -10;
	float delay = 0.15f;

//	float timer = 0;
	
	void  Awake (){
		slot = GetComponent<Slot>();
		slotForChip = GetComponent<SlotForChip>(); 
	}
	
	void  Update (){
		if (!SessionAssistant.main.CanIGravity ()) {
			return;
		}

		chip = slotForChip.GetChip();
		
		if (chip) return;
		
		if (slot.GetBlock()) return;
		
		if (lastTime + delay > Time.time) return;
		
		lastTime = Time.time;
		
		if (!auto && slot.y != FieldAssistant.height - 1) {
			Destroy(this);
			return;
		}
		
		FieldAssistant.main.GetNewSimpleChip(slot.x, slot.y, transform.position + new Vector3(0, 0.2f, 0));
	}
}