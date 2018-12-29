using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CPanel : MonoBehaviour {

	public string hide;
	public string show;

	private string currentClip = "";
	

	public void SetActive (bool a) {
		if (gameObject.activeSelf == a) return;
		currentClip = "";
		if (!a) {
			if (hide != "")
				currentClip = hide;
			else {
				gameObject.SetActive(false);
				return;
			}
		}
		if (a) {
			gameObject.SetActive(true);
			if (show != "")
				currentClip = show;
			else return;
			Update();
		}
		if (currentClip == "") return;
		GetComponent<Animation>().Play(currentClip);
		GetComponent<Animation>()[currentClip].time = 0;
	}

	void Update () {
		if (currentClip == "") return;

		GetComponent<Animation>()[currentClip].time += Mathf.Min (Time.unscaledDeltaTime, Time.maximumDeltaTime);
		GetComponent<Animation>()[currentClip].enabled = true;
		GetComponent<Animation>().Sample();
		GetComponent<Animation>()[currentClip].enabled = false;

		if (GetComponent<Animation>() [currentClip].time >= GetComponent<Animation>() [currentClip].length) {
			if (currentClip == hide)
				gameObject.SetActive(false);
			currentClip = "";
		}
	}
}
	
