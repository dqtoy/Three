using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Slot : MonoBehaviour {

	bool  container;
	public bool  gravity;
	bool  destroyable;
	bool  matching;
	Jelly jelly;
	Block block;

	bool initialized = false;
	
	public int x;
	public int y;

	public Dictionary<Side, Slot> nearSlot = new Dictionary<Side, Slot> ();
	public Slot this[Side index] {
		get {
			return nearSlot[index];
		}
	}
	
	SlotForChip slotForChip;
	SlotGravity slotGravity;
	
	public Dictionary<Side, bool> wallMask = new Dictionary<Side, bool> ();
	
	void  Awake (){
		slotForChip = GetComponent<SlotForChip>();
		slotGravity = GetComponent<SlotGravity>();
	}
	
	public void  Initialize (){
		if (initialized) return;
		foreach (Side side in Utils.allSides)
			nearSlot.Add(side, FieldAssistant.main.GetNearSlot(x, y, side));
		foreach (Side side in Utils.straightSides)
			wallMask.Add(side, false);
		initialized = true;
	}
	
	public void  SetChip ( Chip c  ){
		if (slotForChip) {
			FieldAssistant.main.field.chips[x, y] = c.id;
			FieldAssistant.main.field.powerUps[x, y] = c.powerId;
			slotForChip.SetChip(c);
		}
		else Debug.LogError("I cant have a chip!!! P.S. My name is " + transform.name);
	}
	
	public Chip GetChip (){
		if (slotForChip) return slotForChip.GetChip();
		return null;
	}
	
	public Block GetBlock (){
		return block;
	}
	
	public Jelly GetJelly (){
		return jelly;
	}
	
	public void  SetBlock ( Block b  ){
		block = b;
	}
	
	public void  SetJelly ( Jelly j  ){
		jelly = j;
	}
	
	void  CrushChip (){
		if (slotForChip) slotForChip.CrushChip();
		else Debug.LogError("I dont have any chip!!! P.S. My name is " + transform.name);
	}
	
	public bool GetShadow (){
		if (slotGravity) return slotGravity.shadow;
		else return false;
	}
	
	public bool GetChipShadow (){
		Slot s = nearSlot[Side.Top];
		while (true) {
			if (!s) return false;
			if (!s.gravity)  return false;
			if (!s.GetChip()) s = s.nearSlot[Side.Top];
			else return true;
		}
	}
	
	public void  SetWall (Side side){

		wallMask[side] = true;

		foreach (Side s in Utils.straightSides)
			if (wallMask[s] == true) {
				if (nearSlot[s]) nearSlot[s].nearSlot[Utils.MirrorSide(s)] = null;
				nearSlot[s] = null;
			}
	
		foreach (Side s in Utils.slantedSides)
			if (wallMask[Utils.SideHorizontal(s)] == true && wallMask[Utils.SideVertical(s)] == true) {
				if (nearSlot[s]) nearSlot[s].nearSlot[Utils.MirrorSide(s)] = null;
				nearSlot[s] = null;
			}

	}
}