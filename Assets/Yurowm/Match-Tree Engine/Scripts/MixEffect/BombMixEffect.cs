using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Chip))]
public class BombMixEffect : MonoBehaviour {

	static Dictionary<BombPair, string> data = new Dictionary<BombPair, string>();
	private Chip chip;
	[HideInInspector]
	public bool destroingLock = true;

	public static void AddBombMixEffect(BombPair pair, string effect) {
		if (!data.ContainsKey(pair))
			data.Add(pair, effect);
	}

	public static void Initialize() {
		data.Add (new BombPair ("CrossBomb", "CrossBomb"), "CrossCrossMixEffect");
		data.Add (new BombPair ("SimpleBomb", "SimpleBomb"), "SimpleSimpleMixEffect");
		data.Add (new BombPair ("ColorBomb", "ColorBomb"), "ColorColorMixEffect");
		data.Add (new BombPair ("CrossBomb", "SimpleBomb"), "SimpleCrossMixEffect");
		data.Add (new BombPair ("CrossBomb", "ColorBomb"), "CrossColorMixEffect");
		data.Add (new BombPair ("SimpleBomb", "ColorBomb"), "SimpleColorMixEffect");
	}

	public static bool ContainsPair(string pa, string pb) {
		return ContainsPair (new BombPair (pa, pb));
	}

	public static bool ContainsPair(BombPair pair) {
		return data.ContainsKey (pair);
	}

	public static void Mix(string pa, string pb, SlotForChip slot) {
		Mix (new BombPair (pa, pb), slot);
	}

	public static void Mix(BombPair pair, SlotForChip slot) {
		if (!ContainsPair(pair)) return;
		BombMixEffect effect = ContentAssistant.main.GetItem<BombMixEffect> (data [pair]);
		slot.SetChip (effect.GetChip ());
	}

	public Chip GetChip() {
		if (chip == null)
			chip =GetComponent<Chip>();
		return chip;
	}

	public void DestroyChipFunction (){
		if (destroingLock) return;
		GetChip ().ParentRemove ();
		Destroy(gameObject);
	}
}

public class BombPair {
	public string a;
	public string b;

	public BombPair(string pa, string pb) {
		a = pa;
		b = pb;
	}

	public override bool Equals (object obj) {
		return CompareTo(obj as BombPair);
	}

	public override int GetHashCode()
	{
		return a.GetHashCode() + b.GetHashCode();
	}

	public bool CompareTo(BombPair pair) {
		bool result = false;
		result = result || (pair.a == a && pair.b == b);
		result = result || (pair.a == b && pair.b == a);
		return result;
	}

	public bool CompareTo(string pa, string pb) {
		bool result = false;
		result = result || (pa == a && pb == b);
		result = result || (pa == b && pb == a);
		return result;
	}
}
