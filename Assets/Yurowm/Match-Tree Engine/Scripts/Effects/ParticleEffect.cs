using UnityEngine;
using System.Collections;

public class ParticleEffect : MonoBehaviour {
	
	ParticleSystem ps;
	public string sortingLayer;
	public int sortingOrder;
	public bool killAfterLifetime = true;
	
	void  Start (){
		ps = GetComponent<ParticleSystem>();
		ps.GetComponent<Renderer>().sortingLayerName = sortingLayer;
		ps.GetComponent<Renderer>().sortingOrder = sortingOrder;
		if (killAfterLifetime) StartCoroutine("Kill");
	}
	
	IEnumerator Kill (){
		yield return new WaitForSeconds(ps.duration + ps.startLifetime);
		Destroy(gameObject);
	}
}