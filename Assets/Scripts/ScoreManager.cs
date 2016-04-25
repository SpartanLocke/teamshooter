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
	private GameObject ScoreboardCanvas;


	Dictionary< string, Dictionary<string, int> > playerScores;
    public List<int> conversionScores;

	int changeCounter = 0;
	int roundNumber;

	public int numRounds;

	public float delayBetweenRounds;

    Dictionary<int, List<string>> currentColors;

	int numPlayers = 4;
	// TODO make this a public variable, or read all game objects starting with "player", or something else

	void Awake() {
		Debug.Log ("Awake");
		ScoreboardCanvas = GameObject.Find ("ScoreboardCanvas");
		ScoreboardCanvas.GetComponent<CanvasGroup>().alpha = 0f;
		if (Instance == null) {
			Debug.Log ("null instance");
			DontDestroyOnLoad (gameObject);
			// DontDestroyOnLoad (ScoreboardCanvas);
			Instance = this;
			int playerNumber = 0;
			currentColors = new Dictionary<int, List<string>> ();
			while (playerNumber < numPlayers) {
				playerNumber++;
				string username = playerNumber.ToString ();
				SetScore (username, "score", 0);
				SetScore (username, "kills", 0);
				SetScore (username, "deaths", 0);
				List<string> playerList = new List<string> ();
				playerList.Add (username);
				currentColors.Add (playerNumber - 1, playerList);
			}
			roundNumber = 1;
		} else {
			resetCurrentColors ();
		}
			
	}

	void resetCurrentColors() {
		currentColors = new Dictionary<int, List<string>> ();
		int playerNumber = 0;
		while (playerNumber < numPlayers) {
			playerNumber++;
			string username = playerNumber.ToString ();
			List<string> playerList = new List<string> ();
			playerList.Add (username);
			currentColors[playerNumber - 1] = playerList;
			//Debug.Log (currentColors [playerNumber - 1].Count);
		}
	}

	void Start() {
		//Debug.Log ("Start");
		ScoreboardCanvas = GameObject.Find ("ScoreboardCanvas");
		ScoreboardCanvas.GetComponent<CanvasGroup>().alpha = 0f;
		playerScores = ScoreManager.Instance.playerScores;
		roundNumber = ScoreManager.Instance.roundNumber;
		resetCurrentColors ();

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

    public void changeColorCount(int teamNum, string username)
	{
		currentColors [teamNum - 1].Add (username);
		checkEndCondition ();
    }

	IEnumerator LoadLevelAfterDelay (float delay, GameObject timer) {
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

				ScoreboardCanvas = GameObject.Find ("ScoreboardCanvas");
				ScoreboardCanvas.GetComponent<CanvasGroup> ().alpha = 1f;
				resetCurrentColors ();
				Debug.Log ("set visible");
				GameObject timer = GameObject.Find ("Timer");
				StartCoroutine (LoadLevelAfterDelay (delayBetweenRounds, timer)); 
			} else {
				ScoreboardCanvas = GameObject.Find ("ScoreboardCanvas");
				ScoreboardCanvas.GetComponent<CanvasGroup> ().alpha = 1f;
				// GameObject.Find ("Reset Button").SetActive (true);
				// Destroy (gameObject);
				// GameObject resetButton = GameObject.Find("ResetButton");
				// resetButton.SetActive (true);
				Debug.Log ("final round");
			}
		}
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
