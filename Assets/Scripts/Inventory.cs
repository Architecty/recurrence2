using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
	public int maxItems = 4;
	public List<GameObject> items;
	public GameObject panel;

	private static Inventory instance = null;
	public Image FaderImage = null;

	public static Inventory Instance()
	{
		return instance;
	}

	public void Awake()
	{
		if ((instance != null) && (instance != this)) {
			Destroy(gameObject);
		} else {
			instance = this;
			//DontDestroyOnLoad(gameObject);
		}
	}

	public static Item ItemFromGameObject(GameObject obj)
	{
		if (obj == null)
			return null;
		Item item = null;	
		foreach (MonoBehaviour scr in obj.GetComponentsInChildren<MonoBehaviour>()) {
			item = scr as Item;
			if (item != null)
				break;
		}
		return item;
	}

	public int AddItem(GameObject obj)
	{	
		Item item = ItemFromGameObject(obj);
		if (item == null) {
			Debug.Log("No Item found in " + obj.name);
			return items.Count;
		}
		if (items.Count >= maxItems) {
			DropItem(1);
		}
		items.Add(obj);
		obj.SetActive(false);
		updateButtons();
		return items.Count;
	}
		
	public void DropItem(int i)
	{
		if ((i<0) || (i>=items.Count))
			return;
		GameObject player = Player.GetPlayer().gameObject;
		GameObject obj = items[i];
		items.RemoveAt(i);
		obj.gameObject.SetActive(true);
		obj.gameObject.transform.position = player.transform.position + player.transform.TransformDirection(Vector3.forward);
		updateButtons();
	}

	public Item GetItem(int i)
	{
		if ((i<0) || (i>=items.Count))
			return null;
		GameObject obj = items[i];

		return ItemFromGameObject(obj);
	}

	// Use only for replacing item
	public void SetItem(int i, GameObject item)
	{
		items[i] = item;
		updateButtons();
	}

	public void RemoveItem(GameObject item)
	{
		items.Remove(item);
		updateButtons();
	}

	public bool UseItem(int i)
	{
		Item item = GetItem(i);
		if (item == null) {
			return false;
		}
		return item.use(i);
	}

	public void updateButtons()
	{
		int i = 0;
		foreach (MonoBehaviour scr in panel.gameObject.GetComponentsInChildren<MonoBehaviour>()) {
			Image im = scr as Image;
			if (im == null)
				continue;
			// Hack to make sure we don't grab Button outline images
			if (!im.preserveAspect)
				continue;
			Item item = null;
			if ((i < items.Count) && ((item = ItemFromGameObject(items[i])) != null))
				im.sprite = item.icon;
			else
				im.sprite = null;
			i++;
		}
	}

	private float fadespeed = 0.5F;
	private Color FadeColor;

	public void Fade(bool up, float speed = 0.5F)
	{
		if (FaderImage == null)
			return;
		FadeColor = up ? Color.clear : Color.black;
//		fadespeed = Mathf.Abs(speed);
//		if (up)
//			fadespeed = -fadespeed;
		FaderImage.color = Color.Lerp(FaderImage.color, FadeColor, 0.02F);
	}

	// Use this for initialization
	void Start () {
		updateButtons();
	}
	
	// Update is called once per frame
	void Update () {
		if ((FaderImage.color.a >= 0.01) && (FaderImage.color.a <= 0.99))
			FaderImage.color = Color.Lerp(FaderImage.color, FadeColor, fadespeed * Time.deltaTime);
	}
}
