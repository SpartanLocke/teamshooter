using UnityEngine;
using System.Collections;
using System.Text;
using System;

public class playerClass : MonoBehaviour {
    
    public bool IS_LOCALLY_CONTROLLED;
    public int PlayerNumber;
    public int tauntNum;
    public float playerSpeed; //speed player moves
    public float playerSpeedAlt;
    public bool timeDelay = false;
    public float delayTime;
    public bool dodgeAbility;
    public bool dodging;
    public AudioClip shootSound;
    public AudioClip convertSound;
    private AudioSource[] source;
    private AudioSource shootSource;
    private AudioSource convertSource;
    
	//Debug tool for the type of movement
	//If movementType = "touchblock", a player only needs to be touching their color to move
	//If movementType = "immerse", a player has to be surrounded by their color to move
	public string movementType = "immerse";
	public bool hitWall = false;

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
    public bool coolDown;
    private float nextFire=0.0f;
    public float tauntRate;
    public int numShots;
    public float offset;
    public float weight;
    public GameObject projectile;
    public GameObject projectileParent;
    public GameObject explosion;
    private GameObject fireDirectionIndicator;
    public int colorNumber;
    public int colorChoiceNumber;           //BE SURE TO SET THIS UPON INSTANTIATION OF NEW PLAYER
    public Color normal;
    public Color paintColor;
    public Color lightColor;
    public Color fired;
    public float colorShift;
	public Color originalPaintColor;
	public Color originalLightColor;
	public Color originalFiredColor;
    public Light light;
    private float normalIntensity;
    ScoreManager scoreManager;
    private GameObject myProjectile;
    public GameObject grid;
    public int teamNum;
    private float gridSize;
    private gridController gridController;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer indicatorRenderer;
    private Vector3 oldInput = new Vector3(0, 0);
    private float nextTaunt = 0.0f;
	private double speed = 1.0;
    public float slowFactor;

    // network data
    private Vector3 lastNetworkInputLeftEvent = new Vector3(0, 0);
    private Vector3 lastNetworkInputRightEvent = new Vector3(0, 0);

    private int networkPlayerId = -1;

    private bool hasReceievedNetworkInitData = false;

	private Vector3 originalPosition;
	private Vector3 scoreboardPosition;

    // setup our OnEvent as callback:
    void Awake() {
        PhotonNetwork.OnEventCall += this.OnPhotonNetworkEvent;

        source = gameObject.GetComponents<AudioSource>();
        shootSource = source[0];
        convertSource = source[1];

        if (grid == null) {
            grid = GameObject.FindGameObjectWithTag("gridGameObject");
        }

        gridController = grid.GetComponent<gridController>();
        gridSize = gridController.gridBlock.transform.localScale.x;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        light = gameObject.GetComponentInChildren<Light>();
        normalIntensity = light.intensity;
		originalPosition = new Vector3(this.transform.position.x, this.transform.position.y);
    }

    void OnDestroy() {
        Debug.Log("player was destroyed");
        PhotonNetwork.OnEventCall -= this.OnPhotonNetworkEvent;
    }

    void Start() {
        if (IS_LOCALLY_CONTROLLED)
        {
            colorNumber = PlayerNumber - 1;
            colorChoiceNumber = colorNumber;

            setColor(colorNumber);
            originalPaintColor = Constants.paintColors[colorNumber];
            originalLightColor = Constants.lightColors[colorNumber];
            originalFiredColor = Constants.firedColors[colorNumber];
            paintUnderMe(10);
        }
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();
		scoreboardPosition = new Vector3((float)(scoreManager.leftStart + scoreManager.spaceBetweenPlayers * (PlayerNumber - 1)), (float)1.5,(float)0);
        fireDirectionIndicator = transform.GetChild(1).gameObject;
        indicatorRenderer = fireDirectionIndicator.GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (grid == null) {
            grid = GameObject.FindGameObjectWithTag("gridGameObject");
        }

		switch (scoreManager.myGameState) 
		{
		    case ScoreManager.gameState.Gameplay:
			    if (IS_LOCALLY_CONTROLLED) {
				    doLocalUpdate ();
			    } else {
				    doNetworkUpdate (true);
			    }
			    break;
		    case ScoreManager.gameState.SetUpScoreboard:
			    setUpScoreboard ();
                doNetworkUpdate(false);
                break;
		    case ScoreManager.gameState.ExecuteScoreboard:
			    executeScoreboard ();
                doNetworkUpdate(false);
                break;
		    case ScoreManager.gameState.Reset:
			    playerReset ();
                doNetworkUpdate(false);
                break;
            case ScoreManager.gameState.InLobby:
                if (IS_LOCALLY_CONTROLLED)
                {
                    doLocalUpdate();
                }
                else {
                    doNetworkUpdate(false);
                }
                break;

		}
    }

