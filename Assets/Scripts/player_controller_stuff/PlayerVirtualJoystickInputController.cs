using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using CnControls;

public class PlayerVirtualJoystickInputController : MonoBehaviour {
    public GameObject tauntButtonInside, tauntButtonOutside;

    private int STARTING_PLAYER_COLOR;
    private bool sentInitData = false;
    private Image playerColorInside, playerColorOutside;

    void Awake() {
        PhotonNetwork.OnEventCall += this.OnPhotonNetworkEvent;
        STARTING_PLAYER_COLOR = MenuUiController.PLAYER_COLOR_CHOICE_VALUE;

        Debug.Log("start color: " + STARTING_PLAYER_COLOR);
    }

    void Start() {
        // setting of colors
        playerColorInside = tauntButtonInside.GetComponent<Image>();
        playerColorOutside = tauntButtonOutside.GetComponent<Image>();

        playerColorInside.color = Color.white;
        playerColorOutside.color = Color.white;

        playerColorInside.color = Constants.lightColors[STARTING_PLAYER_COLOR];
        setControllerColor(STARTING_PLAYER_COLOR);
    }

    // Update is called once per frame
    void Update () {
        if (PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
            float left_x = CnInputManager.GetAxis("HorizontalP1");
            float left_y = CnInputManager.GetAxis("VerticalP1");

            float right_x = CnInputManager.GetAxis("HorizontalShootP1");
            float right_y = CnInputManager.GetAxis("VerticalShootP1");

            sendControllerNetworkInput(left_x, left_y, right_x, right_y);

            // support the onscreen and joystic taunt
            if (CnInputManager.GetButtonDown("Taunt") || Input.GetButton("FireP1")) {
                Debug.Log("taunt pressed");
                sendPlayerTaunt();
            }

            if (!sentInitData) {
                sendPlayerInitializationData();
                sentInitData = true;
            }
        }
    }

    private void sendPlayerInitializationData() {
        // send the init data via reliable transmission
        playerDataInitEvent dataInitEvent = new playerDataInitEvent(STARTING_PLAYER_COLOR);
        byte[] content = dataInitEvent.getBytes();
        bool reliable = true;

        PhotonNetwork.RaiseEvent(Constants.PLAYER_DATA_INIT_EVENT_CODE, content, reliable, null);
        Debug.Log("send init data");
    }

    private void sendControllerNetworkInput(float left_x, float left_y, float right_x, float right_y) {
        PlayerInputEvent inputEvent = new PlayerInputEvent(left_x, left_y, right_x, right_y);

        // player input
        byte[] content = inputEvent.getBytes();
        sendNetworkEvent(Constants.PLAYER_INPUT_EVENT_CODE, content);
    }

    private void sendPlayerTaunt() {
        // there's no data being sent, just the taunting
        sendNetworkEvent(Constants.PLAYER_TAUNT_EVENT_CODE, new byte[1]);
    }

    private void setControllerColor(int colorChoiceIndex) {
        playerColorOutside.color = Constants.lightColors[colorChoiceIndex];
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

                if (colorChangeEvent.sendingPlayerId == PhotonNetwork.player.ID) {
                    //Color newPlayerColor = Constants.lightColors[colorChangeEvent.newPlayerColor];
                    //setControllerColor(newPlayerColor);

                    setControllerColor(colorChangeEvent.newPlayerColor);
                    //Debug.Log(newPlayerColor);
                }
                break;

            case Constants.SERVER_REQUEST_INIT_DATA_EVENT_CODE:
                sendPlayerInitializationData();
                break;
        }
    }
}
