using UnityEngine;
using System.Collections;

public class PlayerInputController : MonoBehaviour {
    //private string[,] axes = new string[,] { {"HorizontalP1", "HorizontalP2", "HorizontalP3", "HorizontalP4" },
    //    { "VerticalP1", "VerticalP2", "VerticalP3", "VerticalP4" },
    //    {"FireP1","FireP2", "FireP3", "FireP4" },
    //    {"HorizontalShootP1", "HorizontalShootP2", "HorizontalShootP3", "HorizontalShootP4" },
    //    { "VerticalShootP1", "VerticalShootP2", "VerticalShootP3", "VerticalShootP4" }};

    // Update is called once per frame
    void Update() {
        bool shooting = Input.GetButton("FireP1");
        Vector3 axisInputLeft = (new Vector3(Input.GetAxis("HorizontalP1"), Input.GetAxis("VerticalP1"))).normalized;
        Vector3 axisInputRight = (new Vector3(Input.GetAxis("HorizontalShootP1"), Input.GetAxis("VerticalShootP1"))).normalized;
        bool axisShootButton = Input.GetButton("FireP1");

        if (PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
            sendControllerInput(axisInputLeft, axisInputRight, axisShootButton);
        }
    }

    private void sendControllerInput(Vector3 axisInputLeft, Vector3 axisInputRight, bool shooting) {
        // create a player input event
        PlayerInputEvent inputEvent = new PlayerInputEvent(axisInputLeft.x, axisInputLeft.y, axisInputRight.x, axisInputRight.y, shooting);

        // player input
        byte eventCode = 0;
        byte[] content = inputEvent.getBytes();

        // todo: is false the best here? (its the fastest)
        bool reliable = false;

        // todo: use RaiseEventOptions?
        PhotonNetwork.RaiseEvent(eventCode, content, reliable, null);

        Debug.Log("send input");
    }
}
