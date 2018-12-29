using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Soomla.Store;

public class BoosterButton : MonoBehaviour {

	public enum BoosterButtonType {Page, Message};
	public string boosterItemId;
	public BoosterButtonType type;
	public string value;
	public FieldTarget[] modeMask;

	Button button;
	
	void Awake () {
		button = GetComponent<Button> ();
		button.onClick.AddListener(() => {
			OnClick();
		});
	}

	
	void OnClick () {
		if (StoreInventory.GetItemBalance (boosterItemId) == 0) {
			UIServer.main.ShowPage("Store");
			return;
		}
		if (type == BoosterButtonType.Message)
			SendMessage(value, SendMessageOptions.DontRequireReceiver);
		if (type == BoosterButtonType.Page)
			UIServer.main.ShowPage(value);
	}
}
