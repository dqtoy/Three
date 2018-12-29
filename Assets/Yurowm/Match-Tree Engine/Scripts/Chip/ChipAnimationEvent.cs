using UnityEngine;
using System.Collections;

public class ChipAnimationEvent : MonoBehaviour {
		
		void  PlayAnimation ( string name  ){
			GetComponent<Animation>().CrossFade(name, 0.2f);
		}
	}