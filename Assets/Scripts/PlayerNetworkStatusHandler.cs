using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Text;

public class PlayerNetworkStatusHandler : MonoBehaviour {
    private gridController gridController;
    private HashSet<int> spawnedPlayersTable;
	private int count = 0;

    public GameObject playerPrefab;
    public GameObject hideableStartGamePrompt;
	public GameObject spawnpointsPrefab;

    void Awake() {
        PhotonNetwork.OnEventCall += this.OnPhotonNetworkEvent;

        gridController = GameObject.FindGameObjectWithTag("gridGameObject").GetComponent<gridController>();

        spawnedPlayersTable = new HashSet<int>();

        //if (PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
        //    // we just restarted. respawn everybody
        //    onNewRound();
        //}
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
                ConnectAndJoinRandom.setJoinRandomRooms(false);
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
                SceneManager.LoadScene("controller menu");
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

        SceneManager.LoadScene("controller menu");
    }

    public void OnJoinedRoom() {
        Debug.Log("joined room. checking for previous players!");

        //foreach (PhotonPlayer ply in PhotonNetwork.otherPlayers) {
        //    int playerId = ply.ID;
        //    Debug.Log("spawning " + playerId);

        //    spawnPlayer(playerId);
        //}

        sendPlayerInitDataRequest();
    }

    void OnPhotonPlayerConnected(PhotonPlayer player) {
        Debug.Log("playerJoined");

        //int playerId = player.ID;
        //spawnPlayer(playerId);
        sendPlayerInitDataRequest();
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer player) {
        Debug.Log("player left!");
    }

    int nextPlayerNumber = 0;
    private void spawnPlayer(int playerId, int colorChoiceIndex) {
        if (spawnedPlayersTable.Contains(playerId)) {
            // skip it
            Debug.Log("skipping the repeat spawning of network playerid: " + playerId + " with colorChoiceIndex: " + colorChoiceIndex);
            return;
        }

        Debug.Log("new spawn of colorChoiceIndex: " + colorChoiceIndex);

        spawnedPlayersTable.Add(playerId);
        nextPlayerNumber++;
        Vector3 spawnpoint = spawnpointsPrefab.transform.GetChild(count).transform.position;
        if (count < 8) {
            count++;
        } else {
            Debug.Log("too many players counter reset to 0");
            count = 0;
        }
        GameObject playerGameObject = Instantiate(playerPrefab, spawnpoint, Quaternion.identity) as GameObject;

        // initialize the player values
        playerClass playerScript = playerGameObject.GetComponent<playerClass>();
        playerScript.setNetworkPlayerId(playerId);
        playerScript.IS_LOCALLY_CONTROLLED = false;
        playerScript.PlayerNumber = nextPlayerNumber;
        playerScript.setColor(colorChoiceIndex);
        playerScript.teamNum = nextPlayerNumber;

        playerScript.paintUnderMe(10);
    }

    private void OnPhotonNetworkEvent(byte eventcode, object content, int senderid) {
        // everything is in json format

        PhotonPlayer sender = PhotonPlayer.Find(senderid);  // who sent this?

        byte[] byteContent;
        string contentStringJson;

        switch (eventcode) {
            case Constants.PLAYER_DATA_INIT_EVENT_CODE:
                byteContent = (byte[])content;
                contentStringJson = Encoding.UTF8.GetString(byteContent);
                playerDataInitEvent playerInitEvent = playerDataInitEvent.CreateFromJSON(contentStringJson);

                spawnPlayer(sender.ID, playerInitEvent.startingColor);
                break;
        }
    }
}
