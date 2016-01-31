using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;

public class introAnimation : MonoBehaviour
{
    public GameObject Animation1;
    public GameObject Animation2;
    public GameObject Animation3;

    public AutoMoveAndRotate moveScript;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(runAnimation());
    }

    IEnumerator runAnimation()
    {
        yield return new WaitForSeconds(1);
        Animation1.SetActive(true);

        yield return new WaitForSeconds(3);
        Animation1.SetActive(false);
        Animation2.SetActive(true);
        yield return new WaitForSeconds(5);
        Animation2.SetActive(false);
        yield return new WaitForSeconds(1);
        Application.LoadLevel("Main Game");
    }
}
