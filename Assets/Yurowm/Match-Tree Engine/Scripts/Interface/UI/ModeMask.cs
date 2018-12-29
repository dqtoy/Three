using UnityEngine;
using System.Collections;

public class ModeMask : MonoBehaviour {

	public FieldTarget[] visibleMask;

	void OnEnable () {
		if (LevelProfile.main == null) {
			SetVisible (false);
			return;
		}
		bool v = false;
		foreach (FieldTarget t in visibleMask) {
			if (t == LevelProfile.main.target) {
				v = true;
				break;
			}
		}
		SetVisible (v);
	}

	void SetVisible (bool v) {
		foreach (Transform t in transform) {
			t.gameObject.SetActive(v);
		}
	}
}
