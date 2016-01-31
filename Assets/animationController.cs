using UnityEngine;
using System.Collections;

public class animationController : MonoBehaviour {
    public GameObject Animation1;
    public GameObject Animation2;
    public GameObject Animation3;
    public GameObject Fader;
    public float timeToFade = .5f;

    // Use this for initialization
    void Start () {
        StartCoroutine(runAnimation());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator runAnimation()
    {
        yield return new WaitForSeconds(1);
        Animation1.SetActive(true);
        yield return new WaitForSeconds(3);
        fadeOut();
        yield return new WaitForSeconds(timeToFade);
        Animation1.SetActive(false);
        Animation2.SetActive(true);
        yield return new WaitForSeconds(3);
        fadeOut();
        Animation2.SetActive(false);
        Animation3.SetActive(true);
        yield return new WaitForSeconds(15);
        Animation3.SetActive(false);
        Application.LoadLevel("Main Game");
    }

    IEnumerator fadeOut()
    {
        Material fadeMaterial = Fader.GetComponent<Renderer>().material;
        float currentTime = Time.time;
        while(Time.time - currentTime < timeToFade)
        {
            Color currentColor = fadeMaterial.color;
            currentColor.a = (Time.time - currentTime / timeToFade);
            fadeMaterial.color = currentColor;
            yield return null;
        }
        Color thisColor = fadeMaterial.color;
        thisColor.a = 1;
        fadeMaterial.color = thisColor;
    }
}
