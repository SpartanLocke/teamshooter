using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class ControllerDropdownOptionsScript : MonoBehaviour {

    public static int PLAYER_COLOR_CHOICE_VALUE = 0;
    public Dropdown colorChoiceDropdown;

	void Start () {
        // reset the options
        colorChoiceDropdown.ClearOptions();

        // set our custom options
        colorChoiceDropdown.AddOptions(Constants.playerColorChoiceStrings);
	}

    public void onColorValueChanged(int value) {
        PLAYER_COLOR_CHOICE_VALUE = colorChoiceDropdown.value;
    }

    public void onDropdownValueChanged(int _x) {
        Debug.Log("chose color " + colorChoiceDropdown.value);
        PLAYER_COLOR_CHOICE_VALUE = colorChoiceDropdown.value;
    }

    public void onJoinButtonPressed() {
        ConnectAndJoinRandom.setJoinRandomRooms(true);
        SceneManager.LoadScene("dual stick controller");
    }

    public void onBackButtonPressed() {
        SceneManager.LoadScene("controller menu");
    }
}
