using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUiController : MonoBehaviour {
    public static int PLAYER_COLOR_CHOICE_VALUE = 0;
    public static string lobbyName = "";

    public InputField lobbyNameInputField;

    void Start() {
        //PhotonNetwork.OnEventCall += this.OnPhotonNetworkEvent;
    }

    public void onEditTextChange() {
        MenuUiController.lobbyName = lobbyNameInputField.text;
        Debug.Log("lobby set to: " + lobbyName);
    }

    public void onStartAsServerButtonPressed() {
        ConnectAndJoinRandom.setJoinRandomRooms(true);
        Debug.Log("started as server");
        //SceneManager.LoadScene("main");
        //PhotonNetwork.OnEventCall -= this.OnPhotonNetworkEvent;
        StartCoroutine(LoadLevel("main"));
    }

    IEnumerator LoadLevel(string levelName) {
        yield return StartCoroutine(CameraFade.GetCameraFade().WaitForCameraFade(true));
        SceneManager.LoadScene(levelName);
    }

    public void onControllerColorValueChanged(int value) {
        Debug.Log("onColorValueChanged " + value);
        PLAYER_COLOR_CHOICE_VALUE = value;
    }

    public void onDropdownValueChanged(int _x) {
        Debug.Log("chose color " + _x);
        PLAYER_COLOR_CHOICE_VALUE = _x;
    }

    public void onJoinButtonPressed() {
        ConnectAndJoinRandom.setJoinRandomRooms(true);
        SceneManager.LoadScene("dual stick controller");
    }

    public void onBackButtonPressed() {
        SceneManager.LoadScene("controller menu");
    }

    // handle events
    private void OnPhotonNetworkEvent(byte eventcode, object content, int senderid) {
        //Debug.Log("got networking event inside the menu!");
    }
}
