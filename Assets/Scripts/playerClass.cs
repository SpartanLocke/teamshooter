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
    public bool dodgeAbility;
    public bool dodging;
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
    public int colorNumber;
    public Color normal;
    public Color paintColor;
    public Color lightColor;
    public Color fired;
    public Light light;
    private float normalIntensity;
    ScoreManager scoreManager;
    private GameObject myProjectile;
    public GameObject grid;
    public int teamNum;
    private float gridSize;
    private gridController gridController;
    private SpriteRenderer spriteRenderer;

    private Vector3 oldInput = new Vector3(0, 0);
    private float nextTaunt = 0.0f;

    // network data
    private Vector3 lastNetworkInputLeftEvent = new Vector3(0,0);
    private Vector3 lastNetworkInputRightEvent = new Vector3(0, 0);

    private int networkPlayerId = -1;

    private Color lastNetworkColor = Color.clear;
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
        if (IS_LOCALLY_CONTROLLED)
        {
            colorNumber = PlayerNumber - 1;
        }
        setColor(colorNumber);
        paintUnderMe(1);
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

        if ((Input.GetButton(axes[2, (PlayerNumber - 1)]))) {
            taunt();
        }
    }

    void taunt()
    {
        if ( Time.time > nextTaunt && (myProjectile ==null)){
            //put the taunting action here
            nextTaunt = Time.time + tauntRate;
            if (dodgeAbility)
            {
                dodging = true;
            }
            StartCoroutine(tauntNumber(tauntNum));
        }
    }

    IEnumerator tauntNumber(int i)
    {
        for (int f = 0; f< 2; f++) {
            if (f == 0)
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
                                    light.color = lightColor;
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
                    for (int j = 0; j < 20; j++)
                    {
                        transform.Rotate(18f * Vector3.up);
                        yield return null;
                    }
                }
                else if (i == 3)
                {
                    for (int j = 0; j < 40; j++)
                    {
                        transform.Rotate(9f * Vector3.forward);
                        yield return null;
                    }
                }
            }
            else if (f == 1)
            {
                dodging = false;
            }
        }
        
    }

    void getInputs() {
		/*
        Vector3 AxisInput = (new Vector3(Input.GetAxis(axes[0, (PlayerNumber - 1)]), Input.GetAxis(axes[1, (PlayerNumber - 1)]))).normalized;
        Vector3 AxisInput2 = (new Vector3(Input.GetAxis(axes[3, (PlayerNumber - 1)]), Input.GetAxis(axes[4, (PlayerNumber - 1)]))).normalized;

        if (AxisInput2.magnitude > shootThreshold) {
            shoot(AxisInput2);
        }
        move(AxisInput);
        */
		if (m8s4) {
			Vector3 moveDir = Vector3.zero;
			if (Input.GetKey (KeyCode.UpArrow)) {
				moveDir += Vector3.up;
			}
			if (Input.GetKey (KeyCode.DownArrow)) {
				moveDir += Vector3.down;
			}
			if (Input.GetKey (KeyCode.RightArrow)) {
				moveDir += Vector3.right;
			}
			if (Input.GetKey (KeyCode.LeftArrow)) {
				moveDir += Vector3.left;
			}
			move (moveDir.normalized);
			if (Input.GetKeyDown ("w")) {
				shoot (Vector3.up);
			}
			if (Input.GetKeyDown ("s")) {
				shoot (Vector3.down);
			}
			if (Input.GetKeyDown ("d")) {
				shoot (Vector2.right);
			}
			if (Input.GetKeyDown ("a")) {
				shoot (Vector2.left);
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

        if (fireButton &&  myProjectile == null && !dodging)
        {    //StartCoroutine(cooldownIndicator());
            //StartCoroutine(fire(direction));
            StartCoroutine(fireAnimation());
            paintUnderMe(3);
            GameObject paint = Instantiate(projectileParent, transform.position + direction.normalized*offset, Quaternion.LookRotation(Vector3.forward, direction)) as GameObject;
            projectileParent parent = paint.GetComponent<projectileParent>();
            myProjectile = paint;
            parent.myColor = paintColor;
            parent.grid = grid;
            parent.teamNum = teamNum;
            parent.playerNumber = PlayerNumber;
            parent.colorNumber = colorNumber;
        }
        
    }

    IEnumerator fireAnimation()
    {
        for(int i = 0; i < 30; i++)
        {
            if (i < 10)
            {
                light.range += .4f;
            }
            else
            {
                light.range -= .2f;
            }
            yield return null;
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

			if (paintColor == gridController.getGridColor (gridX, gridY)) {
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

			if (paintColor == gridController.getGridColor (gridLeft, gridY) && paintColor == gridController.getGridColor (gridRight, gridY) &&
			    paintColor == gridController.getGridColor (gridX, gridUp) && paintColor == gridController.getGridColor (gridX, gridDown)) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
    }

    void setColor(int i)
    {
        Constants constants = grid.GetComponent<Constants>();
        normal = constants.playerColorChoices[i];
        spriteRenderer.color = normal;
        fired = constants.firedColors[i];
        lightColor = constants.lightColors[i];
        light.color = lightColor;
        paintColor = constants.paintColors[i];

    }

    void paintUnderMe(int size)
    {
        int x = Mathf.RoundToInt(transform.position.x / gridSize);
        int y = Mathf.RoundToInt(transform.position.y / gridSize);
        for(int i = 0; i < 2*size; i++)
        {
            for( int j = 0; j < 2*size; j++)
            {
                //gridController.grid[x + i -size, y + j - size].GetComponent<SpriteRenderer>().color = normal;
                gridController.setGridBlockToColor(x + i - size, y + j - size, paintColor);
            }
        }
    }


    void OnCollisionEnter2D(Collision2D coll) {
        //Debug.Log(coll);
		if (coll.gameObject.tag == "paint" && coll.gameObject.GetComponent<SpriteRenderer>().color != paintColor && !dodging)
        {
            setColor(coll.gameObject.GetComponent<shotMovement>().colorNumber);
            teamNum = coll.gameObject.GetComponent<shotMovement>().teamNum;
            GameObject hitIndicator = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
            hitIndicator.GetComponent<ParticleSystem>().startColor = paintColor;

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

        PhotonPlayer sender = PhotonPlayer.Find(senderid);  // who sent this?
        if (sender.ID != networkPlayerId) {
            // not input from our player!
            return;
        }

        switch (eventcode) {
            // player input for 0
            case Constants.PLAYER_INPUT_EVENT_CODE:
                byte[] byteContent = (byte[])content;
                string contentStringJson = Encoding.UTF8.GetString(byteContent);
                PlayerInputEvent playerInput = PlayerInputEvent.CreateFromJSON(contentStringJson);

                // now we have what we need
                lastNetworkInputLeftEvent = new Vector3(playerInput.left_x, playerInput.left_y);
                lastNetworkInputRightEvent = new Vector3(playerInput.right_x, playerInput.right_y);

                //Debug.Log(lastNetworkInputEvent);
                break;

            case Constants.PLAYER_TAUNT_EVENT_CODE:
                // do the taunt here
                Debug.Log("performing taunt from network call");
                taunt();
                break;
        }
    }

    // If the player's color changed, then send that as a network event.
    private void checkForPlayerColorChange() {
        if (spriteRenderer.color != lastNetworkColor) {
            Debug.Log("player color changed! sending as event");
            lastNetworkColor = spriteRenderer.color;

            controllerColorChangeEvent colorEvent = new controllerColorChangeEvent(lastNetworkColor, getNetworkPlayerId());

            // color change
            byte[] content = colorEvent.getBytes();

            sendNetworkEvent(Constants.PLAYER_COLOR_CHANGE_EVENT_CODE, content);
        }
    }

    private void sendNetworkEvent(byte eventCode, byte[] content) {
        // todo: is false the best here? (its the fastest)
        bool reliable = false;

        // todo: use RaiseEventOptions?
        PhotonNetwork.RaiseEvent(eventCode, content, reliable, null);
    }
}
