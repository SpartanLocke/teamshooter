using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Constants : MonoBehaviour {
    // const means static in c# !!!
    public const byte PLAYER_INPUT_EVENT_CODE = 0;
    public const byte PLAYER_COLOR_CHANGE_EVENT_CODE = 1;
    public const byte PLAYER_TAUNT_EVENT_CODE = 2;
    public const byte PLAYER_DATA_INIT_EVENT_CODE = 3;
    public const byte SERVER_REQUEST_INIT_DATA_EVENT_CODE = 4;


    // The player chooses their color from this list
    public static List<string> playerColorChoiceStrings = new List<string>() { "red", "blue", "green", "yellow" };

    // http://stackoverflow.com/questions/5142349/declare-a-const-array
    public static readonly Color[] playerColorChoices = new Color[] {
       new Color32(77 ,0,0,195),
        new Color32(0,38,77,195),
        new Color32(0, 77, 0,195),
        new Color32(77, 50, 0,195) };
    public static readonly Color[] lightColors = new Color[]
    {
        new Color32(255,28,28,195),
        new Color32(0,128,255,195),
        new Color32(0,255,0,195),
        new Color32(255,165,0,195) };
    public static readonly Color[] paintColors = new Color[]
    {
        new Color32(255,28,28,195),
        new Color32(0,128,255,195),
        new Color32(0,255,0,195),
        new Color32(255,165,0,195) };
    public static readonly Color[] firedColors = new Color[]
   {
        new Color32(50,0,0,90),
        new Color32(0,72,143,90),
        new Color32(50,89,0,90),
        new Color32(135,101,26,90) };
}
