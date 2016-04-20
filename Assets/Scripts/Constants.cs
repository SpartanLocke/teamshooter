using UnityEngine;
using System.Collections;

public class Constants : MonoBehaviour {
    // const means static in c# !!!
    public const byte PLAYER_INPUT_EVENT_CODE = 0;
    public const byte PLAYER_COLOR_CHANGE_EVENT_CODE = 1;
    public const byte PLAYER_TAUNT_EVENT_CODE = 2;

    // http://stackoverflow.com/questions/5142349/declare-a-const-array
    public static readonly Color[] playerColorChoices = new Color[] { Color.red, Color.blue, Color.cyan, Color.green };
}
