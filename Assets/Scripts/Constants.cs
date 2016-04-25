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
       HSBColor.ToColor(new HSBColor(0.0f/10.0f, 1f, .2f, 1f)),
        HSBColor.ToColor(new HSBColor(2.0f/10.0f, 1f, .2f, 1f)),
        HSBColor.ToColor(new HSBColor(4.0f/10.0f, 1f, .2f, 1f)),
        HSBColor.ToColor(new HSBColor(6.0f/10.0f, 1f, .2f, 1f)),
        HSBColor.ToColor(new HSBColor(8.0f/10.0f, 1f, .2f, 1f)),
        HSBColor.ToColor(new HSBColor(10.0f/10.0f, 1f, .2f, 1f)),
        HSBColor.ToColor(new HSBColor(1.0f/10.0f, 1f, .2f, 1f)),
        HSBColor.ToColor(new HSBColor(3.0f/10.0f, 1f, .2f, 1f)),
        HSBColor.ToColor(new HSBColor(5.0f/10.0f, 1f, .2f, 1f)),
        HSBColor.ToColor(new HSBColor(7.0f/10.0f, 1f, .2f, 1f)),
        HSBColor.ToColor(new HSBColor(9.0f/10.0f, 1f, .2f, 1f)) };
    public static readonly Color[] lightColors = new Color[]
    {
        
        HSBColor.ToColor(new HSBColor(0.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(2.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(4.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(6.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(8.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(10.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(1.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(3.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(5.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(7.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(9.0f/10.0f, 1f, 1f, 1f))
        
        
        


    /*new Color32(255,28,28,195),
    new Color32(0,128,255,195),
    new Color32(0,255,0,195),
    new Color32(255,165,0,195),
    new Color32(255,0,170,195),
    new Color32(170,0,255,195),
    new Color32(249,106,0,195),
    new Color32(199,0,63,195),
    new Color32(119,210,233,195),
    new Color32(239,254,75,195),
    new Color32(255,255,255,195),
    new Color32(152,152,152,195)*/
};
    public static readonly Color[] paintColors = new Color[]
    {
        HSBColor.ToColor(new HSBColor(0.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(2.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(4.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(6.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(8.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(10.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(1.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(3.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(5.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(7.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(9.0f/10.0f, 1f, 1f, 1f))
    };
    public static readonly Color[] firedColors = new Color[]
   {
        HSBColor.ToColor(new HSBColor(0.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(2.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(4.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(6.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(8.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(10.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(1.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(3.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(5.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(7.0f/10.0f, 1f, 1f, 1f)),
        HSBColor.ToColor(new HSBColor(9.0f/10.0f, 1f, 1f, 1f)) };
}
