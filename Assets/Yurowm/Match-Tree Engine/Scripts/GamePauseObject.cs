using UnityEngine;

public class GamePauseObject : MonoBehaviour {

	static int activeObjectsCount = 0;

	void OnEnable () {
		activeObjectsCount ++;
		PauseUpdate ();
	}

	void OnDisable() {
		activeObjectsCount --;
		PauseUpdate ();
	}

	void PauseUpdate() {
		if (UIServer.main != null)
			UIServer.main.SetPause (activeObjectsCount == 0);
	}
}
