using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour {

	// The map we're building is going to look like:
	//
	//	LIST OF USERS -> A User -> LIST OF SCORES for that user
	//

	Dictionary< string, Dictionary<string, int> > playerScores;
    public List<int> conversionScores;

	int changeCounter = 0;

    Dictionary<int, List<string>> currentColors;

	int numPlayers = 4;
	// TODO make this a public variable, or read all game objects starting with "player", or something else

	void Start() {
		int playerNumber = 0;
        currentColors = new Dictionary<int, List<string>>();
        while (playerNumber < numPlayers)
		{
			playerNumber++;
			string username = playerNumber.ToString();
			SetScore(username, "score", 0);
			SetScore(username, "kills", 0);
			SetScore(username, "deaths", 0);
            List<string> playerList = new List<string>();
            playerList.Add(username);
            currentColors.Add(playerNumber-1, playerList);
		}
	}

	void Init() {
		if(playerScores != null)
			return;

		playerScores = new Dictionary<string, Dictionary<string, int>>();
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
        currentColors[teamNum].Add(username);
        checkEndCondition();
    }

    public void checkEndCondition()
    {
        foreach (int key in currentColors.Keys)
        {
            List<string> converts = currentColors[key];
            if (converts.Count == numPlayers) //everyone is one color
            {
                for (int i=0; i < converts.Count; i++)
                {
                    ChangeScore(converts[i], "score", conversionScores[i]);
                }
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

	public void DEBUG_ADD_KILL_TO_4() {
		Debug.Log("adding score to 4");
		ChangeScore("4", "kills", 1);
		ChangeScore("4", "score", 1);
	}

}
