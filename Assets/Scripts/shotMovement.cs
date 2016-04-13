using UnityEngine;
using System.Collections;

public class shotMovement : MonoBehaviour {
    public bool pinned;
    public float shotSpeed;
    public float distance;
    public float shotWidth;
    public GameObject grid;
    private float moveSpeed;

    public int playerNumber;

    private SpriteRenderer mySpriteRenderer;

    void Awake() {
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (pinned)
        {
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
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
        Color myColor = mySpriteRenderer.color;

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
        if (coll.gameObject.tag == "wall" ) {
            Destroy(gameObject);
        }
        else if (coll.gameObject.tag == "paint" && coll.gameObject.GetComponent<SpriteRenderer>().color != mySpriteRenderer.color)
        {
            Destroy(gameObject);
        }
    }
}
