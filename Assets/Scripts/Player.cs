using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		foreach (char c in Input.inputString) {
			if ((c >= '1') && (c <= '6')) {
				Inventory.Instance().DropItem(c - '1');
			}
		}
		if (Input.GetMouseButtonDown(0)) {
			Debug.Log("Mouse pressed");
			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 3.0F)) {
				Debug.Log("Found object: " + hit.collider.gameObject.name);
				Item item = Inventory.ItemFromGameObject(hit.collider.gameObject);
				if (item != null) {
					Inventory.Instance().AddItem(item.gameObject);
				}
			}
		}
	}
}
