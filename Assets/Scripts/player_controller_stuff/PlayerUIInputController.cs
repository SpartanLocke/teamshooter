using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerUIInputController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
    // leave the photon lobby
    // go back to the main controller scene
    public void onExitButonClicked() {
        Debug.Log("player clicked exit button");
        if (PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
            ConnectAndJoinRandom.setJoinRandomRooms(false);
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        } else {
            Debug.Log("some networking status failed on exit button press");
            SceneManager.LoadScene("controller menu");
        }
    }

    void OnLeftRoom() {
        Debug.Log("left the room!");
        SceneManager.LoadScene("controller menu");
    }
}
