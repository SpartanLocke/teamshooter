using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class reset : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void resetActive()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
