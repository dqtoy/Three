using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Text))]
public class Timer : MonoBehaviour {
	
	
	Text label;
	float min = 0;
	float sec = 0;
	
	void  Awake (){
		label = GetComponent<Text> ();
	} 

	void OnEnable() {
		StartCoroutine (TimerRoutine ());
	}

	void OnDisable() {
		StopAllCoroutines ();
	}
	
	void  Update (){
		label.text = ToTimerFormat(Mathf.Max(0, SessionAssistant.main.timeLeft));
	}

	IEnumerator TimerRoutine ()
	{
		yield return 0;
		while(SessionAssistant.main.timeLeft > 10)
			yield return new WaitForSeconds(0.1f);
		while (SessionAssistant.main.timeLeft > 0) {
			AudioAssistant.Shot("TimeWarrning");
			yield return new WaitForSeconds(1f);
			}
	}

	string ToTimerFormat(float t) {
		string f = "";
		min = Mathf.FloorToInt (t / 60);
		sec = Mathf.FloorToInt(t - 60f * min);
		f += min.ToString ();
		if (f.Length < 2)
			f = "0" + f;
		f += ":";
		if (sec.ToString().Length < 2)
			f += "0";
		f += sec.ToString ();
		return f;
	}
}