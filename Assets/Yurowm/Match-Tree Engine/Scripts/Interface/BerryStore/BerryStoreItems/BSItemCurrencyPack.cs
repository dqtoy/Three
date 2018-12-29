using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BSItemCurrencyPack : MonoBehaviour {

	public string packID;

	public Text title;
	public Text description;
	public Text priceTag;

	public BerryStorePurchaser buyButton;

	void Start () {
		BerryStoreAssistant.StoreCurrencyPack pack =  null;

		foreach (BerryStoreAssistant.StoreCurrencyPack p in BerryStoreAssistant.main.currencyPacks)
			if (p.id == packID)
				pack = p;
		if (pack != null) {
			priceTag.text = pack.price.ToString() + "$";
			title.text = pack.name;
			description.text = pack.description;

			buyButton.id = packID;
		} else {
			Debug.LogError("Item " + packID + " is not founded!");
		}

		Destroy (this);
	}

//	void OnEnable() {
//		StartCoroutine (Remove());
//	}

//	IEnumerator Remove ()
//	{
//		yield return 0;
//		Debug.Log ("On ENable");
//
////		if (!gameObject.activeInHierarchy) yield break;
//
//		HorizontalLayoutGroup[] group = gameObject.GetComponentsInChildren<HorizontalLayoutGroup>();
//		ContentSizeFitter[] fitter = gameObject.GetComponentsInChildren<ContentSizeFitter> ();
//		LayoutElement[] element = gameObject.GetComponentsInChildren<LayoutElement> (); 
//
//		for (int g = 0; g < group.Length; g++) Destroy(group[g]);
//		for (int f = 0; f < fitter.Length; f++) Destroy(fitter[f]);
//		for (int e = 0; e < element.Length; e++) Destroy(element[e]);
//	}
}
