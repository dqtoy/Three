using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Soomla.Store;

[RequireComponent (typeof (Text))]
public class ItemCounter : MonoBehaviour {

	Text label;
	public string itemID;

	void Awake () {
		label = GetComponent<Text> ();
	}
	
	void OnEnable () {
		Refresh ();
	}

	public void Refresh() {
		label.text = StoreInventory.GetItemBalance(itemID).ToString();
	}

	public static void RefreshAll() {
		foreach (ItemCounter counter in GameObject.FindObjectsOfType<ItemCounter> ())
			counter.Refresh();
		foreach (ItemMask mask in GameObject.FindObjectsOfType<ItemMask> ())
			mask.Refresh();
	}
}