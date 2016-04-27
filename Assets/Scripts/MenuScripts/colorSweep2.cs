using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.UI;

public class colorSweep2 : MonoBehaviour
{
    public bool isSweeping;
    public HSBColor col;
    public Image image;
    public float h = 0f;
    public GameObject[,] grid;
    public int input;
    public Canvas UI;
    // Use this for initialization
    void Start()
    {
        image = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isSweeping)
        {
            h += .005f;
        }
        else
        {
            h += .007f;
        }
        if (h >= 1)
        {
            h = 0;
        }
        if (isSweeping)
        {
            image.color = HSBColor.ToColor(new HSBColor(h, 1f, 1f, 1f));
        }
    }

    public void gridSweep(int num)
    {
        input = num;
        h = 0;
        grid = gameObject.GetComponent<gridController>().grid;
        Debug.Log("gridSweep");
        StartCoroutine(sweepGrid());
    }

    IEnumerator sweepGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j].GetComponent<SpriteRenderer>().color = HSBColor.ToColor(new HSBColor(h, 1f, 1f, 1f));
                

            }
            yield return null;
        }
        StartCoroutine(sweepGrid2());
    }

    IEnumerator sweepGrid2()
    {
        for (int i = 0; i < grid.GetLength(0)-1; i+=2)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                
                grid[i, j].GetComponent<SpriteRenderer>().color = HSBColor.ToColor(new HSBColor(0, 0f, 0f, 1f));
                grid[i+1, j].GetComponent<SpriteRenderer>().color = HSBColor.ToColor(new HSBColor(0, 0f, 0f, 1f));

            }
            yield return null;
        }
        if (input == 1)
        {
            UI.GetComponent<MenuUiController>().onStartAsServerButtonPressed();
        }
        else if(input == 0)
        {
            UI.GetComponent<MenuUiController>().onJoinButtonPressed();
        }
    }

}
