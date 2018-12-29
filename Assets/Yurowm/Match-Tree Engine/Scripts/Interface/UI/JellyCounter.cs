using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Text))]
public class JellyCounter : MonoBehaviour {
	
	
	Text label;
	int count = 0;
	float lastUpdate = 0;

	void  Awake (){
		label = GetComponent<Text> ();
	} 
	
	void OnEnable () {
		lastUpdate = -9E9f;
		Update ();
	}
	
	void  Update (){
		if (lastUpdate + 0.3f > Time.unscaledTime) return;
		lastUpdate = Time.unscaledTime;
		count = GameObject.FindObjectsOfType<Jelly> ().Length;

		label.text = count.ToString();
	}
}