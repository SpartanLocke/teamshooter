using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

	// The map we're building is going to look like:
	//
	//	LIST OF USERS -> A User -> LIST OF SCORES for that user
	//

	public static ScoreManager Instance;
	public GameObject playerPrefab;
	private GameObject ScoreboardCanvas;
	public enum gameState {Gameplay, SetUpScoreboard, ExecuteScoreboard, Reset, Wait};
	public gameState myGameState;

	Dictionary< string, Dictionary<string, int> > playerScores;
    public List<int> conversionScores;

	int changeCounter = 0;
	public int roundNumber;

	public int numRounds;

	public float delayBetweenRounds;

	public GameObject Grid;

    public Dictionary<int, List<string>> currentColors;

    int numPlayers;

	public double gridCenter;
	public double spaceBetweenPlayers;
	public double leftStart;

    private bool final;

    /*-------------------   Scoreboard UI Objects                   ------------------------------*/

    public GameObject countdown;
    public GameObject ScoreText;


    /*---------------------------------------------------------------------*/
    public GameObject[] levels;
    public GameObject currentLevel;

    public static ScoreManager getInstance() {
        // may the lord bless your soul if this isn't initialized yet
        return Instance;
    }

	void Awake() {
        final = false;

		ScoreboardCanvas = GameObject.Find ("ScoreboardCanvas");
		ScoreboardCanvas.GetComponent<CanvasGroup>().alpha = 0f;

		if (Instance == null) {
			Debug.Log ("null instance");
			// DontDestroyOnLoad (gameObject);
			// DontDestroyOnLoad (ScoreboardCanvas);
			Instance = this;
			currentColors = new Dictionary<int, List<string>> ();
			roundNumber = 1;
		}
			
		var gridScript = Grid.GetComponent<gridController>();
		gridCenter = Grid.transform.position.x + gridScript.width/2.0;
		spaceBetweenPlayers = 2;
		leftStart = gridCenter - spaceBetweenPlayers * (numPlayers / 2.0);

	}

    public gameState getCurrentGameState() {
        return myGameState;
    }

	void resetCurrentColors() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        currentColors = new Dictionary<int, List<string>> ();
		int playerNumber = 0;
		while (playerNumber < numPlayers) {
			playerNumber++;
			string username = playerNumber.ToString ();
			List<string> playerList = new List<string> ();
			playerList.Add (username);
			currentColors[playerNumber - 1] = playerList;

            playerClass player = players[playerNumber - 1].GetComponent<playerClass>();
            player.resetTeamNum();
			//Debug.Log (currentColors [playerNumber - 1].Count);
		}
	}

    public void startGame() {
        numPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
        Debug.Log("startGame() called with " + numPlayers + " players");

        int playerNumber = 0;
        while (playerNumber < numPlayers) {
            playerNumber++;
            string username = playerNumber.ToString();
            SetScore(username, "score", 0);
            SetScore(username, "kills", 0);
            SetScore(username, "deaths", 0);
            List<string> playerList = new List<string>();
            playerList.Add(username);
			//Debug.Log ("adding current colors");
            currentColors.Add(playerNumber - 1, playerList);
        }
        myGameState = gameState.Wait;
    }

    public void startGameButton() {
        Debug.Log("startGameButton pressed");
        startGame();
        StartCoroutine(MoveBackToStartAfterDelay(0.0f, 1.5f));
    }

    void Start() {
		//Debug.Log ("Start");
		ScoreboardCanvas = GameObject.Find ("ScoreboardCanvas");
		ScoreboardCanvas.GetComponent<CanvasGroup>().alpha = 0f;
		playerScores = ScoreManager.Instance.playerScores;
		roundNumber = ScoreManager.Instance.roundNumber;

		GameObject ScoreboardTitle = GameObject.Find ("Title");
		ScoreboardTitle.GetComponent<UnityEngine.UI.Text> ().text = "Round " + roundNumber.ToString () + "/" + numRounds.ToString ();
	}

	void Init() {
		if(playerScores != null)
			return;

		playerScores = new Dictionary<string, Dictionary<string, int>>();
    }

	public void SaveScoresBetweenRounds()
	{
		ScoreManager.Instance.playerScores = playerScores;
		ScoreManager.Instance.roundNumber = roundNumber;
	}

	public void Reset() {
		changeCounter++;
		playerScores = null;
        foreach (int key in currentColors.Keys)
        {
            //make each player list only contain one player
            List<string> playerList = new List<string>();
            playerList.Add(key+1.ToString());
            currentColors[key] = playerList;
        }
    }

	public int GetScore(string username, string scoreType) {
		Init ();

		if(playerScores.ContainsKey(username) == false) {
			// We have no score record at all for this username
			return 0;
		}

		if(playerScores[username].ContainsKey(scoreType) == false) {
			return 0;
		}

		return playerScores[username][scoreType];
	}

	public void SetScore(string username, string scoreType, int value) {
		Init ();

		changeCounter++;
        // Debug.Log(username+" "+ scoreType+" "+ value);
		if(playerScores.ContainsKey(username) == false) {
			playerScores[username] = new Dictionary<string, int>();
		}

		playerScores[username][scoreType] = value;
	}

	public void ChangeScore(string username, string scoreType, int amount) {
		Init ();
		int currScore = GetScore(username, scoreType);
		SetScore(username, scoreType, currScore + amount);
	}

    public void changeColorCount(int oldTeamNum, int newTeamNum, string username)
	{
		
		//Debug.Log (username);
		//Debug.Log (oldTeamNum);
		//Debug.Log (currentColors.Count);
		//foreach(var item in currentColors)
		//{
		//	Debug.Log("changeColorCount key: " + item.Key);
		//	Debug.Log("changeColorCount value: " + item.Value);
		//}
		//Debug.Log (currentColors [oldTeamNum - 1].Count);
  //      Debug.Log(oldTeamNum + " " + newTeamNum + " "+currentColors.Keys.Count);
        currentColors[oldTeamNum - 1].Remove(username);
        currentColors [newTeamNum - 1].Add (username);
		checkEndCondition ();
    }

	IEnumerator RestartRoundAfterDelay (float delay, GameObject timer) {
		float timeRemaining = delay;
		while (timeRemaining > 0) {
			// timer.GetComponent<UnityEngine.UI.Text> ().text = ((int)timeRemaining).ToString();
			yield return new WaitForSeconds (1);
			timeRemaining--;
		}
        //Debug.Log("starting new game");
        
		myGameState = gameState.Gameplay;
		// SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

//	IEnumerator LoadSceneAfterDelay (float delay, GameObject timer) {
//		float timeRemaining = delay;
//		while (timeRemaining > 0) {
//			timer.GetComponent<UnityEngine.UI.Text> ().text = ((int)timeRemaining).ToString();
//			yield return new WaitForSeconds (1);
//			timeRemaining--;
//		}
//		Debug.Log("starting new game");
//		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//	}

    public void checkEndCondition()
    {
		bool endCondition = false;
        foreach (int key in currentColors.Keys)
        {
            List<string> converts = currentColors[key];
			//Debug.Log (converts.Count);
            if (converts.Count == numPlayers) //everyone is one color
            {
                for (int i=0; i < converts.Count; i++)
                {
                    ChangeScore(converts[i], "score", conversionScores[i]);
                }
				endCondition = true;
            }
        }

		if (endCondition) {
			if (roundNumber < numRounds) {
				roundNumber += 1;
                
                StartCoroutine(scoreboard());
			} else {
                final = true;
				// ScoreboardCanvas = GameObject.Find ("ScoreboardCanvas");
				// ScoreboardCanvas.GetComponent<CanvasGroup> ().alpha = 1f;
				// GameObject.Find ("Reset Button").SetActive (true);
				// Destroy (gameObject);
				// GameObject resetButton = GameObject.Find("ResetButton");
				// resetButton.SetActive (true);
				Debug.Log ("final round");
                StartCoroutine(scoreboard());
            }
		}
			
    }
    public void ScoreboardTestButton()
    {
        StartCoroutine(scoreboard());
    }
    IEnumerator scoreboard()
    {
        yield return new WaitForSeconds(1);
        SaveScoresBetweenRounds();
        Destroy(currentLevel);
        destroyProjectiles();
        resetPlayerPositionsForScoreboard();

        // ScoreboardCanvas = GameObject.Find ("ScoreboardCanvas");
        // ScoreboardCanvas.GetComponent<CanvasGroup> ().alpha = 1f;
        // Debug.Log ("set scoreboard visible");
        resetCurrentColors();

        //TODO do this only after finished displaying the scores
        GameObject timer = GameObject.Find("Timer");
        // StartCoroutine (RestartRoundAfterDelay (delayBetweenRounds, timer)); 
    }

    private void destroyProjectiles() {
		var gameObjects = GameObject.FindGameObjectsWithTag ("paint");

		for(var i = 0 ; i < gameObjects.Length ; i ++)
		{
			Destroy(gameObjects[i]);
		}
	}

	private void resetPlayerPositionsForScoreboard() {
		var gridScript = Grid.GetComponent<gridController>();

		
        double maxScore = 0;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject thisPlayer in players)
        {
            var playerNumber = thisPlayer.GetComponent<playerClass>().PlayerNumber;
            if (playerScores[playerNumber.ToString()]["score"] > maxScore)
            {
                maxScore = playerScores[playerNumber.ToString()]["score"];
            }
        }
        foreach(GameObject thisPlayer in players)
        {
            var playerScript = thisPlayer.GetComponent<playerClass>();
            var playerNumber = playerScript.PlayerNumber;
            

            playerScript.setColor(playerScript.colorChoiceNumber);

            
            double shotDistance = (GetScore(playerScript.PlayerNumber.ToString(), "kills") / maxScore) * (gridScript.height - 5);
            StartCoroutine(ShootAfterDelay(4.0f, shotDistance, thisPlayer,false,true));
            if (playerScores[playerNumber.ToString()]["score"] == maxScore)
            {
                shotDistance = (GetScore(playerScript.PlayerNumber.ToString(), "score") / maxScore) * (gridScript.height - 5);
                StartCoroutine(ShootAfterDelay(8.0f, shotDistance, thisPlayer, true, true));
            }
            shotDistance = (GetScore(playerScript.PlayerNumber.ToString(), "score") / maxScore) * (gridScript.height - 5);
            StartCoroutine(ShootAfterDelay(8.0f, shotDistance, thisPlayer,true,false));
            
            
        }
        StartCoroutine(scaleAfterDelay(ScoreText,"Kills", .1f, 1f, .08f, 3.0f, true, 10.0f,.08f,2.5f));
        if (final)
        {
            StartCoroutine(scaleAfterDelay(ScoreText, "Final Score", .1f, 1f, .08f, 6.3f, true, 10.0f, .08f, 5.0f));
        }
        else
        {
            StartCoroutine(scaleAfterDelay(ScoreText, "Time On Winning Team", .1f, 1f, .08f, 6.3f, true, 10.0f, .08f, 3.0f));
        }
        myGameState = gameState.SetUpScoreboard;
		gridScript.resetGrid();

	}

	public IEnumerator ShootAfterDelay (float delay, double distance, GameObject player, bool second,bool max) {
		//Debug.Log ("shoot after delay called");
		float timeRemaining = delay;

        yield return new WaitForSeconds(delay);

		//Debug.Log ("SHOT!!");
		var playerScript = player.GetComponent<playerClass> ();
		playerScript.scoreboardShoot(distance);
        if (second&&max&& !final)
        {
            StartCoroutine(MoveBackToStartAfterDelay(4.0f, 1.5f));
        }
        else if (second && final)
        {
            yield return new WaitForSeconds(4.0f);
            StartCoroutine(LoadLevel("controller menu"));
        }
	}
    IEnumerator LoadLevel(string levelName)
    {
        yield return StartCoroutine(CameraFade.GetCameraFade().WaitForCameraFade(true));
        SceneManager.LoadScene(levelName);
    }

    public IEnumerator MoveBackToStartAfterDelay (float delay1, float delay2) {
		float timeRemaining = delay1 + delay2;

        yield return new WaitForSeconds(delay1);

        //Load a new level;
        if (roundNumber != 1)
        {
            currentLevel = Instantiate(levels[Random.Range(0, levels.Length)]) as GameObject;
        }
        

		myGameState = gameState.Reset;
		var gridScript = Grid.GetComponent<gridController>();
		gridScript.resetGrid ();
		Debug.Log ("game state reset");

        yield return new WaitForSeconds(delay2);
        StartCoroutine(scaleAfterDelay(countdown, "3", .1f, 1f, .15f,0, true, 10.0f,.15f, 0.1f));
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(scaleAfterDelay(countdown, "2", .1f, 1f, .15f, 0, true, 10.0f, .15f, 0.1f));
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(scaleAfterDelay(countdown, "1", .1f, 1f, .15f, 0, true, 10.0f, .15f, 0.1f));
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(scaleAfterDelay(countdown, "GO!", .1f, 1f, .15f, 0, true, 10.0f, .15f, 0.1f));
        myGameState = gameState.Gameplay;
		Debug.Log ("game state gameplay");
	}

    public IEnumerator scaleAfterDelay(GameObject thing,string Text,float init, float final,float rate,float delay, bool shadow, float shadowLength,float rate2, float delay2)
    {
        yield return new WaitForSeconds(delay);
        int i = 0;
        setText(thing, Text);
        thing.transform.localScale = new Vector3(init, init);
        while (thing.transform.localScale.x < final)
        {
            thing.transform.localScale = new Vector3(init+i*rate, init+i*rate);
            i++;
            yield return null;
        }
        if (shadow)
        {
            GameObject newShadow = Instantiate(thing) as GameObject;
            // newShadow.transform.parent = thing.transform.parent;
            // http://docs.unity3d.com/ScriptReference/Transform.SetParent.html
            newShadow.transform.SetParent(thing.transform.parent, false);
            newShadow.transform.position = thing.transform.position;
            for (int j = 0; j < shadowLength; j++)
            {

                newShadow.transform.localScale = new Vector3(final + rate2 * j, final + rate2 * j);
                Color temp = newShadow.GetComponent<Text>().color;
                temp.a = 1.0f - (j + 1) * 1.0f / shadowLength;
                newShadow.GetComponent<Text>().color = temp;
                yield return null;
            }
            Destroy(newShadow);
        }
        yield return new WaitForSeconds(delay2);
        setText(thing, "");
    }
    public void setText(GameObject ob, string Text)
    {
        ob.GetComponent<Text>().text = Text;
    }

    public string[] GetPlayerNames() {
		Init ();
		return playerScores.Keys.ToArray();
	}

	public string[] GetPlayerNames(string sortingScoreType) {
		Init ();

		return playerScores.Keys.OrderByDescending( n => GetScore(n, sortingScoreType) ).ToArray();
	}

	public int GetChangeCounter() {
		return changeCounter;
	}

    public gameState getGameState() {
        return myGameState;
    }
}
