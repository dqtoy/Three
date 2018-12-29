using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIServer : MonoBehaviour {

	public static UIServer main;

	public string defaultPage;
	public Page[] pages;

	private Dictionary<string, CPanel> dPanels = new Dictionary<string, CPanel>();
	private Dictionary<string, Dictionary<string, bool>> dPages = new Dictionary<string, Dictionary<string, bool>>();

	private string currentPage;
	private string previousPage;

	void Start () {
		ArraysConvertation();
		foreach (string key in dPanels.Keys)
			dPanels[key].gameObject.SetActive(false);
		ShowPage (defaultPage);
	}

	void Awake () {
		main = this;
	}

	void ArraysConvertation () {
		foreach (CPanel gm in GetComponentsInChildren<CPanel>(true))
			dPanels.Add(gm.name, gm);
		foreach (Page pg in pages) {
			Dictionary<string, bool> p = new Dictionary<string, bool>();
			for (int i = 0; i < pg.panels.Length; i++)
				p.Add(pg.panels[i], true);
			dPages.Add(pg.name, p);
		}
	}

	public void ShowPage (string p) {
		if (currentPage == p) return;
		previousPage = currentPage;
		currentPage = p;
		foreach (string key in dPanels.Keys) {
			if (dPages[p].ContainsKey("?" + key)) continue;
			dPanels[key].SetActive(dPages[p].ContainsKey(key));
		}
	}

	public void HideAll () {
		foreach (string key in dPanels.Keys) {
			dPanels[key].SetActive(false);
		}
	}

	public void ShowPreviousPage () {
		ShowPage (previousPage);
	}

	public void SetPause (bool p) {
		Time.timeScale = p ? 0 : 1;
	}

	[System.Serializable]
	public struct Page {
		public string name;
		public string[] panels;
	}
}