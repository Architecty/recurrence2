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

	// 0 = windnoise, 1 = parachute, 2 = scribbling, 3 = folding
	public AudioClip[] sounds;
	public bool[] soundloops;

	private Vector3 faceforward;
	private float deathtime;

	public static Player GetPlayer()
	{
		return Camera.main.gameObject.GetComponentInChildren<Player>();
	}

	// Use this for initialization
	void Start () {
		if ((rbody == null)&&(FPController != null)) {
			rbody = FPController.gameObject.GetComponent<Rigidbody>();
		}
		Inventory.Instance().Fade(true);
	}

	public bool useItem(Item item, int idx)
	{
		if ((state == State.Falling) && (item.type == Item.Type.Backpack)) {
			Debug.Log("Flying");
			FPController.enabled = true;
			usingItem = GameObject.Instantiate(parachute, transform.position,
				Quaternion.LookRotation(faceforward)) as GameObject;
			Inventory.Instance().SetItem(idx, usingItem);
			state = State.Flying;
			playaudio(1);
			return true;
		}
		if (item.type == Item.Type.Ruler) {
			Item it2 = itemInFront();
			if (it2.type == Item.Type.Test) {
				playaudio(3);
				GameObject.Instantiate(paperplane, it2.transform.position, it2.transform.rotation);
				Destroy(it2.gameObject);
				return true;
			}
		}
		if (item.type == Item.Type.Pencil) {
			Item it2 = itemInFront();
			if (it2.type == Item.Type.Test) {
				// start scribble noise
				state = State.Dead;
				Inventory.Instance().Fade(false);
				playaudio(2);
				return true;
			}
		}
		Inventory.Instance().DropItem(idx);
		return false;
	}

	// clipidx -1 mean stop playing
	void playaudio(int clipidx)
	{
		AudioSource audio = GetComponent<AudioSource>();
		if (clipidx < 0) {
			audio.Stop();
			return;
		}
		audio.clip = sounds[clipidx];
		audio.loop = soundloops[clipidx];
		audio.Play();
	}

	void updateState()
	{
		switch(state) {
		case State.None:
			// Debug.Log("vert speed: " + rbody.velocity.y);
			if (rbody.velocity.y < -fallspeed) {
				state = State.Falling;
				faceforward = Vector3.Cross(transform.right, Vector3.up);
				FPController.enabled = false;
				playaudio(0);
			}
			break;
		case State.Falling:
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.down,
				faceforward), Time.deltaTime * camturnspeed);
			if (transform.position.y < (groundlevel + 3F)) {
				Die();
			}
			break;
		case State.Flying:
			rbody.transform.position = usingItem.transform.position;
			transform.rotation = Quaternion.Slerp(transform.rotation, usingItem.transform.rotation, Time.deltaTime * camturnspeed);
			if (transform.position.y < (groundlevel + 1F)) {
				Debug.Log("Landed");
				state = State.None;
				FPController.enabled = true;
				Inventory.Instance().RemoveItem(usingItem);
				Destroy(usingItem);
			}
			break;
		case State.Dead:
			if (Time.time - deathtime > 5F)
				ToEnd();
			break;
		default:
			break;
		}
	}

	public Item itemInFront()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 3.0F)) {
			Debug.Log("Ray hit " + hit.collider.gameObject.name);
			return Inventory.ItemFromGameObject(hit.collider.gameObject);
		}
		return null;
	}

	public void Die()
	{
		Debug.Log("Died");
		playaudio(-1);
		state = State.Dead;
		Inventory.Instance().Fade(false);
		FPController.enabled = true;
		deathtime = Time.time;
	}

	public void ToEnd()
	{
		state = State.None;
		Debug.Log("The End ...?");
		tileManager tm = tileManager.Instance();
		tm.failedLevel();
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
