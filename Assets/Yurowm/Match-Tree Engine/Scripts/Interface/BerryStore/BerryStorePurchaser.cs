using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Button))]
public class BerryStorePurchaser : MonoBehaviour {

	public string id;

	Button button;

	void Awake () {
		button = GetComponent<Button> ();
		button.onClick.AddListener(() => {
			OnClick();
		});
	}
	
	void OnClick () {
		BerryStoreAssistant.main.Purchase (id);
	}

	public static void Succeed(string sku) {

	}
}
