using UnityEngine;
using System.Collections;

public class ControlAssistant : MonoBehaviour {
	
	
	public static ControlAssistant main;
	RaycastHit2D hit;
	public Camera controlCamera;
	
	Chip pressedChip;
	Vector2 pressPoint;
	
	void  Awake (){
		main = this;
	}
	
	void  Update (){
		if (Time.timeScale == 0) return;
		if (Application.isMobilePlatform)
			MobileUpdate();
		else 
			DecktopUpdate();
	}
	
	void  MobileUpdate (){
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
			Vector2 point = controlCamera.ScreenPointToRay(Input.GetTouch(0).position).origin;
			hit = Physics2D.Raycast(point, Vector2.zero);
			if (!hit.transform) return;
			pressedChip = hit.transform.GetComponent<Chip>();
			pressPoint = Input.GetTouch(0).position;
		}
		if (Input.touchCount > 0 && pressedChip) {
			Vector2 move = Input.GetTouch(0).position - pressPoint;
			if (move.magnitude > Screen.height * 0.05f) {
				if (Mathf.Abs(move.x) < Mathf.Abs(move.y)) {
					if (move.y > 0)
						pressedChip.Swap(Side.Top);
					else pressedChip.Swap(Side.Bottom);
				} else {
					if (move.x > 0)
						pressedChip.Swap(Side.Right);
					else pressedChip.Swap(Side.Left);
				}
				pressedChip = null;
			}
		}
	}
	
	void  DecktopUpdate (){
		if (Input.GetMouseButtonDown(0)) {
			Vector2 point = controlCamera.ScreenPointToRay(Input.mousePosition).origin;
			hit = Physics2D.Raycast(point, Vector2.zero);
			if (!hit.transform) return;
			pressedChip = hit.transform.GetComponent<Chip>();
			pressPoint = Input.mousePosition; 
		}
		if (Input.GetMouseButton(0) && pressedChip != null) {
			Vector2 move = Input.mousePosition;
			move -= pressPoint;
			if (move.magnitude > Screen.height * 0.05f) {
				if (Mathf.Abs(move.x) < Mathf.Abs(move.y)) {
					if (move.y > 0)
						pressedChip.Swap(Side.Top);
					else pressedChip.Swap(Side.Bottom);
				} else {
					if (move.x > 0)
						pressedChip.Swap(Side.Right);
					else pressedChip.Swap(Side.Left);
				}
				pressedChip = null;
			}
		}
	}

	public Chip GetChipFromTouch() {
		Vector2 point;
		if (Application.isMobilePlatform) {
			if (Input.touchCount == 0) return null;
			point = controlCamera.ScreenPointToRay(Input.GetTouch(0).position).origin;
		} else 
			point = controlCamera.ScreenPointToRay(Input.mousePosition).origin;

		hit = Physics2D.Raycast(point, Vector2.zero);
		if (!hit.transform) return null;
		return hit.transform.GetComponent<Chip>();
	}
}