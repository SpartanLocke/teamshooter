using UnityEngine;
using System.Collections;

public class MenuButtonFunctions : MonoBehaviour {
    public GameObject lobbySelectButtons;
    public GameObject menuButtons;
public void openPlayButtons()
    {
        menuButtons.SetActive(false);
        lobbySelectButtons.SetActive(true);
    }
public void backFromLobby()
    {
        menuButtons.SetActive(true);
        lobbySelectButtons.SetActive(false);
    }
}
