using UnityEngine;
using System.Collections;

public class HideButtonIfOnMobileScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    if (Application.isMobilePlatform) {
            // hide the parent button
            gameObject.SetActive(false);
        }
	}
}
