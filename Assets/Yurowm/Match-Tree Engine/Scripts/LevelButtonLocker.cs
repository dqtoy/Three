using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (LevelButton))]
[RequireComponent (typeof (Button))]

public class LevelButtonLocker : MonoBehaviour {

	LevelButton level;
	Button button;
	int num = 0;
	public bool alwaysUnlocked = false;
	public GameObject[] lockedElements;
	public GameObject[] unlockedElements;


	void Awake () {
		level = GetComponent<LevelButton> ();
		button = GetComponent<Button> ();
	}

	void OnEnable () {
		num = level.GetNumber ();
		bool l = !alwaysUnlocked && num > 1 && PlayerPrefs.GetInt ("Complete_" + (num - 1).ToString ()) == 0;
		if (l && Application.isEditor) {
			l = false;
			Debug.Log("Level " + level.GetNumber().ToString() + " was unlocked because application was run in editor mode.");
		}
		foreach(GameObject go in lockedElements)
			go.SetActive (l);
		foreach(GameObject go in unlockedElements)
			go.SetActive (!l);
		button.enabled = !l;
	}

	public static void UnlockAllLevels() {
		for (int i = 1; i <= LevelButton.all.Count; i++)
			PlayerPrefs.SetInt ("Complete_" + i.ToString (), 1);
	}
}
