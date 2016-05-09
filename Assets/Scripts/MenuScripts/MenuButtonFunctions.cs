using UnityEngine;
using System.Collections;

public class MenuButtonFunctions : MonoBehaviour {
    public GameObject lobbySelectButtons;
    public GameObject menuButtons;
    public GameObject UI;
    public GameObject credits;
    public GameObject howToPlay;
    public GameObject colorSelect;
    public AudioSource buttonSound;
    void Start()
    {
        buttonSound = gameObject.GetComponent<AudioSource>();
    }
    public void openPlayButtons()
    {
        menuButtons.SetActive(false);
        lobbySelectButtons.SetActive(true);
        buttonSound.Play();
    }
    public void backFromLobby()
    {
        menuButtons.SetActive(true);
        lobbySelectButtons.SetActive(false);
        buttonSound.Play();
    }
    public void toColorSelect()
    {
        UI.SetActive(false);
        colorSelect.SetActive(true);
        buttonSound.Play();
    }
    
    public void backFromColorSelect()
    {
        UI.SetActive(true);
        colorSelect.SetActive(false);
        buttonSound.Play();
    }
    public void toCredits()
    {
        UI.SetActive(false);
        credits.SetActive(true);
        buttonSound.Play();
    }

    public void backFromCredits()
    {
        UI.SetActive(true);
        credits.SetActive(false);
        buttonSound.Play();
    }
    public void toHowToPlay()
    {
        UI.SetActive(false);
        howToPlay.SetActive(true);
        buttonSound.Play();
    }

    public void backFromHowToPlay()
    {
        UI.SetActive(true);
        howToPlay.SetActive(false);
        buttonSound.Play();
    }

    public void disableAll()
    {
        //UI.SetActive(false);
        colorSelect.SetActive(false);
        lobbySelectButtons.SetActive(false);
        menuButtons.SetActive(false);
    }

    public void load(string asdf)
    {
        StartCoroutine(LoadLevel(asdf));
    }

    IEnumerator LoadLevel(string levelName)
    {
        yield return StartCoroutine(CameraFade.GetCameraFade().WaitForCameraFade(true));
        Application.LoadLevel(levelName);
    }

}
