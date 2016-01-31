using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour {
	public enum State {
		None,
		Falling,
		Flying,
		Tested,
		Chased,
		Dead
	};
	public State state;
	public RigidbodyFirstPersonController FPController;
	public Rigidbody rbody;

	private GameObject usingItem = null;
	public GameObject parachute;
	public GameObject paperplane;

	public float groundlevel = 0F;
	public float fallspeed = 8F;
	public float camturnspeed = 1F;

	private Vector3 faceforward;

	public static Player GetPlayer()
	{
		return Camera.main.gameObject.GetComponentInChildren<Player>();
	}

	// Use this for initialization
	void Start () {
		if ((rbody == null)&&(FPController != null)) {
			rbody = FPController.gameObject.GetComponent<Rigidbody>();
		}
	}

	public bool useItem(Item item, int idx)
	{
		if ((state == State.Falling) && (item.type == Item.Type.Backpack)) {
			FPController.enabled = false;
			usingItem = GameObject.Instantiate(parachute, transform.position + Vector3.up,
				Quaternion.Euler(transform.eulerAngles)) as GameObject;
			Inventory.Instance().SetItem(idx, usingItem);
			state = State.Flying;
		}
		Inventory.Instance().DropItem(idx);
		return false;
	}

	void updateState()
	{
		switch(state) {
		case State.None:
			// Debug.Log("vert speed: " + rbody.velocity.y);
			if (rbody.velocity.y < -fallspeed) {
				state = State.Falling;
				faceforward = transform.forward;
				FPController.enabled = false;
			}
			break;
		case State.Falling:
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.down,
				faceforward), Time.deltaTime * camturnspeed);
			if (transform.position.y < (groundlevel + 3F)) {
				Debug.Log("Died");
				state = State.Dead;
				Inventory.Instance().Fade(false);
				FPController.enabled = true;
			}
			break;
		case State.Flying:
			rbody.transform.position = usingItem.transform.position + Vector3.down;
			transform.rotation = Quaternion.Slerp(transform.rotation, usingItem.transform.rotation, Time.deltaTime * camturnspeed);
			if (transform.position.y < (groundlevel + 1F)) {
				state = State.None;
				FPController.enabled = true;
				Inventory.Instance().RemoveItem(usingItem);
				Destroy(usingItem);
			}
			break;
		default:
			break;
		}
	}

	public Item itemInFront()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 3.0F)) {
			return Inventory.ItemFromGameObject(hit.collider.gameObject);
		}
		return null;
	}
	
	// Update is called once per frame
	void Update () {
		foreach (char c in Input.inputString) {
			if ((c >= '1') && (c <= '6')) {
				Item item = Inventory.Instance().GetItem(c - '1');
				if (item != null) {
					useItem(item, c - '1');
				}
			}
			// Just for testing
			if (c == 'p')
				Inventory.Instance().Fade(false);
			if (c == 'o')
				Inventory.Instance().Fade(true);
		}
		if (Input.GetMouseButtonDown(0)) {
			Item item = itemInFront();
			if (item != null)
				Inventory.Instance().AddItem(item.gameObject);
		}
		updateState();
	}
}
