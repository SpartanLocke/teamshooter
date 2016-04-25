using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerNetworkStatusHandler : MonoBehaviour {
    public static bool isGameStarted = false;

    private gridController gridController;
    private HashSet<int> spawnedPlayersTable;
	private int count = 0;

    public GameObject playerPrefab;
    public GameObject hideableStartGamePrompt;
	public GameObject spawnpointsPrefab;

    void Awake() {
        gridController = GameObject.FindGameObjectWithTag("gridGameObject").GetComponent<gridController>();

        isGameStarted = false;
        spawnedPlayersTable = new HashSet<int>();

        if (PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
            // we just restarted. respawn everybody
            onNewRound();
        }
    }

    void Start() {
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("escape button pressed. going back to main menu");

            if (PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
                ConnectAndJoinRandom.setJoinRandomRooms(false);
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            } else {
                Debug.Log("some networking status failed on exit button press");
                SceneManager.LoadScene("controller menu");
            }
        }

        if (!isGameStarted) {
            // we're waiting on the server person to pop off
            if (Input.GetKeyDown(KeyCode.F)) {
                PlayerNetworkStatusHandler.isGameStarted = true;

                // hide the prompt, if its there
                if (hideableStartGamePrompt != null) {
                    hideableStartGamePrompt.SetActive(false);
                } else {
                    Debug.Log("couldn't hide the null hideable prompt");
                }
            }
        }
    }

    private void sendPlayerInitDataRequest() {
        // send the init data via reliable transmission
        byte[] content = new byte[1];
        bool reliable = true;

        PhotonNetwork.RaiseEvent(Constants.SERVER_REQUEST_INIT_DATA_EVENT_CODE, content, reliable, null);
        Debug.Log("send init data request");
    }

    void OnLeftRoom() {
        Debug.Log("left the room!");

        //// delete all the players
        //GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");
        //foreach (GameObject go in gos) {
        //    playerClass playerScript = go.GetComponent<playerClass>();
        //    Destroy(playerScript.gameObject);
        //}

        SceneManager.LoadScene("controller menu");
    }

    public void onNewRound() {
        Debug.Log("round just restarted. checking for previous players!");

        foreach (PhotonPlayer ply in PhotonNetwork.otherPlayers) {
            int playerId = ply.ID;
            Debug.Log("spawning " + playerId);

            spawnPlayer(playerId);
        }

        sendPlayerInitDataRequest();
    }

    public void OnJoinedRoom() {
        Debug.Log("joined room. checking for previous players!");

        foreach (PhotonPlayer ply in PhotonNetwork.otherPlayers) {
            int playerId = ply.ID;
            Debug.Log("spawning " + playerId);

            spawnPlayer(playerId);
        }

        sendPlayerInitDataRequest();
    }

    void OnPhotonPlayerConnected(PhotonPlayer player) {
        Debug.Log("playerJoined");

        int playerId = player.ID;
        spawnPlayer(playerId);
        sendPlayerInitDataRequest();
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer player) {
        Debug.Log("player left!");

        int playerId = player.ID;
        // destroy the player that left

        GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in gos) {
            playerClass playerScript = go.GetComponent<playerClass>();
            if (playerScript.getNetworkPlayerId() == playerId) {
                Destroy(playerScript.gameObject);
                return;
            }
        }

        Debug.Log("Couldn't destroy player with id: " + playerId);
    }

    private void spawnPlayer(int playerId) {
        if (spawnedPlayersTable.Contains(playerId)) {
            // skip it
            Debug.Log("skipping the repeat spawning of player: " + playerId);
            return;
        }

        spawnedPlayersTable.Add(playerId);
		Vector3 spawnpoint = spawnpointsPrefab.transform.GetChild (count).transform.position;
		count++;

//        float randomX = Random.Range(1, gridController.width - 1);
//        float randomY = Random.Range(1, gridController.height - 1);

		GameObject playerGameObject = Instantiate(playerPrefab, spawnpoint, Quaternion.identity) as GameObject;

        // initialize the player values
        playerClass playerScript = playerGameObject.GetComponent<playerClass>();
        playerScript.setNetworkPlayerId(playerId);
        playerScript.IS_LOCALLY_CONTROLLED = false;

        // set the color to red to start since we don't know the player color yet
        playerScript.setColor(0);
    }
}
