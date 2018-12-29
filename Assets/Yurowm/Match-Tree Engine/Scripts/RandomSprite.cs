using UnityEngine;
using System.Collections;

public class RandomSprite : MonoBehaviour {
	
	
	public Sprite[] sprites;
	
	void  Start (){
		int id = Random.Range(0, sprites.Length);
		GetComponent<SpriteRenderer>().sprite = sprites[id];
		Destroy(this);
	}
}