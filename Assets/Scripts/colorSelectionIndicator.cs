using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class colorSelectionIndicator : MonoBehaviour {
    public float rad;
    public float falloff;
    void Start()
    {
        gameObject.GetComponent<Light>().color = Constants.paintColors[0];
    }
	// Use this for initialization
 public void moveto(GameObject reference)
    {
        transform.position = reference.transform.position + new Vector3(0,0,1f);
        gameObject.GetComponent<Light>().color = reference.GetComponent<Image>().color;
        StartCoroutine(shrink());
    }
    IEnumerator shrink()
    {
        for (int i = 0; i < 15; i++)
        {
            gameObject.GetComponent<Light>().range = (rad - i * falloff);
            yield return null;
        }
        
    }
}
