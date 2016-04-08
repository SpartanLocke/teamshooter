using UnityEngine;
using System.Collections;

public class playerClass : MonoBehaviour {
    public float playerSpeed; //speed player moves
    public float playerSpeedAlt; // speed player turns

    public int PlayerNumber;
    private string[,] axes = new string[,] { {"HorizontalP1", "HorizontalP2", "HorizontalP3", "HorizontalP4" }, { "VerticalP1", "VerticalP2", "VerticalP3", "VerticalP4" }, {"FireP1","FireP2", "FireP3", "FireP4" },
                                             {"HorizontalShootP1", "HorizontalShootP2", "HorizontalShootP3", "HorizontalShootP4" }, { "VerticalShootP1", "VerticalShootP2", "VerticalShootP3", "VerticalShootP4" }};

    public bool oneJoystick;
    public bool twoJoystick;
    public float shootThreshold;
    public bool m8s4;
    public bool m8s8;
    private Vector3 shootDir;
    private int frames;
    public int frameDelay;
    
    public float fireRate;
    public int numShots;
    public float offset;
    public float weight;
    public GameObject projectile;
    public Color normal;
    public Color fired;
    
    public GameObject grid;
    private Vector3 oldInput = new Vector3 (0,0);
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().color = normal;
        paintUnderMe();
    }
    void Update()
    {
        
        //MoveForward(); // Player Movement
        //TurnRightAndLeft();//Player Turning
        if (oneJoystick)
        {
            Vector3 AxisInput = (new Vector3(Input.GetAxis(axes[0, (PlayerNumber - 1)]), Input.GetAxis(axes[1, (PlayerNumber - 1)]))).normalized;
            
            if (AxisInput == new Vector3(0, 0))
            {
                shoot(oldInput);
            }
            else
            {
                shoot(AxisInput);
                oldInput = AxisInput;
            }
            move(AxisInput);
        } else if (twoJoystick)
        {
            Vector3 AxisInput = (new Vector3(Input.GetAxis(axes[0, (PlayerNumber - 1)]), Input.GetAxis(axes[1, (PlayerNumber - 1)]))).normalized;
            Vector3 AxisInput2 = (new Vector3(Input.GetAxis(axes[3, (PlayerNumber - 1)]), Input.GetAxis(axes[4, (PlayerNumber - 1)]))).normalized;
            
            if (AxisInput2.magnitude > shootThreshold)
            {
                shoot(AxisInput2);
            }
            move(AxisInput);

        } else if (m8s4)
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
        } else if (m8s8)
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


    private float nextFire = 0.0f;
    void shoot(Vector3 direction)
    {
        if (direction == Vector3.zero) return;
        if (( Input.GetButton(axes[2,(PlayerNumber-1)]) || (m8s4 || m8s8 || twoJoystick) )
            && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            
            StartCoroutine(cooldownIndicator());
            StartCoroutine(fire(direction));
        }
        
    }

    IEnumerator cooldownIndicator()
    {
        gameObject.GetComponent<SpriteRenderer>().color = fired;
        yield return new WaitForSeconds(fireRate);
        gameObject.GetComponent<SpriteRenderer>().color = normal;
    }

    IEnumerator fire(Vector3 direction)
    {
        for (int i = 0; i < numShots;i++)
        {
            /*for (int j = 0; j < 1; j++)
            {
                yield return null;
            }*/
            Vector3 newPosition = transform.position - direction.normalized * offset + Quaternion.Euler(0, 0, -90) * (direction.normalized* i * weight);
            GameObject paint = Instantiate(projectile, newPosition , Quaternion.LookRotation(Vector3.forward, direction)) as GameObject;
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

    void move(Vector3 direction)
    {
        
        if(isValidPosition(gameObject.transform.position + playerSpeed * direction * Time.deltaTime))
        {
            transform.Translate(playerSpeed * direction * Time.deltaTime);
        }
        
    }
    void moveAlt()
    {
        Vector3 movement = new Vector3(0,0);
        if (Input.GetKey("up"))//Press up arrow key to move forward on the Y AXIS
        {
            movement = movement + new Vector3 (0, playerSpeed * Time.deltaTime);
        }
        if (Input.GetKey("down"))//Press up arrow key to move forward on the Y AXIS
        {
            movement = movement + new Vector3(0, -playerSpeed * Time.deltaTime);
        }
        if (Input.GetKey("right")) //Right arrow key to turn right
        {
            movement = movement + new Vector3(playerSpeed * Time.deltaTime,0);
        }

        if (Input.GetKey("left"))//Left arrow key to turn left
        {
            movement = movement + new Vector3(-playerSpeed * Time.deltaTime, 0);
        }
        transform.Translate(playerSpeedAlt * movement * Time.deltaTime);

    }

    bool isValidPosition(Vector3 position)
    {
       
        gridController gridController = grid.GetComponent<gridController>();
        float gridSize = gridController.gridBlock.transform.localScale.x;
        if (normal == gridController.grid[Mathf.RoundToInt(position.x / gridSize), Mathf.RoundToInt(position.y / gridSize)].GetComponent<SpriteRenderer>().color)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void paintUnderMe()
    {
        gridController gridController = grid.GetComponent<gridController>();
        float gridSize = gridController.gridBlock.transform.localScale.x;
        gridController.grid[Mathf.RoundToInt(transform.position.x / gridSize), Mathf.RoundToInt(transform.position.y / gridSize)].GetComponent<SpriteRenderer>().color = normal;
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log(coll);
        if (coll.gameObject.tag == "paint" )
        {
            /*coll.gameObject.GetComponent<playerClass>().normal = normal;
            coll.gameObject.GetComponent<playerClass>().fired = fired;
            coll.gameObject.GetComponent<SpriteRenderer>().color = normal;*/

            normal = coll.gameObject.GetComponent<SpriteRenderer>().color;
            gameObject.GetComponent<SpriteRenderer>().color = normal;

        }
    }
}
