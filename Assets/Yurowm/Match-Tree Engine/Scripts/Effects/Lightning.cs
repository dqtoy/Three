using UnityEngine;
using System.Collections;

public class Lightning: MonoBehaviour {
	
	public Transform start;
	public Transform end;
	public int bend = 2;
	public Vector3[] bendPoint;
	public Color color;
	
	public string sortingLayer;
	public int sortingOrder;

	LineRenderer line;
	float distance = 0f;
	float lastTime = -100f;
	float frequency = 3f;
	bool  destroing = false;
	Vector3 a;
	Vector3 b;
	
	
	void  Start (){
		line = GetComponent<LineRenderer>();
		bendPoint = new Vector3[bend];
		line.SetColors(color, color);
		line.SetVertexCount(bend + 2);
		line.GetComponent<Renderer>().sortingLayerName = sortingLayer;
		line.sortingOrder = sortingOrder;
	}
	
	void  Update (){
		if (end == null || !end.gameObject.activeSelf || start == null || !start.gameObject.activeSelf) {
			StartCoroutine("FadeOut");
		}
		
		if (!destroing) {
			a = start.position;
			b = end.position;
		}
		distance = (a - b).magnitude;
		if (lastTime + 1f/frequency < Time.time) {
			lastTime = Time.time;
			for (int i = 0; i < bendPoint.Length; i++)
				bendPoint[i] = new Vector3((2f * Random.value - 1f) * 0.1f * distance, (2f * Random.value - 1f) * 0.1f * distance, 0f);
		}
		line.SetPosition(0, a);
		for (int i= 1; i < bend + 1; i++) {
			line.SetPosition(i, Vector3.Lerp(a, b, (1f * i)/(bend+1)) + bendPoint[i-1]);
		}
		line.SetPosition(bend + 1, b);
	}
	
	IEnumerator FadeOut (){
		if (destroing) yield break;
		destroing = true;
		GetComponent<Animation>().Play("LightningFadeOut");
		while (GetComponent<Animation>().isPlaying) yield return 0;
		Destroy(gameObject);
	}

	public static Lightning CreateLightning ( int bend ,   Transform start ,   Transform end ,   Color color  ){
		Lightning newLightning = ContentAssistant.main.GetItem<Lightning> ("Lightning");
		newLightning.bend = bend;
		newLightning.start = start;
		newLightning.end = end;
		newLightning.color = color;
		return newLightning;
	}
}