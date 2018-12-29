using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Text))]
public class TargetScoreCounter : MonoBehaviour {

	Text label;
	
	void  Awake (){
		label = GetComponent<Text> ();
	} 
	
	void  OnEnable (){
		if (LevelProfile.main != null)
			label.text = LevelProfile.main.firstStarScore.ToString();
	}
}