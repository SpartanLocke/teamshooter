using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ControllerMenuUiController : MonoBehaviour {
    public static string lobbyName = "";

    public void onEditTextChange(string currentLobby) {
        lobbyName = currentLobby;
    }

    public void onJoinButtonPressed() {
        // going to the team options page first!
        //ConnectAndJoinRandom.setJoinRandomRooms(true);
        SceneManager.LoadScene("controller options screen");
    }

    public void onStartAsServerButtonPressed() {
        ConnectAndJoinRandom.setJoinRandomRooms(true);
        Debug.Log("started as server");
        SceneManager.LoadScene("main");
    }
}
