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

	Dictionary< string, Dictionary<string, int> > playerScores;
    public List<int> conversionScores;

	int changeCounter = 0;
	int roundNumber;

	public int numRounds;

	public float delayBetweenRounds;

	public GameObject Grid;

    public Dictionary<int, List<string>> currentColors;

    int numPlayers;

	void Awake() {
		Debug.Log ("Awake");
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

	IEnumerator LoadRoundAfterDelay (float delay, GameObject timer) {
		float timeRemaining = delay;
		while (timeRemaining > 0) {
			timer.GetComponent<UnityEngine.UI.Text> ().text = ((int)timeRemaining).ToString();
			yield return new WaitForSeconds (1);
			timeRemaining--;
		}
        Debug.Log("starting new game");
		// SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	IEnumerator LoadSceneAfterDelay (float delay, GameObject timer) {
		float timeRemaining = delay;
		while (timeRemaining > 0) {
			timer.GetComponent<UnityEngine.UI.Text> ().text = ((int)timeRemaining).ToString();
			yield return new WaitForSeconds (1);
			timeRemaining--;
		}
		Debug.Log("starting new game");
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

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
				resetPlayerPositionsForScoreboard ();

				// ScoreboardCanvas = GameObject.Find ("ScoreboardCanvas");
				// ScoreboardCanvas.GetComponent<CanvasGroup> ().alpha = 1f;
				// Debug.Log ("set scoreboard visible");
				resetCurrentColors ();
				GameObject timer = GameObject.Find ("Timer");
				StartCoroutine (LoadSceneAfterDelay (delayBetweenRounds, timer)); 
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

	private void resetPlayerPositionsForScoreboard() {
		var gridScript = Grid.GetComponent<gridController>();
		double gridCenter = Grid.transform.position.x + gridScript.width/2.0;
		double spaceBetweenPlayers = 2;
		double leftStart = gridCenter - spaceBetweenPlayers * (numPlayers / 2.0);

		int playerNumber = 1;
		while (playerNumber <= numPlayers) {
			GameObject thisPlayer = GameObject.Find ("player" + playerNumber);
			var thisPlayerScript = thisPlayer.GetComponent<playerClass> ();

			Vector3 position = new Vector3((float)(leftStart + spaceBetweenPlayers * (playerNumber - 1)), (float)1.5,(float)0);
			GameObject fakePlayer = Instantiate(playerPrefab, position, Quaternion.identity) as GameObject;

			var playerScript = fakePlayer.GetComponent<playerClass> ();
			playerScript.PlayerNumber = playerNumber;
			playerScript.paintColor = thisPlayerScript.originalPaintColor;
			playerScript.originalLightColor = thisPlayerScript.originalLightColor;
			Debug.Log ("reset player " + playerNumber);
			StartCoroutine( ShootAfterDelay ((float)2, (float)1, fakePlayer));

			Destroy (thisPlayer);
			playerNumber++;
		}

		//Grid.reset(); TODO

	}

	IEnumerator ShootAfterDelay (float delay, float distance, GameObject player) {
		Debug.Log ("shoot after delay called");
		float timeRemaining = delay;

		while (timeRemaining > 0) {
			// timer.GetComponent<UnityEngine.UI.Text> ().text = ((int)timeRemaining).ToString();
			yield return new WaitForSeconds (1);
			timeRemaining--;
		}

		Debug.Log ("SHOT!!");
		var playerScript = player.GetComponent<playerClass> ();
		playerScript.scoreboardShoot();

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
