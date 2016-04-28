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
	public enum gameState {Gameplay, SetUpScoreboard, ExecuteScoreboard, Reset};
	public gameState myGameState;

	Dictionary< string, Dictionary<string, int> > playerScores;
    public List<int> conversionScores;

	int changeCounter = 0;
	int roundNumber;

	public int numRounds;

	public float delayBetweenRounds;

	public GameObject Grid;

    public Dictionary<int, List<string>> currentColors;

    int numPlayers;

	public double gridCenter;
	public double spaceBetweenPlayers;
	public double leftStart;

	void Awake() {
		ScoreboardCanvas = GameObject.Find ("ScoreboardCanvas");
		ScoreboardCanvas.GetComponent<CanvasGroup>().alpha = 0f;

		if (Instance == null) {
			Debug.Log ("null instance");
			// DontDestroyOnLoad (gameObject);
			// DontDestroyOnLoad (ScoreboardCanvas);
			Instance = this;
			currentColors = new Dictionary<int, List<string>> ();
			roundNumber = 1;
			startGame ();
		}
			
		var gridScript = Grid.GetComponent<gridController>();
		gridCenter = Grid.transform.position.x + gridScript.width/2.0;
		spaceBetweenPlayers = 2;
		leftStart = gridCenter - spaceBetweenPlayers * (numPlayers / 2.0);

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

    public void startGame()
    {
        numPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
        int playerNumber = 0;
        while (playerNumber < numPlayers)
        {
            playerNumber++;
            string username = playerNumber.ToString();
            SetScore(username, "score", 0);
            SetScore(username, "kills", 0);
            SetScore(username, "deaths", 0);
            List<string> playerList = new List<string>();
            playerList.Add(username);
			Debug.Log ("adding current colors");
            currentColors.Add(playerNumber - 1, playerList);
        }
		myGameState = gameState.Gameplay;
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
		
		Debug.Log (username);
		Debug.Log (oldTeamNum);
		Debug.Log (currentColors.Count);
		foreach(var item in currentColors)
		{
			Debug.Log("key: " + item.Key);
			Debug.Log("value: " + item.Value);
		}
		Debug.Log (currentColors [oldTeamNum - 1].Count);
        Debug.Log(oldTeamNum + " " + newTeamNum + " "+currentColors.Keys.Count);
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
        Debug.Log("starting new game");
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
				SaveScoresBetweenRounds ();
				destroyProjectiles ();
				resetPlayerPositionsForScoreboard ();

				// ScoreboardCanvas = GameObject.Find ("ScoreboardCanvas");
				// ScoreboardCanvas.GetComponent<CanvasGroup> ().alpha = 1f;
				// Debug.Log ("set scoreboard visible");
				resetCurrentColors ();

				//TODO do this only after finished displaying the scores
				GameObject timer = GameObject.Find ("Timer");
				// StartCoroutine (RestartRoundAfterDelay (delayBetweenRounds, timer)); 
			} else {
				// ScoreboardCanvas = GameObject.Find ("ScoreboardCanvas");
				// ScoreboardCanvas.GetComponent<CanvasGroup> ().alpha = 1f;
				// GameObject.Find ("Reset Button").SetActive (true);
				// Destroy (gameObject);
				// GameObject resetButton = GameObject.Find("ResetButton");
				// resetButton.SetActive (true);
				Debug.Log ("final round");
			}
		}
			
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

		int playerNumber = 1;
		double maxScore = 0;
		while (playerNumber <= numPlayers) {
			if (playerScores [playerNumber.ToString ()] ["score"] > maxScore) {
				maxScore = playerScores [playerNumber.ToString ()] ["score"];
			}
			playerNumber++;
		}

		playerNumber = 1;
		while (playerNumber <= numPlayers) {
			GameObject thisPlayer = GameObject.Find ("player" + playerNumber);
			var playerScript = thisPlayer.GetComponent<playerClass> ();

			playerScript.paintColor = playerScript.originalPaintColor;
			playerScript.lightColor = playerScript.originalLightColor;
			playerScript.fired = playerScript.originalFiredColor;
			playerScript.light.color = playerScript.lightColor;

			Debug.Log ("reset player " + playerNumber);
			Debug.Log (playerScript.originalPaintColor);
			Debug.Log (playerScript.paintColor);

			double shotDistance = (GetScore(playerNumber.ToString(), "score") / maxScore) * (gridScript.height - 5);
			StartCoroutine( ShootAfterDelay ((float)5, shotDistance, thisPlayer));

			playerNumber++;
		}

		myGameState = gameState.SetUpScoreboard;
		gridScript.resetGrid();

	}

	public IEnumerator ShootAfterDelay (float delay, double distance, GameObject player) {
		Debug.Log ("shoot after delay called");
		float timeRemaining = delay;

		while (timeRemaining > 0) {
			// timer.GetComponent<UnityEngine.UI.Text> ().text = ((int)timeRemaining).ToString();
			yield return new WaitForSeconds (1);
			timeRemaining--;
		}

		Debug.Log ("SHOT!!");
		var playerScript = player.GetComponent<playerClass> ();
		playerScript.scoreboardShoot(distance);
		StartCoroutine (MoveBackToStartAfterDelay ((float)4, (float)4));
	}

	public IEnumerator MoveBackToStartAfterDelay (float delay1, float delay2) {
		float timeRemaining = delay1 + delay2;

		while (timeRemaining > delay1) {
			// timer.GetComponent<UnityEngine.UI.Text> ().text = ((int)timeRemaining).ToString();
			yield return new WaitForSeconds (1);
			timeRemaining--;
		}
			
		myGameState = gameState.Reset;
		var gridScript = Grid.GetComponent<gridController>();
		gridScript.resetGrid ();
		Debug.Log ("game state reset");

		while (timeRemaining > 0) {
			// timer.GetComponent<UnityEngine.UI.Text> ().text = ((int)timeRemaining).ToString();
			yield return new WaitForSeconds (1);
			timeRemaining--;
		}
			
		myGameState = gameState.Gameplay;
		Debug.Log ("game state gameplay");
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
}
