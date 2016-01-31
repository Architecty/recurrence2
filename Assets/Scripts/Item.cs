using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
	public Sprite icon;
	public enum Type {
		Pencil,
		Ruler,
		Parachute,
		PaperPlane,
		Wallet,
		Coffee,
		Test,
		Backpack,
		Box
	}
	public Type type;

	public bool use(int idx)
	{
		return Player.GetPlayer().useItem(this, idx);
	}
}
