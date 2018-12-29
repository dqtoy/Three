using UnityEngine;
using System.Collections;
using Soomla.Store;

public class ItemMask : MonoBehaviour {

	public string itemID;
	public ComparisonOperator mustBe;
	public int value = 1;

	public enum ComparisonOperator {Less, Greater, Equal, EqualLess, EqualGreater};

	void Start () {
		Refresh ();
	}

	void OnEnable () {
		Refresh ();
	}

	public void Refresh ()
	{
		int balance = StoreInventory.GetItemBalance (itemID);
		bool result = false;
		switch (mustBe) {
		case ComparisonOperator.Less: result = balance < value; break;
		case ComparisonOperator.Greater: result = balance > value; break;
		case ComparisonOperator.EqualLess: result = balance <= value; break;
		case ComparisonOperator.EqualGreater: result = balance >= value; break;
		case ComparisonOperator.Equal: result = balance == value; break;
		}
		SetVisible (result);
	}
	
	void SetVisible (bool v) {
		foreach (Transform t in transform) {
			t.gameObject.SetActive(v);
		}
	}
}
