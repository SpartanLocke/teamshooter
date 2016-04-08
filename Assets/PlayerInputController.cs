using UnityEngine;
using System.Collections;

public class PlayerInputController : MonoBehaviour {

    // Update is called once per frame
    void Update() {
        bool shooting = false;
        Vector3 axisInput = (new Vector3(Input.GetAxis("HorizontalP1"), Input.GetAxis("VerticalP1"))).normalized;

        if (PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
            sendControllerInput(axisInput, shooting);
        }
    }

    private void sendControllerInput(Vector3 axisInput, bool shooting) {
        // create a player input event
        PlayerInputEvent inputEvent = new PlayerInputEvent(axisInput.x, axisInput.y, shooting);

        // player input
        byte eventCode = 0;
        byte[] content = inputEvent.getBytes();

        // todo: is false the best here? (its the fastest)
        bool reliable = false;

        // todo: use RaiseEventOptions?
        PhotonNetwork.RaiseEvent(eventCode, content, reliable, null);
    }
}
