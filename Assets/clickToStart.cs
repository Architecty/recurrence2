using UnityEngine;
using System.Collections;

public class clickToStart : MonoBehaviour {

    public Material hoverMaterial;
    public Material notHoverMaterial;
    public Transform redButton;

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void OnMouseDown()
    {
        int i = Application.loadedLevel;
        Application.LoadLevel(i + 1);
    }
    void OnMouseEnter()
    {
        redButton.GetComponent<Renderer>().material = hoverMaterial;
    }
    void OnMouseExit()
    {
        redButton.GetComponent<Renderer>().material = notHoverMaterial;
    }
}
