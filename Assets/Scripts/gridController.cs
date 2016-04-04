using UnityEngine;
using System.Collections;

public class gridController : MonoBehaviour {
    public GameObject gridBlock;
    public float width;
    public float height;
    public GameObject[,] grid;
	
	void Awake () {
        //Mathf.RoundToInt(Screen.width / (gridBlock.transform.localScale.x*50)), Mathf.RoundToInt(Screen.height / (gridBlock.transform.localScale.y * 50))
        grid = new GameObject[Mathf.RoundToInt(width / (gridBlock.transform.localScale.x)), Mathf.RoundToInt(height / (gridBlock.transform.localScale.y))];
        createGrid();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void createGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Vector3 position = new Vector3((i*gridBlock.transform.localScale.x), (j * gridBlock.transform.localScale.x));
                GameObject newBlock = Instantiate(gridBlock, position, Quaternion.identity) as GameObject;
                grid[i, j] = newBlock;
                newBlock.transform.parent = transform;
            }
        }
    }
}