	void playerReset () {
        // Debug.Log (Time.deltaTime);
        fireDirectionIndicator.SetActive(true);
        Vector3 targetPosition = scoreManager.currentLevel.transform.GetChild(1).GetChild(PlayerNumber-1).transform.position;
		double distToTarget = (transform.position - targetPosition).magnitude;
		if (distToTarget == 0) {
			paintUnderMe(10);
		}

		transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, (float)10.0 * Time.deltaTime); 
	}

	void setUpScoreboard () {
        // Debug.Log ("setupScoreboard called");
        // Debug.Log (Time.deltaTime);
        fireDirectionIndicator.SetActive(false);
		double distToTarget = (this.transform.position - scoreboardPosition).magnitude;

		this.transform.position = Vector3.MoveTowards(this.transform.position, scoreboardPosition, (float)10.0 * Time.deltaTime); 
	}

	// TODO if we want the player to move up with the score, and do separate shots for 'previous round' score,
	// score from kills this round, and score from being on the winning team early.
	void executeScoreboard () {
		
	}

    /// <summary>
    ///  Using the dual joystick control scheme.
    /// </summary>
    /// 
    private void doNetworkUpdate(bool allowedToMove) {
        if (lastNetworkInputRightEvent.magnitude > shootThreshold) {
            shoot(lastNetworkInputRightEvent);
        }
        if (allowedToMove)
        {
            move(lastNetworkInputLeftEvent);
        }
    }
    void doLocalUpdate() {
        getInputs();

        if ((Input.GetButton(axes[2, (PlayerNumber - 1)]))) {
            taunt();
        }
    }

    void taunt() {
        if (Time.time > nextTaunt && (myProjectile == null)) {
            //put the taunting action here
            nextTaunt = Time.time + tauntRate;
            if (dodgeAbility) {
                dodging = true;
            }
            StartCoroutine(tauntNumber(tauntNum));
        }
    }

    IEnumerator tauntNumber(int i) {
        for (int f = 0; f < 2; f++) {
            if (f == 0) {
                if (i == 0 || i == 1) {
                    for (int k = 0; k < 4; k++) {
                        for (int j = 0; j < 5; j++) {
                            if (k == 0 || k == 2) {
                                if (i == 0) {
                                    light.color = Color.white;
                                    yield return null;
                                } else if (i == 1) {
                                    spriteRenderer.color = Color.white;
                                    yield return null;
                                }
                            }
                            if (k == 1 || k == 3) {
                                if (i == 0) {
                                    light.color = lightColor;
                                    yield return null;
                                } else if (i == 1) {
                                    spriteRenderer.color = normal;
                                    yield return null;
                                }
                            }

                        }
                    }
                } else if (i == 2) {
                    for (int j = 0; j < 20; j++) {
                        transform.Rotate(18f * Vector3.up);
                        yield return null;
                    }
                } else if (i == 3) {
                    for (int j = 0; j < 40; j++) {
                        transform.Rotate(9f * Vector3.forward);
                        yield return null;
                    }
                }
            } else if (f == 1) {
                dodging = false;
            }
        }

    }

    void getInputs() {
        if (twoJoystick)
        {
            Vector3 AxisInput = (new Vector3(Input.GetAxis(axes[0, (PlayerNumber - 1)]), Input.GetAxis(axes[1, (PlayerNumber - 1)]))).normalized;
            Vector3 AxisInput2 = (new Vector3(Input.GetAxis(axes[3, (PlayerNumber - 1)]), Input.GetAxis(axes[4, (PlayerNumber - 1)]))).normalized;


            if (AxisInput2.magnitude > shootThreshold)
            {
                shoot(AxisInput2);
            }
            move(AxisInput);
        }
        else if (oneJoystick)
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
                //moveAlt(AxisInput.normalized);
            }


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
		else if (m8s4) {
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

        float angle = Vector3.Angle(Vector3.right, direction);
        if (direction.y < 0)
        {
            angle = 360 - angle;
        }
        angle -= 90;
        //Debug.Log("the angle is: " + angle);
        fireDirectionIndicator.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (coolDown)
        {

            if (fireButton && Time.time > nextFire && !dodging && scoreManager.myGameState == ScoreManager.gameState.Gameplay)
            {
                nextFire = Time.time + fireRate;
                fire(direction);
            }
        }
        else {
            if (fireButton && myProjectile == null && !dodging && scoreManager.myGameState == ScoreManager.gameState.Gameplay)
            {
                //StartCoroutine(cooldownIndicator());
                //StartCoroutine(fire(direction));
                fire(direction);
            }
        }

    }

    void fire(Vector3 direction)
    {
        shootSource.Play();
        StartCoroutine(fireAnimation());
        paintUnderMe(3);
        Vector3 temp = transform.position + direction.normalized * offset;
        float x = Mathf.Round(temp.x / gridSize) * gridSize;
        float y = Mathf.Round(temp.y / gridSize) * gridSize;
        GameObject paint = Instantiate(projectileParent, new Vector3(x, y), Quaternion.LookRotation(Vector3.forward, direction)) as GameObject;
        projectileParent parent = paint.GetComponent<projectileParent>();
        myProjectile = paint;
        parent.myColor = paintColor;
        parent.grid = grid;
        parent.teamNum = teamNum;
        parent.playerNumber = PlayerNumber;
        parent.colorNumber = colorNumber;

        
    }

	public void scoreboardShoot(double distance) {
		// bool fireButton;
		// if (!IS_LOCALLY_CONTROLLED) {
			// read from the last event
			// twoJoystick is essentially true here, so just return true.
			// also, shoot is only called if the second stick is active
		// 	fireButton = true;
		// } else {
			// read from the standard axis stuff
		// 	fireButton = (Input.GetButton(axes[2, (PlayerNumber - 1)]) || (m8s4 || m8s8 || twoJoystick || gridMovement));
		// }

		// if (fireButton &&  myProjectile == null && !dodging) {
			//StartCoroutine(cooldownIndicator());
			//StartCoroutine(fire(direction));
		Vector3 direction = Vector3.up;
		shootSource.Play();
		StartCoroutine(fireAnimation());
		paintUnderMe(3);
		GameObject paint = Instantiate(projectileParent, transform.position + direction.normalized * offset, Quaternion.LookRotation(Vector3.forward, direction)) as GameObject;
		projectileParent parent = paint.GetComponent<projectileParent>();
		myProjectile = paint;
		parent.myColor = paintColor;
		parent.grid = grid;
		parent.teamNum = teamNum;
		parent.playerNumber = PlayerNumber;
		parent.colorNumber = colorNumber;
		parent.shotDistance = distance;
		// }

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
        indicatorRenderer.color = fired;
        //transform.GetChild(0).gameObject.SetActive(false);
        //light.color = Color.white;
        //light.intensity = 1.25f;
        yield return new WaitForSeconds(fireRate);
        //light.intensity = normalIntensity;
        //light.color = normal;
        //transform.GetChild(0).gameObject.SetActive(true);
        indicatorRenderer.color = normal;
    }

    void move(Vector3 direction) {
        if (hitWall) {
            transform.Translate(-playerSpeed * direction * Time.deltaTime * (float)1.3);
            hitWall = false;
        }

        else if (isValidPosition(gameObject.transform.position + playerSpeed * direction * Time.deltaTime)) {
            transform.Translate(playerSpeed * direction * Time.deltaTime);
        }
        //else if (isValidPosition(gameObject.transform.position + new Vector3(0, playerSpeed * direction.y * Time.deltaTime)))
        //{
        //    transform.Translate(playerSpeed * new Vector3(0, playerSpeed * direction.y).normalized * Time.deltaTime);
        //}
        //else if (isValidPosition(gameObject.transform.position + new Vector3 (playerSpeed * direction.x * Time.deltaTime, 0)))
        //{
        //    transform.Translate(playerSpeed * new Vector3(playerSpeed * direction.x,0 ).normalized * Time.deltaTime);
        //}
        else
        {
            transform.Translate(slowFactor * playerSpeed * direction * Time.deltaTime);
        }
    }

    public void resetTeamNum()
    {
        teamNum = PlayerNumber;
    }

    bool isValidPosition(Vector3 position) {
        float gridSize = gridController.gridBlock.transform.localScale.x;

        //sprite only has to touch color
        if (movementType.Equals("touchblock")) {
            int gridX = Mathf.RoundToInt(position.x / gridSize);
            int gridY = Mathf.RoundToInt(position.y / gridSize);
            if (!gridController.inGridBounds(gridX, gridY)) {
                // out of bounds, then get outta here
                return false;
            }
			if (paintColor == gridController.getGridColor (gridX, gridY)) {
				return true;
			} else {
				return false;
			}
		//sprite has to be within color
        } else if (movementType.Equals("immerse")) {
            int gridX = Mathf.RoundToInt(position.x / gridSize);
            int gridY = Mathf.RoundToInt(position.y / gridSize);
            float playerRadius = GetComponent<SpriteRenderer>().bounds.size.x / 2;
            int gridLeft = Mathf.RoundToInt((position.x + playerRadius) / gridSize);
            int gridRight = Mathf.RoundToInt((position.x - playerRadius) / gridSize);
            int gridUp = Mathf.RoundToInt((position.y + playerRadius) / gridSize);
            int gridDown = Mathf.RoundToInt((position.y - playerRadius) / gridSize);
            if (!gridController.inGridBounds(gridLeft, gridY) || !gridController.inGridBounds(gridRight, gridY) ||
                !gridController.inGridBounds(gridX, gridUp) || !gridController.inGridBounds(gridX, gridDown)) {
                return false;
            }

            if (paintColor == gridController.getGridColor(gridLeft, gridY) && paintColor == gridController.getGridColor(gridRight, gridY) &&
                paintColor == gridController.getGridColor(gridX, gridUp) && paintColor == gridController.getGridColor(gridX, gridDown)) {
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    public void setColor(int i) {
        colorNumber = i;
        normal = Constants.playerColorChoices[i];
        spriteRenderer.color = normal;
        fired = Constants.firedColors[i];
        lightColor = Constants.lightColors[i];
        light.color = lightColor;
        paintColor = Constants.paintColors[i];

        // send the color change event to the player
        if (!IS_LOCALLY_CONTROLLED) {
            sendPlayerColorChange(i);
        }
    }

    public Color setBrightness(Color col, float change)
    {
        //Debug.Log("brightness change");
        return HSBColor.ToColor(new HSBColor(HSBColor.FromColor(col).h, HSBColor.FromColor(col).s, HSBColor.FromColor(col).b + change, HSBColor.FromColor(col).a));
    }
    public Color setSaturation(Color col, float change)
    {
        //Debug.Log("saturation change");
        return HSBColor.ToColor(new HSBColor(HSBColor.FromColor(col).h, HSBColor.FromColor(col).s + change, HSBColor.FromColor(col).b, HSBColor.FromColor(col).a));
    }

    public void shadeChange()
    {
        //TODO:  Set the index of the shade change by the order I am in the team
        Predicate<string> findMe = (string p) => { return p == PlayerNumber.ToString(); };
        float index = ScoreManager.Instance.currentColors[teamNum-1].FindIndex(findMe);
        if (index >= 4.0f)
        {
            normal = setSaturation(Constants.paintColors[colorNumber], -colorShift * (index - 3.0f));
        }
        else
        {
            normal = setBrightness(Constants.playerColorChoices[colorNumber], index * colorShift);
        }

        spriteRenderer.color = normal;
    }
    public void paintUnderMe(int size) {
        int x = Mathf.RoundToInt(transform.position.x / gridSize);
        int y = Mathf.RoundToInt(transform.position.y / gridSize);
        for (int i = 0; i < 2 * size; i++) {
            for (int j = 0; j < 2 * size; j++) {
                //gridController.grid[x + i -size, y + j - size].GetComponent<SpriteRenderer>().color = normal;
                gridController.setGridBlockToColor(x + i - size, y + j - size, paintColor);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll) {
        //Debug.Log(coll);
		if (coll.gameObject.tag == "wall") {
			hitWall = true;
		}
		//if (coll.gameObject.tag == "paint" && coll.gameObject.GetComponent<SpriteRenderer>().color != normal && !dodging)
        //{
        //    normal = coll.gameObject.GetComponent<SpriteRenderer>().color;
        //    spriteRenderer.color = normal;
        if (coll.gameObject.tag == "paint" && coll.gameObject.GetComponent<SpriteRenderer>().color != paintColor && !dodging) {
            convertSource.Play();
            sendControllerSimpleVibrate();
            setColor(coll.gameObject.GetComponent<shotMovement>().colorNumber);
            
            //coll.transform.parent.gameObject.GetComponentInParent<playerClass>().shadeChange();
            GameObject hitIndicator = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
            hitIndicator.GetComponent<ParticleSystem>().startColor = paintColor;
            paintUnderMe(4);
            if (scoreManager != null) {
                scoreManager.ChangeScore(PlayerNumber.ToString(), "deaths", 1);
                scoreManager.changeColorCount(teamNum, coll.gameObject.GetComponent<shotMovement>().teamNum, PlayerNumber.ToString());
                teamNum = coll.gameObject.GetComponent<shotMovement>().teamNum;
                int playerWhoShotMe = coll.gameObject.GetComponent<shotMovement>().playerNumber;
                scoreManager.ChangeScore(playerWhoShotMe.ToString(), "kills", 1);
                scoreManager.ChangeScore(playerWhoShotMe.ToString(), "score", 1);
            }
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                player.GetComponent<playerClass>().shadeChange();
            }
            

        }
    }

    public void setNetworkPlayerId(int id) {
        networkPlayerId = id;
    }

    public int getNetworkPlayerId() {
        return networkPlayerId;
    }

    private bool isAcceptingNetworkMovement() {
        ScoreManager.gameState state = ScoreManager.getInstance().getCurrentGameState();
        return (state == ScoreManager.gameState.Gameplay || state == ScoreManager.gameState.InLobby);
    }

    private bool isAcceptingNetworkFiring() {
        ScoreManager.gameState state = ScoreManager.getInstance().getCurrentGameState();
        return (state == ScoreManager.gameState.Gameplay);
    }

    // handle events
    private void OnPhotonNetworkEvent(byte eventcode, object content, int senderid) {
        // everything is in json format

        PhotonPlayer sender = PhotonPlayer.Find(senderid);  // who sent this?
        if (sender.ID != networkPlayerId) {
            // not input from our player!
            return;
        }

        byte[] byteContent;
        string contentStringJson;

        switch (eventcode) {
            // player input for 0
            case Constants.PLAYER_INPUT_EVENT_CODE:
                byteContent = (byte[])content;
                contentStringJson = Encoding.UTF8.GetString(byteContent);
                PlayerInputEvent playerInput = PlayerInputEvent.CreateFromJSON(contentStringJson);
                lastNetworkInputLeftEvent = new Vector3(playerInput.left_x, playerInput.left_y);
                lastNetworkInputRightEvent = new Vector3(playerInput.right_x, playerInput.right_y);
                // now we have what we need
                //if (isAcceptingNetworkMovement()) {
                //    lastNetworkInputLeftEvent = new Vector3(playerInput.left_x, playerInput.left_y);
                //}

                //if (isAcceptingNetworkFiring()) {
                //    lastNetworkInputRightEvent = new Vector3(playerInput.right_x, playerInput.right_y);
                //}
                break;

            case Constants.PLAYER_TAUNT_EVENT_CODE:
                //Debug.Log("got network taunt");
                if (isAcceptingNetworkMovement()) {
                    taunt();
                }
                break;
        }
    }

    private void sendControllerSimpleVibrate() {
        Debug.Log("sending over the vibrate");
        controllerPerformVibrateEvent hapticEvent = new controllerPerformVibrateEvent(getNetworkPlayerId());
        byte[] haptic = hapticEvent.getBytes();
        bool isHaptic = false;
        sendNetworkEvent(Constants.PLAYER_CONTROLLER_VIBRATE_SIMPLE, haptic, isHaptic);
    }

    // If the player's color changed, then send that as a network event.
    private void sendPlayerColorChange(int newColor) {
        controllerColorChangeEvent colorEvent = new controllerColorChangeEvent(newColor, getNetworkPlayerId());

        // color change
        byte[] content = colorEvent.getBytes();

        sendNetworkEvent(Constants.PLAYER_COLOR_CHANGE_EVENT_CODE, content, true);
    }

    private void sendNetworkEvent(byte eventCode, byte[] content, bool reliable) {
        // todo: use RaiseEventOptions?
        PhotonNetwork.RaiseEvent(eventCode, content, reliable, null);
    }
}
