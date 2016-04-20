using UnityEngine;
using System.Collections;

public class particleController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        unChild();
	}
	
	// Update is called once per frame
	void Update () {
        if (!gameObject.GetComponent<ParticleSystem>().IsAlive())
        {
            Destroy(gameObject);
        }
	}
    void unChild()
    {
        transform.parent = null;
    }
}
