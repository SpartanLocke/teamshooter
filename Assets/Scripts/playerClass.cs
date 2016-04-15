using UnityEngine;
using System.Collections;
using System.Text;

public class playerClass : MonoBehaviour {
    public bool IS_LOCALLY_CONTROLLED;
    public int tauntNum;
    public float playerSpeed; //speed player moves
    public float playerSpeedAlt;
    public bool timeDelay = false;
    public float delayTime;

    public int PlayerNumber;

	//Debug tool for the type of movement
	//If movementType = "touchblock", a player only needs to be touching their color to move
	//If movementType = "immerse", a player has to be surrounded by their color to move
	public string movementType = "immerse";

    private string[,] axes = new string[,] { {"HorizontalP1", "HorizontalP2", "HorizontalP3", "HorizontalP4" }, { "VerticalP1", "VerticalP2", "VerticalP3", "VerticalP4" }, {"FireP1","FireP2", "FireP3", "FireP4" },
		{"HorizontalShootP1", "HorizontalShootP2", "HorizontalShootP3", "HorizontalShootP4" }, { "VerticalShootP1", "VerticalShootP2", "VerticalShootP3", "VerticalShootP4" }};

    public bool oneJoystick;
    public bool twoJoystick;
    public float shootThreshold;
    public float moveThreshold;
    public bool m8s4;
    public bool m8s8;
    public bool gridMovement;
    private Vector3 shootDir;
    private int frames;
    public int frameDelay;
    
    public float fireRate;
    public float tauntRate;
    public int numShots;
    public float offset;
    public float weight;
    public GameObject projectile;
    public GameObject projectileParent;
    public GameObject explosion;
    public Color normal;
    public Color fired;
    public Light light;
    private float normalIntensity;
    ScoreManager scoreManager;

    public GameObject grid;
    public int teamNum;
    private float gridSize;
    private gridController gridController;
    private SpriteRenderer spriteRenderer;

    private Vector3 oldInput = new Vector3(0, 0);
    private float nextFire = 0.0f;
    private float nextTaunt = 0.0f;

    // network data
    private Vector3 lastNetworkInputLeftEvent = new Vector3(0,0);
    private Vector3 lastNetworkInputRightEvent = new Vector3(0, 0);

    private int networkPlayerId = -1;

    // setup our OnEvent as callback:
    void Awake() {
        PhotonNetwork.OnEventCall += this.OnPhotonNetworkEvent;

        if (grid == null) {
            grid = GameObject.FindGameObjectWithTag("gridGameObject");
        }

        gridController = grid.GetComponent<gridController>();
        gridSize = gridController.gridBlock.transform.localScale.x;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        light = gameObject.GetComponentInChildren<Light>();
        normalIntensity = light.intensity;
    }

    void Start() {
        spriteRenderer.color = normal;
        light.color = normal;
        paintUnderMe();
		scoreManager = GameObject.FindObjectOfType<ScoreManager>();
    }

    void Update() {
        if (IS_LOCALLY_CONTROLLED) {
            doLocalUpdate();
        } else {
            doNetworkUpdate();
        }
    }

    /// <summary>
    ///  Using the dual joystick control scheme.
    /// </summary>
    private void doNetworkUpdate() {
        if (lastNetworkInputRightEvent.magnitude > shootThreshold) {
            shoot(lastNetworkInputRightEvent);
        }
        move(lastNetworkInputLeftEvent);
    }

    void doLocalUpdate() {
        getInputs();
        taunt();
    }

