using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	public Slot slot;
	
	
	public int level = 1;
	public Sprite[] sprites;
	SpriteRenderer sr;
	int eventCountBorn;

	public void Initialize (){
		slot.gravity = false;
		sr = GetComponent<SpriteRenderer>();
		eventCountBorn = SessionAssistant.main.eventCount;
		sr.sprite = sprites[level-1];
	}
	
	public void  BlockCrush () {
		if (eventCountBorn == SessionAssistant.main.eventCount) return;
		eventCountBorn = SessionAssistant.main.eventCount;
		GameObject o = ContentAssistant.main.GetItem ("BlockCrush");
		o.transform.position = transform.position;
		level --;
		FieldAssistant.main.field.blocks [slot.x, slot.y] = level;
		if (level == 0) {
			slot.gravity = true;
			SlotGravity.Reshading();
			Destroy(gameObject);
			return;
		}
		GetComponent<Animation>().Play("BlockCrush");
		sr.sprite = sprites[level-1];
	}
	
	
}