using UnityEngine;

public class alwaysFace : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        Vector3 targetPostition = new Vector3(Camera.main.transform.position.x,
                                        this.transform.position.y,
                                        Camera.main.transform.position.z);
        this.transform.LookAt(targetPostition);
    }
}
