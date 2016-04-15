using UnityEngine;
using System.Collections;
using CnControls;

public class PlayerVirtualJoystickInputController : MonoBehaviour {
	// Update is called once per frame
	void Update () {
        float left_x = CnInputManager.GetAxis("HorizontalP1");
        float left_y = CnInputManager.GetAxis("VerticalP1");

        float right_x = CnInputManager.GetAxis("HorizontalShootP1");
        float right_y = CnInputManager.GetAxis("VerticalShootP1");

        //Debug.Log(left_x + " " + left_y + " " + right_x + " " + right_y + " ");

        if (PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
            sendControllerNetworkInput(left_x, left_y, right_x, right_y);
        }
    }

    private void sendControllerNetworkInput(float left_x, float left_y, float right_x, float right_y) {
        PlayerInputEvent inputEvent = new PlayerInputEvent(left_x, left_y, right_x, right_y);

        // player input
        byte eventCode = 0;
        byte[] content = inputEvent.getBytes();

        // todo: is false the best here? (its the fastest)
        bool reliable = false;

        // todo: use RaiseEventOptions?
        PhotonNetwork.RaiseEvent(eventCode, content, reliable, null);
    }
}
