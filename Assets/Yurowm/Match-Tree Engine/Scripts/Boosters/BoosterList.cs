using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (GridLayoutGroup))]
public class BoosterList : MonoBehaviour {

	List<KeyValuePair<FieldTarget, Transform>> pairs = new List<KeyValuePair<FieldTarget, Transform>>();

	void Awake () {
		Initialize ();
	}

	void Initialize ()
	{
		BoosterButton button;
		KeyValuePair<FieldTarget, Transform> pair;
		foreach (Transform booster in transform) {
			button = booster.GetComponent<BoosterButton>();
			foreach (FieldTarget target in button.modeMask) {
				pair = new KeyValuePair<FieldTarget, Transform>(target, booster);
				pairs.Add(pair);
			}
		}
	}

	void OnEnable () {
		Refresh ();
	}

	void Refresh ()
	{
		if (LevelProfile.main == null) return;
		foreach (Transform booster in transform)
			booster.gameObject.SetActive(false);
		
		foreach (KeyValuePair<FieldTarget, Transform> pair in pairs)
			if (pair.Key == LevelProfile.main.target)
				pair.Value.gameObject.SetActive(true);
	}
}
