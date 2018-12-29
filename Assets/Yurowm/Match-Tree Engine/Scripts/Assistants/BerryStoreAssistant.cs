using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;

public class BerryStoreAssistant : MonoBehaviour, IStoreAssets {

	[ContextMenu ("Clear Data")]
	public void ClearData() {
		List<string> ids = new List<string> ();
		ids.Add ("seed");

		foreach (StoreItem i in items) ids.Add(i.id);

		foreach (string id in ids)
			StoreInventory.TakeItem(id, StoreInventory.GetItemBalance(id));

		PlayerPrefs.DeleteAll ();
	}

	public void AddSomeSeeds() {
		StoreInventory.GiveItem("seed", Random.Range(10, 100));
		AudioAssistant.Shot ("Buy");
	}

	public static BerryStoreAssistant main;

	public StoreItem[] items;
	public StoreItemPack[] itemPacks;
	public StoreCurrencyPack[] currencyPacks;

	void Awake () {
		main = this;
	}

	void Start () {
		SoomlaStore.Initialize(this);
		StoreEvents.OnCurrencyBalanceChanged += onCurrencyBalanceChanged;
		StoreEvents.OnItemPurchased += onItemPurchased;
		StoreEvents.OnGoodBalanceChanged += onGoodBalanceChanged;
		StoreEvents.OnMarketPurchase += onMarketPurchase;
		StoreEvents.OnNotEnoughTargetItem += onNotEnoughTargetItem;
	}

	#region IStoreAssets implementation
	public int GetVersion () {
		return 1;
	}

	public VirtualCurrency[] GetCurrencies () {
		VirtualCurrency seeds = new VirtualCurrency (
			"Seed", //Name
			"Seed currency", //Description
			"seed" //Currency ID
			);
		return new VirtualCurrency[] {seeds};
	}

	public VirtualGood[] GetGoods () {
		List<VirtualGood> goods = new List<VirtualGood> ();
		foreach (StoreItem item in items)
			goods.Add(item.Complete());
		foreach (StoreItemPack pack in itemPacks)
			goods.Add(pack.Complete());
		return goods.ToArray ();
	}

	public VirtualCurrencyPack[] GetCurrencyPacks ()
	{
		List<VirtualCurrencyPack> packs = new List<VirtualCurrencyPack> ();
		foreach (StoreCurrencyPack pack in currencyPacks)
			packs.Add(pack.Complete());
		return packs.ToArray ();
	}

	public VirtualCategory[] GetCategories ()
	{
		return new VirtualCategory[0];
	}
	#endregion


	string developerPayload = "BerryMatchTreePayload";
	public void Purchase (string id)
	{
		StoreInventory.BuyItem (id, developerPayload);
		
	}
	
	void onCurrencyBalanceChanged(VirtualCurrency virtualCurrency, int balance, int amountAdded) {
		ItemCounter.RefreshAll ();
	}
	
	void onNotEnoughTargetItem (VirtualItem item)
	{
		if (item.ItemId == "seed")
			UIServer.main.ShowPage ("Store");
	}
	
	void onGoodBalanceChanged (VirtualGood good, int balance, int amountAdded)
	{		
		ItemCounter.RefreshAll ();
	}
	
	void onItemPurchased (PurchasableVirtualItem item, string payload)
	{
		if (payload != developerPayload) return;
		AudioAssistant.Shot ("Buy");
		BerryStorePurchaser.Succeed (item.ItemId);
	}
	
	void onMarketPurchase (PurchasableVirtualItem item, string payload, Dictionary<string, string> extra)
	{
		if (payload != developerPayload) return;
		AudioAssistant.Shot ("Buy");
		BerryStorePurchaser.Succeed (item.ItemId);
	}






	[System.Serializable]
	public class StoreItem {

		public string name;
		public string id;
		public string description;
		public float price;
		public Currency currency;
		public string sku;
		public bool consumable = true;

		public VirtualGood Complete() {
			VirtualGood good;
			PurchaseType purchase = null;

			if (currency == Currency.Dollars)
				purchase = new PurchaseWithMarket(sku, price);
			if (currency == Currency.Seeds)
				purchase = new PurchaseWithVirtualItem("seed", Mathf.CeilToInt(price));

			if (consumable)
				good = new SingleUseVG(name, description, id, purchase);
			else
				good = new LifetimeVG(name, description, id, purchase);
			return good;
		}
		public enum Currency {Dollars, Seeds};
	}

	[System.Serializable]
	public class StoreItemPack {
		
		public string name;
		public string id;
		public string description;
		public string itemId;
		public int itemCount;
		public int price;
		public StoreItem.Currency currency;
		public string sku;

		public VirtualGood Complete ()
		{
			PurchaseType purchase;
			
			if (currency == StoreItem.Currency.Dollars)
				purchase = new PurchaseWithMarket(sku, price);
			else
				purchase = new PurchaseWithVirtualItem("seed", Mathf.CeilToInt(price));

			return new SingleUsePackVG (itemId, itemCount, name, description, id, purchase);
		}
	}

	[System.Serializable]
	public class StoreCurrencyPack {
		
		public string name;
		public string id;
		public string description;
		public int count;
		public int price;
		public string sku;

		public VirtualCurrencyPack Complete () {
			return new VirtualCurrencyPack (name, description, id, count, "seed", new PurchaseWithMarket(sku, price));
		}
	}

}
