using UnityEngine;
using System.Collections;

public class Jelly : MonoBehaviour {

	public int level = 1;
	public Sprite[] sprites;
	SpriteRenderer sr;
	
	void  Start (){
		sr = GetComponent<SpriteRenderer>();
		sr.sprite = sprites[level-1];
	}

	void  JellyCrush (){
		GameObject o = ContentAssistant.main.GetItem ("JellyCrush");
		o.transform.position = transform.position;
		if (level == 1) {
			Destroy(gameObject);
			return;
		}
		level --;
		sr.sprite = sprites[level-1];
	}
}