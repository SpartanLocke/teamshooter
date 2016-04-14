using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ControllerMenuUiController : MonoBehaviour {
    public static string lobbyName = "";

    public void onEditTextChange(string currentLobby) {
        lobbyName = currentLobby;
    }

    public void onJoinButtonPressed() {
        ConnectAndJoinRandom.setJoinRandomRooms(true);
        SceneManager.LoadScene("dual stick controller");
    }
}
