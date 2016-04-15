using UnityEngine;
using System.Collections;

public class projectileParent : MonoBehaviour {
    public GameObject paint;
    public Vector3[] positionArray;
    public Color myColor;
    public GameObject grid;
    public GameObject explosion;
    // Use this for initialization
    void Start () {
        for (int i = 0; i < positionArray.Length; i++)
        {
            GameObject newPaint = Instantiate(paint, transform.rotation* positionArray[i] + transform.position, transform.rotation) as GameObject;
            newPaint.transform.parent = transform;
            newPaint.GetComponent<SpriteRenderer>().color = myColor;
            newPaint.GetComponent<shotMovement>().grid = grid;

        }
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
        }
	}
}
