using UnityEngine;
using System.Collections;

public class gridController : MonoBehaviour {
    public GameObject gridBlock;
    public float width;
    public float height;
    public float zpos;
    // todo: make this private
    public GameObject[,] grid;

    void Awake() {
        //Mathf.RoundToInt(Screen.width / (gridBlock.transform.localScale.x*50)), Mathf.RoundToInt(Screen.height / (gridBlock.transform.localScale.y * 50))
        grid = new GameObject[Mathf.RoundToInt(width / (gridBlock.transform.localScale.x)), Mathf.RoundToInt(height / (gridBlock.transform.localScale.y))];
        createGrid();
    }

    public Color getGridColor(int x, int y) {
        if (!inGridBounds(x, y)) {
            Debug.Log("got grid color clear for out of bounds grid");
            return Color.clear;
        }

        return grid[x, y].GetComponent<SpriteRenderer>().color;
    }

    public void setGridBlockToColor(int x, int y, Color color) {
        if (!inGridBounds(x, y)) {
            // Debug.Log(x + " " + y);
            return;
        }

        grid[x, y].GetComponent<SpriteRenderer>().color = color;
    }

    public bool inGridBounds(int x, int y) {
        if (x < 0 || x >= grid.GetLength(0)) {
            return false;
        }

        if (y < 0 || y >= grid.GetLength(1)) {
            return false;
        }

        return true;
    }

    void createGrid() {
        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                Vector3 position = new Vector3((i * gridBlock.transform.localScale.x), (j * gridBlock.transform.localScale.x), zpos);
                GameObject newBlock = Instantiate(gridBlock, position, Quaternion.identity) as GameObject;
                grid[i, j] = newBlock;
                newBlock.transform.parent = transform;
            }
        }
    }
    public void resetGrid()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
        }
    }
}
