using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentAssistant : MonoBehaviour {

	public static ContentAssistant main;
	
	public List<ContentAssistantItem> cItems;

	private Dictionary<string, GameObject> content = new Dictionary<string, GameObject>();

	private Transform front;
	private Transform quarantine;

	private GameObject zObj;

	void Awake () {
		main = this;
		content.Clear ();
		foreach (ContentAssistantItem item in cItems)
			content.Add(item.item.name, item.item);
	}
	
	public T GetItem<T> (string key) where T : Component {
		return ((GameObject) Instantiate (content [key])).GetComponent<T>();
	}

	public GameObject GetItem (string key) {
		return (GameObject) Instantiate (content [key]);
	}

	public T GetItem<T> (string key, Vector3 position) where T : Component {
		zObj = GetItem (key);
		zObj.transform.position = position;
		return zObj.GetComponent<T>();
	}

	public GameObject GetItem(string key, Vector3 position) {
		zObj = GetItem (key);
		zObj.transform.position = position;
		return zObj;
	}

	public GameObject GetItem(string key, Vector3 position, Quaternion rotation) {
		zObj = GetItem (key, position);
		zObj.transform.rotation = rotation;
		return zObj;
	}


	[System.Serializable]
	public struct ContentAssistantItem {
		public GameObject item;
		public string category;
	}
}