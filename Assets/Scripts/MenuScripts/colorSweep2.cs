using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.UI;

public class colorSweep2 : MonoBehaviour
{
    public bool blackSweeping = true;
    public int rowToSweep;
    public int bandWidth;
    public bool isRowSweeping;
    public bool isSweeping;
    public bool dontSweep = false;
    public HSBColor col;
    public GameObject imageObject;
    public Image image;
    public float h = 0f;
    public GameObject[,] grid;
    public int input;
    public Canvas UI;
    // Use this for initialization
    void Start()
    {
        image = imageObject.GetComponent<Image>();
        grid = gameObject.GetComponent<gridController>().grid;
        if (isRowSweeping)
        {
            int delay = 2;
            int rowToSweep2 = rowToSweep+1;
            for(int i = 0; i < 5; i++)
            {
                StartCoroutine(sweepRow(rowToSweep, i*delay));
                StartCoroutine(sweepRowBlack(rowToSweep, bandWidth, i*delay));
                StartCoroutine(sweepRow(rowToSweep2, i * delay));
                StartCoroutine(sweepRowBlack(rowToSweep2, bandWidth, i * delay));
                rowToSweep--;
                rowToSweep2++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if (isSweeping || isRowSweeping)
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
        blackSweeping = false;
        isRowSweeping = false;
        input = num;
        h = 0;
        grid = gameObject.GetComponent<gridController>().grid;
        Debug.Log("gridSweep");
        StartCoroutine(sweepGrid());
        StartCoroutine(loadNext());
    }
    IEnumerator loadNext()
    {
        if (input == 1)
        {
            yield return new WaitForSeconds(1.5f);
            UI.GetComponent<MenuUiController>().onStartAsServerButtonPressed();
        }
        else if (input == 0)
        {
            yield return null;
            UI.GetComponent<MenuUiController>().onJoinButtonPressed();
        }
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
        
        
        
    }
    IEnumerator sweepRow(int row,int delayFrames)
    {
        for (int i = 0; i < delayFrames; i++)
        {
            yield return null;
        }
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            grid[i, row].GetComponent<SpriteRenderer>().color = HSBColor.ToColor(new HSBColor(h, 1f, 1f, 1f));
            yield return null;
        }
        StartCoroutine(sweepRow(row,0));
        StartCoroutine(sweepRowBlack(row, bandWidth,0));
    }
    IEnumerator sweepRowBlack(int row, int width, int delayFrames)
    {
        for (int i = 0; i < delayFrames+width; i++)
        {
            yield return null;
        }
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            if (blackSweeping)
            {
                grid[i, row].GetComponent<SpriteRenderer>().color = HSBColor.ToColor(new HSBColor(h, 1f, 0f, 1f));
            }
            yield return null;
        }
        
    }
    public void blackSweep()
    {
        if (blackSweeping)
        {
            blackSweeping = false;
        }
        else
        {
            blackSweeping = true;
        }
    }
}