    void taunt()
    {
        if ( Time.time > nextTaunt && (Input.GetButton(axes[2,(PlayerNumber - 1)]))){
            //put the taunting action here
            nextTaunt = Time.time + tauntRate;
            Debug.Log("taunt");
            StartCoroutine(tauntNumber(tauntNum));
        }
    }
    IEnumerator tauntNumber(int i)
    {
        
        if (i == 0 || i == 1)
        {
            for (int k = 0; k < 4; k++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (k == 0 || k == 2)
                    {
                        if (i == 0)
                        {
                            light.color = Color.white;
                            yield return null;
                        }
                        else if (i == 1)
                        {
                            spriteRenderer.color = Color.white;
                            yield return null;
                        }
                    }
                    if (k == 1 || k == 3)
                    {
                        if (i == 0)
                        {
                            light.color = normal;
                            yield return null;
                        }
                        else if (i == 1)
                        {
                            spriteRenderer.color = normal;
                            yield return null;
                        }
                    }

                }
            }
        }
        else if (i == 2)
        {
            for(int j = 0; j < 20; j++)
            {
                transform.Rotate(18f*Vector3.up);
                yield return null;
            }
        }
    }

    void getInputs()
    {
        if (oneJoystick)
        {
            Vector3 AxisInput = (new Vector3(Input.GetAxis(axes[0, (PlayerNumber - 1)]), Input.GetAxis(axes[1, (PlayerNumber - 1)]))).normalized;

            if (AxisInput == new Vector3(0, 0))
            {
                shoot(oldInput);
            }
            else {
                shoot(AxisInput);
                oldInput = AxisInput;
            }
            move(AxisInput);
        }
        else if (gridMovement)
        {
            Vector3 AxisInput = (new Vector3(Input.GetAxis(axes[0, (PlayerNumber - 1)]), Input.GetAxis(axes[1, (PlayerNumber - 1)]))).normalized;
            Vector3 AxisInput2 = (new Vector3(Input.GetAxis(axes[3, (PlayerNumber - 1)]), Input.GetAxis(axes[4, (PlayerNumber - 1)]))).normalized;

            if (AxisInput2.magnitude > shootThreshold)
            {
                if (Mathf.Abs(AxisInput2.x) > Mathf.Abs(AxisInput2.y))
                {
                    AxisInput2.y = 0;
                    // Debug.Log("shoot horizontal");
                }
                else
                {
                    AxisInput2.x = 0;
                    //Debug.Log("shoot vertical");
                }
                shoot(AxisInput2.normalized);
            }
            if (AxisInput.magnitude > moveThreshold)
            {
                if (Mathf.Abs(AxisInput.x) > Mathf.Abs(AxisInput.y))
                {
                    AxisInput.y = 0;
                    //Debug.Log("move horizontal");
                }
                else
                {
                    AxisInput.x = 0;
                    //Debug.Log("move vertical");
                }
                Debug.Log(AxisInput.normalized);
                moveAlt(AxisInput.normalized);
            }


        }
        else if (twoJoystick)
        {
            Vector3 AxisInput = (new Vector3(Input.GetAxis(axes[0, (PlayerNumber - 1)]), Input.GetAxis(axes[1, (PlayerNumber - 1)]))).normalized;
            Vector3 AxisInput2 = (new Vector3(Input.GetAxis(axes[3, (PlayerNumber - 1)]), Input.GetAxis(axes[4, (PlayerNumber - 1)]))).normalized;

            if (AxisInput2.magnitude > shootThreshold)
            {
                shoot(AxisInput2);
            }
            move(AxisInput);

        }
        else if (m8s4)
        {
            Vector3 moveDir = Vector3.zero;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveDir += Vector3.up;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                moveDir += Vector3.down;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                moveDir += Vector3.right;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveDir += Vector3.left;
            }
            move(moveDir.normalized);
            if (Input.GetKeyDown("w"))
            {
                shoot(Vector3.up);
            }
            if (Input.GetKeyDown("s"))
            {
                shoot(Vector3.down);
            }
            if (Input.GetKeyDown("d"))
            {
                shoot(Vector2.right);
            }
            if (Input.GetKeyDown("a"))
            {
                shoot(Vector2.left);
            }
        }
        else if (m8s8)
        {
            Vector3 moveDir = Vector3.zero;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveDir += Vector3.up;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                moveDir += Vector3.down;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                moveDir += Vector3.right;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveDir += Vector3.left;
            }
            move(moveDir.normalized);

            //this isn't smash so people dont have to be frame perfect to shoot diagonally
            if (Input.GetKey("w"))
            {
                shootDir += Vector3.up;
            }
            if (Input.GetKey("s"))
            {
                shootDir += Vector3.down;
            }
            if (Input.GetKey("d"))
            {
                shootDir += Vector3.right;
            }
            if (Input.GetKey("a"))
            {
                shootDir += Vector3.left;
            }
            frames++;
            if (frames > frameDelay)
            {
                shoot(shootDir.normalized);
                shootDir = Vector3.zero;
                frames = 0;
            }

        }
    }
    void shoot(Vector3 direction) {
        bool fireButton;

        if (!IS_LOCALLY_CONTROLLED) {
            // read from the last event
            // twoJoystick is essentially true here, so just return true.
            // also, shoot is only called if the second stick is active
            fireButton = true;
        } else {
            // read from the standard axis stuff
            fireButton = (Input.GetButton(axes[2, (PlayerNumber - 1)]) || (m8s4 || m8s8 || twoJoystick || gridMovement));
        }

        if (fireButton && Time.time > nextFire) {
            nextFire = Time.time + fireRate;

            StartCoroutine(cooldownIndicator());
            //StartCoroutine(fire(direction));
            GameObject paint = Instantiate(projectileParent, transform.position , Quaternion.LookRotation(Vector3.forward, direction)) as GameObject;
            projectileParent parent = paint.GetComponent<projectileParent>();
            parent.myColor = normal;
            parent.grid = grid;
        }
        
    }

    IEnumerator cooldownIndicator() {
        spriteRenderer.color = fired;
        //transform.GetChild(0).gameObject.SetActive(false);
        //light.color = Color.white;
        //light.intensity = 1.25f;
        yield return new WaitForSeconds(fireRate);
        //light.intensity = normalIntensity;
        //light.color = normal;
        //transform.GetChild(0).gameObject.SetActive(true);
        spriteRenderer.color = normal;
    }

    /*IEnumerator fire(Vector3 direction) {
        for (int i = 0; i < numShots; i++) {
            /*for (int j = 0; j < 1; j++)
            {
                yield return null;
            }
            Vector3 newPosition = transform.position - direction.normalized * offset + Quaternion.Euler(0, 0, -90) * (direction.normalized* i * weight);
            GameObject paint = Instantiate(projectile, newPosition , Quaternion.LookRotation(Vector3.forward, direction)) as GameObject;
			var paintScript = paint.GetComponent<shotMovement>();
			paintScript.playerNumber = PlayerNumber;
            paintScript.teamNum = teamNum;
            //paint.transform.parent = transform;
            paint.GetComponent<SpriteRenderer>().color = normal;
            paint.GetComponent<shotMovement>().grid = grid;
            Vector3 newPosition2 = transform.position - direction.normalized * offset + Quaternion.Euler(0, 0, -90) * (direction.normalized * -i * weight);
            GameObject paint2 = Instantiate(projectile, newPosition2, Quaternion.LookRotation(Vector3.forward, direction)) as GameObject;
			var paintScript2 = paint2.GetComponent<shotMovement>();
			paintScript2.playerNumber = PlayerNumber;
            paintScript.teamNum = teamNum;
            //paint2.transform.parent = transform;
            paint2.GetComponent<SpriteRenderer>().color = normal;
            paint2.GetComponent<shotMovement>().grid = grid;
            yield return null;

        }

    }*/

    void move(Vector3 direction) {

        if (isValidPosition(gameObject.transform.position + playerSpeed * direction * Time.deltaTime)) {
            transform.Translate(playerSpeed * direction * Time.deltaTime);
        }

    }
    void moveAlt(Vector3 direction)
    {
        Vector3 newPos = gameObject.transform.position + playerSpeedAlt * direction * Time.deltaTime;
        if (isValidPosition(newPos) && !timeDelay)
        {
            Debug.Log("entered");
            timeDelay = true;
            StartCoroutine(timer(delayTime));
            transform.position = gridController.grid[Mathf.RoundToInt(newPos.x / gridSize), Mathf.RoundToInt(newPos.y / gridSize)].transform.position;
        }
    }
		
