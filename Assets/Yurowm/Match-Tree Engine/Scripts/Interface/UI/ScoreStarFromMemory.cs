using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Image))]
public class ScoreStarFromMemory : MonoBehaviour {
	
	Image image;
	
	public Sprite fullStar;
	public Sprite emptyStar;
	public StarType starType;
	LevelButton button;
	
	void Awake () {
		image = GetComponent<Image> ();
		button = GetComponentInParent<LevelButton> ();
	}
	
	void OnEnable () {
		float target = 0;
		switch (starType) {
			case StarType.First: target = button.profile.firstStarScore; break;
			case StarType.Second: target = button.profile.secondStarScore; break;
			case StarType.Third: target = button.profile.thirdStarScore; break;
		}
		bool filled = PlayerPrefs.GetInt ("Best_Score_" + button.GetNumber ()) >= target;
		image.sprite = filled ? fullStar : emptyStar;
	}
}
