using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ColorSelectButton : MonoBehaviour {
    public int colorIndex;
	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Image>().color = Constants.paintColors[colorIndex];
	}
}
