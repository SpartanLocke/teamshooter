using UnityEngine;
using System.Collections;

public class PlayerUIInputController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
    // leave the photon lobby
    // go back to the main controller scene
    public void onExitButonClicked() {
        Debug.Log("player clicked exit button");
        if (PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
            PhotonNetwork.LeaveRoom();
        }
    }

    void OnLeftRoom() {
        Debug.Log("left the room!");
        Application.Quit();
    }
}
