using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Image))]
public class ScoreStar : MonoBehaviour {

	Image image;
	float lastUpdate = 0;
	bool filled = false;

	public Sprite fullStar;
	public Sprite emptyStar;
	public StarType starType;

	void Awake () {
		image = GetComponent<Image> ();
	}

	void OnEnable () {
		lastUpdate = 0;
		filled = false;
		image.sprite = emptyStar;
	}

	void Update () {
		if (filled) return;
		if (lastUpdate + 0.5f > Time.unscaledTime) return;
		lastUpdate = Time.unscaledTime;
		float target = 0;
		switch (starType) {
			case StarType.First: target = LevelProfile.main.firstStarScore; break;
			case StarType.Second: target = LevelProfile.main.secondStarScore; break;
			case StarType.Third: target = LevelProfile.main.thirdStarScore; break;
		}
		filled = target <= SessionAssistant.main.score;
		if (filled)
			image.sprite = fullStar;
	}
}
