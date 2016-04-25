using UnityEngine;
using System.Collections;

public class MenuButtonFunctions : MonoBehaviour {
    public GameObject lobbySelectButtons;
    public GameObject menuButtons;
    public GameObject UI;
    public GameObject colorSelect;
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
    public void toColorSelect()
    {
        UI.SetActive(false);
        colorSelect.SetActive(true);
    }
    public void backFromColorSelect()
    {
        UI.SetActive(true);
        colorSelect.SetActive(false);
    }
}
