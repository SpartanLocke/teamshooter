using UnityEngine;
using System.Collections;

public class colorSelectionIndicator : MonoBehaviour {

	// Use this for initialization
 public void moveto(GameObject reference)
    {
        transform.position = reference.transform.position;
    }
}
