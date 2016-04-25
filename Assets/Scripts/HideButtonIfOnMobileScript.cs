using UnityEngine;
using System.Collections;

public class HideButtonIfOnMobileScript : MonoBehaviour {
    public bool serverStart;
    public bool controllerStart;
	// Use this for initialization
	void Start () {
	    if (Application.isMobilePlatform && serverStart) {
            // hide the parent button
            gameObject.SetActive(false);
        }
        else if((Application.isMobilePlatform && controllerStart)){
            float y = transform.position.y;
            transform.position = new Vector3(0.0f, y);

        }
	}
}