//    bool isValidPosition(Vector3 position) {
//        gridController gridController = grid.GetComponent<gridController>();
//        float gridSize = gridController.gridBlock.transform.localScale.x;
//		if (movementType.Equals ("touchblock")) {
//			if (normal == gridController.grid [Mathf.RoundToInt (position.x / gridSize), Mathf.RoundToInt (position.y / gridSize)].GetComponent<SpriteRenderer> ().color) {
//				return true;
//			} else {
//				return false;
//			}
//		} else if (movementType.Equals ("immerse")) {
//			float playerRadius = GetComponent<SpriteRenderer> ().bounds.size.x / 2;
//			if (normal == gridController.grid [Mathf.RoundToInt ((position.x + playerRadius) / gridSize), Mathf.RoundToInt (position.y / gridSize)].GetComponent<SpriteRenderer> ().color &&
//			    normal == gridController.grid [Mathf.RoundToInt ((position.x - playerRadius) / gridSize), Mathf.RoundToInt (position.y / gridSize)].GetComponent<SpriteRenderer> ().color &&
//			    normal == gridController.grid [Mathf.RoundToInt (position.x / gridSize), Mathf.RoundToInt ((position.y + playerRadius) / gridSize)].GetComponent<SpriteRenderer> ().color &&
//			    normal == gridController.grid [Mathf.RoundToInt (position.x / gridSize), Mathf.RoundToInt ((position.y - playerRadius) / gridSize)].GetComponent<SpriteRenderer> ().color) {
//				return true;
//			} else {
//				return false;
//			}
//		} else {
//			return false;
//		}
//	}

    IEnumerator timer(float time)
    {
        yield return new WaitForSeconds(time);
        timeDelay = false;
        Debug.Log("timer expired");
    }

    bool isValidPosition(Vector3 position) {
        float gridSize = gridController.gridBlock.transform.localScale.x;

		//sprite only has to touch color
		if (movementType.Equals ("touchblock")) {
			int gridX = Mathf.RoundToInt (position.x / gridSize);
			int gridY = Mathf.RoundToInt (position.y / gridSize);
			if (!gridController.inGridBounds (gridX, gridY)) {
				// out of bounds, then get outta here
				return false;
			}

			if (normal == gridController.getGridColor (gridX, gridY)) {
				return true;
			} else {
				return false;
			}
			//sprite has to be within color
		} else if (movementType.Equals ("immerse")) {
            int gridX = Mathf.RoundToInt(position.x / gridSize);
            int gridY = Mathf.RoundToInt(position.y / gridSize);
            float playerRadius = GetComponent<SpriteRenderer> ().bounds.size.x / 2;
			int gridLeft = Mathf.RoundToInt ((position.x + playerRadius) / gridSize);
			int gridRight = Mathf.RoundToInt ((position.x - playerRadius) / gridSize);
			int gridUp = Mathf.RoundToInt ((position.y + playerRadius) / gridSize);
			int gridDown = Mathf.RoundToInt ((position.y - playerRadius) / gridSize);
			if (!gridController.inGridBounds (gridLeft, gridY) || !gridController.inGridBounds (gridRight, gridY) ||
			    !gridController.inGridBounds (gridX, gridUp) || !gridController.inGridBounds (gridX, gridDown)) {
				return false;
			}

			if (normal == gridController.getGridColor (gridLeft, gridY) && normal == gridController.getGridColor (gridRight, gridY) &&
			    normal == gridController.getGridColor (gridX, gridUp) && normal == gridController.getGridColor (gridX, gridDown)) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
    }

    void paintUnderMe() {
        
        gridController.grid[Mathf.RoundToInt(transform.position.x / gridSize), Mathf.RoundToInt(transform.position.y / gridSize)].GetComponent<SpriteRenderer>().color = normal;
    }


    void OnCollisionEnter2D(Collision2D coll) {
        //Debug.Log(coll);
		if (coll.gameObject.tag == "paint" && coll.gameObject.GetComponent<SpriteRenderer>().color != normal)
        {
            /*coll.gameObject.GetComponent<playerClass>().normal = normal;
            coll.gameObject.GetComponent<playerClass>().fired = fired;
            coll.gameObject.GetComponent<SpriteRenderer>().color = normal;*/

            normal = coll.gameObject.GetComponent<SpriteRenderer>().color;
            spriteRenderer.color = normal;
            teamNum = coll.gameObject.GetComponent<shotMovement>().teamNum;
            GameObject hitIndicator = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
            hitIndicator.GetComponent<ParticleSystem>().startColor = normal;

            if (scoreManager != null) {
                scoreManager.ChangeScore(PlayerNumber.ToString(), "deaths", 1);
                scoreManager.ChangeScore(PlayerNumber.ToString(), "score", -1);
                scoreManager.changeColorCount(teamNum, PlayerNumber.ToString());
                int playerWhoShotMe = coll.gameObject.GetComponent<shotMovement>().playerNumber;
                scoreManager.ChangeScore(playerWhoShotMe.ToString(), "kills", 1);
                scoreManager.ChangeScore(playerWhoShotMe.ToString(), "score", 1);
            }
        }
    }

    public void setNetworkPlayerId(int id) {
        networkPlayerId = id;
    }

    public int getNetworkPlayerId() {
        return networkPlayerId;
    }

    // handle events
    private void OnPhotonNetworkEvent(byte eventcode, object content, int senderid) {
        // everything is in json format
        switch (eventcode) {
            // player input for 0
            case 0:
                PhotonPlayer sender = PhotonPlayer.Find(senderid);  // who sent this?

                if (sender.ID != networkPlayerId) {
                    // not input from our player!
                    break;
                }

                byte[] byteContent = (byte[])content;
                string contentStringJson = Encoding.UTF8.GetString(byteContent);
                PlayerInputEvent playerInput = PlayerInputEvent.CreateFromJSON(contentStringJson);

                // now we have what we need
                lastNetworkInputLeftEvent = new Vector3(playerInput.left_x, playerInput.left_y);
                lastNetworkInputRightEvent = new Vector3(playerInput.right_x, playerInput.right_y);

                //Debug.Log(lastNetworkInputEvent);
                break;
        }
    }
}
