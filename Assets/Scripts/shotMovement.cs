using UnityEngine;
using System.Collections;

public class shotMovement : MonoBehaviour {

    public float shotSpeed;
    public float distance;
    public float shotWidth;
    public GameObject grid;
    private float moveSpeed;
    private SpriteRenderer spriteRenderer;

    void Awake() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start() {
        StartCoroutine(timer());
    }

    // Update is called once per frame
    void Update() {
        transform.Translate(Vector3.up * Time.deltaTime * shotSpeed);
        paintUnderMe();
    }

    void paintUnderMe() {
        gridController gridController = grid.GetComponent<gridController>();
        float gridSize = gridController.gridBlock.transform.localScale.x;
        Color myColor = spriteRenderer.color;

        if (gridController.inGridBounds(Mathf.RoundToInt(transform.position.x / gridSize), Mathf.RoundToInt(transform.position.y / gridSize))) {
            //gridController.grid[Mathf.RoundToInt(transform.position.x / gridSize), Mathf.RoundToInt(transform.position.y / gridSize)].GetComponent<SpriteRenderer>().color = gameObject.GetComponent<SpriteRenderer>().color;
            //gridController.grid[Mathf.RoundToInt((transform.position.x + shotWidth / 2) / gridSize), Mathf.RoundToInt((transform.position.y + shotWidth / 2) / gridSize)].GetComponent<SpriteRenderer>().color = gameObject.GetComponent<SpriteRenderer>().color;
            //gridController.grid[Mathf.RoundToInt((transform.position.x - shotWidth / 2) / gridSize), Mathf.RoundToInt((transform.position.y - shotWidth / 2) / gridSize)].GetComponent<SpriteRenderer>().color = gameObject.GetComponent<SpriteRenderer>().color;

            gridController.setGridBlockToColor(Mathf.RoundToInt(transform.position.x / gridSize), Mathf.RoundToInt(transform.position.y / gridSize), myColor);
            gridController.setGridBlockToColor(Mathf.RoundToInt((transform.position.x + shotWidth / 2) / gridSize), Mathf.RoundToInt((transform.position.y + shotWidth / 2) / gridSize), myColor);
            gridController.setGridBlockToColor(Mathf.RoundToInt((transform.position.x - shotWidth / 2) / gridSize), Mathf.RoundToInt((transform.position.y - shotWidth / 2) / gridSize), myColor);
        }
    }
    IEnumerator timer() {
        yield return new WaitForSeconds(distance / shotSpeed);
        Destroy(gameObject);
    }


    void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "wall") {
            Destroy(gameObject);
        }
    }
}
