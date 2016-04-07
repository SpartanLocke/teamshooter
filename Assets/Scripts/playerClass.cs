using UnityEngine;
using System.Collections;

public class playerClass : MonoBehaviour {
    public bool IS_NETWORK_CONTROLLED;

    public float playerSpeed; //speed player moves
    public float playerSpeedAlt; // speed player turns

    public int PlayerNumber;
    private string[,] axes = new string[,] { {"HorizontalP1", "HorizontalP2", "HorizontalP3", "HorizontalP4" },
        { "VerticalP1", "VerticalP2", "VerticalP3", "VerticalP4" },
        {"FireP1","FireP2", "FireP3", "FireP4" } };

    public float fireRate;
    public int numShots;
    public float offset;
    public float weight;
    public GameObject projectile;
    public Color normal;
    public Color fired;

    public GameObject grid;

    private Vector3 oldInput = new Vector3(0, 0);
    private Vector3 lastNetworkInputEvent = new Vector3(0,0);

    // setup our OnEvent as callback:
    void Awake() {
        PhotonNetwork.OnEventCall += this.OnPhotonNetworkEvent;
    }

    void Start() {
        gameObject.GetComponent<SpriteRenderer>().color = normal;
        paintUnderMe();
    }

    void Update() {
        //MoveForward(); // Player Movement
        //TurnRightAndLeft();//Player Turning
        //Vector3 AxisInput = (new Vector3(Input.GetAxis(axes[0, (PlayerNumber - 1)]), Input.GetAxis(axes[1, (PlayerNumber - 1)]))).normalized;
        Vector3 axisInput = getAxisInput();

        // don't log crap
        if (axisInput != new Vector3(0,0)) {
            Debug.Log(axisInput);
        }
        if (axisInput == new Vector3(0, 0)) {
            shoot(oldInput);
        } else {
            shoot(axisInput);
            oldInput = axisInput;
        }
        move(axisInput);
    }

    /**
        If the player is networked controlled, then the axis input is some form of whatever the network last communicated.

        If not networked controlled, then the axis input is "live" 
    **/
    private Vector3 getAxisInput() {
        Vector3 axisInput = new Vector3();
        if (IS_NETWORK_CONTROLLED) {
            // read from the last event
            axisInput = lastNetworkInputEvent;
        } else {
            // read from the standard axis stuff
            axisInput = (new Vector3(Input.GetAxis(axes[0, (PlayerNumber - 1)]), Input.GetAxis(axes[1, (PlayerNumber - 1)]))).normalized;
        }

        return axisInput;
    }

    // handle events
    private void OnPhotonNetworkEvent(byte eventcode, object content, int senderid) {
        // everything is in json format
        // encoded as class -> json -> base64 -> bytes
        // decoded as bytes -> base64 -> json -> class
        switch (eventcode) {
            // player input for 0
            case 0:
                PhotonPlayer sender = PhotonPlayer.Find(senderid);  // who sent this?
                byte[] byteContent = (byte[])content;
                string contentString = System.Convert.ToBase64String(byteContent);
                PlayerInputEvent playerInput = PlayerInputEvent.CreateFromJSON(contentString);

                // now we have what we need
                lastNetworkInputEvent = new Vector3(playerInput.x, playerInput.y);

                Debug.Log(lastNetworkInputEvent);
                break;
        }
    }

    private float nextFire = 0.0f;
    void shoot(Vector3 direction) {
        if (Input.GetButton(axes[2, (PlayerNumber - 1)]) && Time.time > nextFire) {
            nextFire = Time.time + fireRate;

            StartCoroutine(cooldownIndicator());
            StartCoroutine(fire(direction));
        }
    }

    IEnumerator cooldownIndicator() {
        gameObject.GetComponent<SpriteRenderer>().color = fired;
        yield return new WaitForSeconds(fireRate);
        gameObject.GetComponent<SpriteRenderer>().color = normal;
    }

    IEnumerator fire(Vector3 direction) {
        for (int i = 0; i < numShots; i++) {
            /*for (int j = 0; j < 1; j++)
            {
                yield return null;
            }*/
            Vector3 newPosition = transform.position - direction.normalized * offset + Quaternion.Euler(0, 0, -90) * (direction.normalized * i * weight);
            GameObject paint = Instantiate(projectile, newPosition, Quaternion.LookRotation(Vector3.forward, direction)) as GameObject;
            //paint.transform.parent = transform;
            paint.GetComponent<SpriteRenderer>().color = normal;
            paint.GetComponent<shotMovement>().grid = grid;
            Vector3 newPosition2 = transform.position - direction.normalized * offset + Quaternion.Euler(0, 0, -90) * (direction.normalized * -i * weight);
            GameObject paint2 = Instantiate(projectile, newPosition2, Quaternion.LookRotation(Vector3.forward, direction)) as GameObject;
            //paint2.transform.parent = transform;
            paint2.GetComponent<SpriteRenderer>().color = normal;
            paint2.GetComponent<shotMovement>().grid = grid;
            yield return null;

        }

    }

    void move(Vector3 direction) {

        if (isValidPosition(gameObject.transform.position + playerSpeed * direction * Time.deltaTime)) {
            transform.Translate(playerSpeed * direction * Time.deltaTime);
        }

    }
    void moveAlt() {
        Vector3 movement = new Vector3(0, 0);
        if (Input.GetKey("up"))//Press up arrow key to move forward on the Y AXIS
        {
            movement = movement + new Vector3(0, playerSpeed * Time.deltaTime);
        }
        if (Input.GetKey("down"))//Press up arrow key to move forward on the Y AXIS
        {
            movement = movement + new Vector3(0, -playerSpeed * Time.deltaTime);
        }
        if (Input.GetKey("right")) //Right arrow key to turn right
        {
            movement = movement + new Vector3(playerSpeed * Time.deltaTime, 0);
        }

        if (Input.GetKey("left"))//Left arrow key to turn left
        {
            movement = movement + new Vector3(-playerSpeed * Time.deltaTime, 0);
        }
        transform.Translate(playerSpeedAlt * movement * Time.deltaTime);

    }

    bool isValidPosition(Vector3 position) {

        gridController gridController = grid.GetComponent<gridController>();
        float gridSize = gridController.gridBlock.transform.localScale.x;
        if (normal == gridController.grid[Mathf.RoundToInt(position.x / gridSize), Mathf.RoundToInt(position.y / gridSize)].GetComponent<SpriteRenderer>().color) {
            return true;
        } else {
            return false;
        }
    }

    void paintUnderMe() {
        gridController gridController = grid.GetComponent<gridController>();
        float gridSize = gridController.gridBlock.transform.localScale.x;
        gridController.grid[Mathf.RoundToInt(transform.position.x / gridSize), Mathf.RoundToInt(transform.position.y / gridSize)].GetComponent<SpriteRenderer>().color = normal;
    }


    void OnCollisionEnter2D(Collision2D coll) {
        Debug.Log(coll);
        if (coll.gameObject.tag == "paint") {
            /*coll.gameObject.GetComponent<playerClass>().normal = normal;
            coll.gameObject.GetComponent<playerClass>().fired = fired;
            coll.gameObject.GetComponent<SpriteRenderer>().color = normal;*/

            normal = coll.gameObject.GetComponent<SpriteRenderer>().color;
            gameObject.GetComponent<SpriteRenderer>().color = normal;

        }
    }
}
