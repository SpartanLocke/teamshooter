using UnityEngine;
using System.Collections;

public class PlayerNetworkStatusHandler : MonoBehaviour {
    private Color[] colorChoices = new Color[] {Color.red, Color.blue, Color.cyan, Color.green};
    private gridController gridController;

    public GameObject playerPrefab;

    void Start() {
        gridController = GameObject.FindGameObjectWithTag("gridGameObject").GetComponent<gridController>();
    }

    public void OnJoinedRoom() {
        Debug.Log("joined room. checking for previous players!");

        foreach (PhotonPlayer ply in PhotonNetwork.otherPlayers) {
            int playerId = ply.ID;
            Debug.Log("spawning " + playerId);

            spawnPlayer(playerId);
        }
    }

    void OnPhotonPlayerConnected(PhotonPlayer player) {
        Debug.Log("playerJoined");

        int playerId = player.ID;
        spawnPlayer(playerId);
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
        float randomX = Random.Range(1, gridController.width - 1);
        float randomY = Random.Range(1, gridController.height - 1);

        GameObject playerGameObject = Instantiate(playerPrefab, new Vector3(randomX, randomY, 0), Quaternion.identity) as GameObject;

        // initialize the player values
        // TODO: get the grid object and randomly place inside it
        playerClass playerScript = playerGameObject.GetComponent<playerClass>();
        playerScript.setNetworkPlayerId(playerId);
        playerScript.normal = colorChoices[Random.Range(0, (colorChoices.GetLength(0)))];
    }
}
