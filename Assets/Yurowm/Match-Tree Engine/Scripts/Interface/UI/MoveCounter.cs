using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Text))]
public class MoveCounter : MonoBehaviour {

	Text label;

	void Awake () {
		label = GetComponent<Text> ();
	}
	
	void Update () {
		label.text = SessionAssistant.main.movesCount.ToString();
	}
}
