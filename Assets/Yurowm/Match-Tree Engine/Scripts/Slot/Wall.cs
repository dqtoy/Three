using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {
	
	
	public int x = 4;
	public int y = 3;
	public bool  h;
	
	public void  Initialize (){
		if (h) {
			if (FieldAssistant.main.field.slots[x,y] && FieldAssistant.main.field.slots[x,y+1]) {
				FieldAssistant.main.GetNearSlot(x, y, Side.Top).SetWall(Side.Bottom);
				FieldAssistant.main.GetSlot(x, y).SetWall(Side.Top);
			} else Destroy(gameObject);
		}
		if (!h) {
			if (FieldAssistant.main.field.slots[x,y] && FieldAssistant.main.field.slots[x+1,y]) {
				FieldAssistant.main.GetNearSlot(x, y, Side.Right).SetWall(Side.Left);
				FieldAssistant.main.GetSlot(x, y).SetWall(Side.Right);
			} else Destroy(gameObject);
		}
		
	}
}