using UnityEngine;
using System.Collections;
using System.Text;
using CnControls;

public class PlayerVirtualJoystickInputController : MonoBehaviour {
	// Update is called once per frame
	void Update () {
        float left_x = CnInputManager.GetAxis("HorizontalP1");
        float left_y = CnInputManager.GetAxis("VerticalP1");

        float right_x = CnInputManager.GetAxis("HorizontalShootP1");
        float right_y = CnInputManager.GetAxis("VerticalShootP1");

        if (PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
            sendControllerNetworkInput(left_x, left_y, right_x, right_y);
        }
    }

    private void sendControllerNetworkInput(float left_x, float left_y, float right_x, float right_y) {
        PlayerInputEvent inputEvent = new PlayerInputEvent(left_x, left_y, right_x, right_y);

        // player input
        byte[] content = inputEvent.getBytes();

        sendNetworkEvent(Constants.PLAYER_INPUT_EVENT_CODE, content);
    }

    private void sendPlayerTaunt() {
        // code 2
        // there's no data being sent, just the taunting
        sendNetworkEvent(Constants.PLAYER_TAUNT_EVENT_CODE, new byte[1]);
    }

    private void sendNetworkEvent(byte eventCode, byte[] content) {
        // todo: is false the best here? (its the fastest)
        bool reliable = false;

        // todo: use RaiseEventOptions?
        PhotonNetwork.RaiseEvent(eventCode, content, reliable, null);
    }

    // handle events
    private void OnPhotonNetworkEvent(byte eventcode, object content, int senderid) {
        // everything is in json format
        switch (eventcode) {
            case Constants.PLAYER_COLOR_CHANGE_EVENT_CODE:
                byte[] byteContent = (byte[])content;
                string contentStringJson = Encoding.UTF8.GetString(byteContent);
                controllerColorChangeEvent colorChangeEvent = controllerColorChangeEvent.CreateFromJSON(contentStringJson);

                if (colorChangeEvent.sendingPlayerId != PhotonNetwork.player.ID) {
                    setControllerColor(colorChangeEvent.newPlayerColor);
                }


                break;
        }
    }

    private void setControllerColor(Color someColor) {
        
    }
}
