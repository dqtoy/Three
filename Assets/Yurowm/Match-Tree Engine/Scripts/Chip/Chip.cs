using UnityEngine;
using System.Collections;

public class Chip : MonoBehaviour {

	public SlotForChip parentSlot;
	public string chipType = "None";
	public int id;
	public int powerId;
	public bool  move = false;
	public int movementID = 0;
	public Vector3 impuls = Vector3.zero;
	Vector3 impulsParent = new Vector3(0,0,-1);
	Vector3 startPosition;
	Vector3 moveVector;
	public bool  destroing = false;
	float velocity = 0;
	float acceleration = 14f;
	static float velocityLimit = 12f;
	
	public static Color[] colors = {new Color(0.75f, 0.3f, 0.3f),
		new Color(0.3f, 0.75f, 0.3f),
		new Color(0.3f, 0.5f, 0.75f),
		new Color(0.75f, 0.75f, 0.3f),
		new Color(0.75f, 0.3f, 0.75f),
		new Color(0.75f, 0.6f, 0.3f)};
	
	Vector3 lastPosition;
	Vector3 zVector;
	
	void  Awake (){
		move = true;
		velocity = 1;
		SessionAssistant.main.gravity ++;
	}
	
	public bool IsMatcheble (){
		if (destroing) return false;
		if (SessionAssistant.main.gravity == 0) return true;
		if (move) return false;
		if (transform.position != parentSlot.transform.position) return false;
		if (velocity != 0) return false;

		foreach (Side side in Utils.straightSides)
			if (parentSlot[side]
			&& parentSlot[side].gravity
			&& !parentSlot[side].GetShadow()
			&& !parentSlot[side].GetChip())
				return false;

		return true;
	}
	
	void  Update (){
		if (!SessionAssistant.main.isPlaying) return;
		if (impuls != Vector3.zero && (parentSlot || impulsParent.z != -1)) {
			if (impulsParent.z == -1) {
				if (!parentSlot) {
					impuls = Vector3.zero;
					return;
				}
				if (!move) SessionAssistant.main.gravity ++;
				move = true;
				impulsParent = parentSlot.transform.position;
			} 
			transform.position += impuls * Time.deltaTime;
			transform.position += (impulsParent - transform.position) * Time.deltaTime;
			impuls -= impuls * Time.deltaTime;
			impuls -= transform.position - impulsParent;
			impuls *= 1 - 6*Time.deltaTime;
			if ((transform.position - impulsParent).magnitude < 2 * Time.deltaTime && impuls.magnitude < 2) {
				impuls = Vector3.zero;
				transform.position = impulsParent;
				impulsParent.z = -1;
				if (move) {
					AudioAssistant.Shot("ChipHit");
					SessionAssistant.main.gravity --;
				}

				move = false;
			}
			return;
		}
		
		if (!SessionAssistant.main.CanIGravity()) return;
		if (destroing) return;
		
		if (SessionAssistant.main.matching > 0 && !move) return;
		moveVector.x = 0;
		moveVector.y = 0;
		
		if (parentSlot && transform.position != parentSlot.transform.position) {
			if (!move) {
				move = true;
				SessionAssistant.main.gravity ++;
				velocity = 2;
			}
			
			velocity += acceleration * Time.deltaTime;
			if (velocity > velocityLimit) velocity = velocityLimit;
			
			lastPosition = transform.position;
			
			if (Mathf.Abs(transform.position.x - parentSlot.transform.position.x) < velocity * Time.deltaTime) {
				zVector = transform.position;
				zVector.x = parentSlot.transform.position.x;
				transform.position = zVector;
			}
			if (Mathf.Abs(transform.position.y - parentSlot.transform.position.y) < velocity * Time.deltaTime) {
				zVector = transform.position;
				zVector.y = parentSlot.transform.position.y;
				transform.position = zVector;
			}
			
			if (transform.position == parentSlot.transform.position) {
				parentSlot.SendMessage("GravityReaction");
				if (transform.position != parentSlot.transform.position) 
					transform.position = lastPosition;
			}
			
			if (transform.position.x < parentSlot.transform.position.x)
				moveVector.x = 10;
			if (transform.position.x > parentSlot.transform.position.x)
				moveVector.x = -10;
			if (transform.position.y < parentSlot.transform.position.y)
				moveVector.y = 10;
			if (transform.position.y > parentSlot.transform.position.y)
				moveVector.y = -10;
			moveVector = moveVector.normalized * velocity;
			transform.position += moveVector * Time.deltaTime;
		} else {
			if (move) {
				move = false;
				velocity = 0;
				movementID = SessionAssistant.main.GetMovementID();
				AudioAssistant.Shot("ChipHit");
				SessionAssistant.main.gravity --;
			}
		}
	}
	
	public int GetPotencial (){
		return GetPotencial(powerId);
	}
	
	public static int GetPotencial ( int i  ){
		if (i == 0) return 1;
		if (i == 1) return 7;
		if (i == 2) return 12;
		if (i == 3) return 30;
		return 0;
	}
	
	public void  Swap (Side side){
		if (parentSlot[side]) AnimationAssistant.main.SwapTwoItem(this, parentSlot[side].GetChip());
	}
	
	public void  ParentRemove (){
		if (!parentSlot) return;
		parentSlot.chip = null;
		parentSlot = null;
	}
	
	void  OnDestroy (){
		if (move) SessionAssistant.main.gravity --;
		
	}
	
	public void  DestroyChip (){
		if (destroing) return;
		destroing = true;
		SendMessage("DestroyChipFunction");
	}

	public void  HideChip (){
		if (destroing) return;
		destroing = true;
		ParentRemove();
		Destroy(gameObject);
	}

	public void  SetScore (float s){
		if (id < 0 || id > 5 ) return;
		SessionAssistant.main.score += Mathf.RoundToInt(s * SessionAssistant.scoreC);
		ScoreBubble.Bubbling(Mathf.RoundToInt(s * SessionAssistant.scoreC), transform, id);
	}

	public void Flashing ( int eventCount  ){
		StartCoroutine (FlashingUntil (eventCount));
	}

	IEnumerator  FlashingUntil ( int eventCount  ){
		GetComponent<Animation>().Play("Flashing");
		while (eventCount == SessionAssistant.main.eventCount) yield return 0;
		if (!this) yield break;
		while (GetComponent<Animation>()["Flashing"].time % GetComponent<Animation>()["Flashing"].length > 0.1f) yield return 0;
		GetComponent<Animation>()["Flashing"].time = 0;
		yield return 0;
		GetComponent<Animation>().Stop("Flashing");
	}
}