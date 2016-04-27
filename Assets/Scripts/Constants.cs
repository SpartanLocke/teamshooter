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
      HSBColor.ToColor(new HSBColor(0.0f/8.0f, 1f, .2f, 1f)),
      HSBColor.ToColor(new HSBColor(.65f/8.0f, 1f, .2f, 1f)),
      HSBColor.ToColor(new HSBColor(1.5f/8.0f, 1f, .2f, 1f)),
      HSBColor.ToColor(new HSBColor(2.57f/8.0f, 1f, .2f, 1f)),
      HSBColor.ToColor(new HSBColor(4.0f/8.0f, 1f, .2f, 1f)),
      HSBColor.ToColor(new HSBColor(4.9f/8.0f, 1f, .2f, 1f)),
      HSBColor.ToColor(new HSBColor(6.1f/8.0f, 1f, .2f, 1f)),
      HSBColor.ToColor(new HSBColor(7.35f/8.0f, 1f, .2f, 1f))
    };
    public static readonly Color[] lightColors = new Color[]
    {
      HSBColor.ToColor(new HSBColor(0.0f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(.65f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(1.5f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(2.57f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(4.0f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(4.9f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(6.1f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(7.35f/8.0f, 1f, 1f, 1f))

};
    static float asdf = 0.25f;
    public static readonly Color[] paintColors = new Color[]
    {
      
      HSBColor.ToColor(new HSBColor(0.0f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(.65f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(1.5f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(2.57f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(4.0f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(4.9f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(6.1f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(7.35f/8.0f, 1f, 1f, 1f))

      /*HSBColor.ToColor(new HSBColor(2.57f/8.0f, 1f, .2f, 1f)),
      HSBColor.ToColor(new HSBColor(2.57f/8.0f, 1f, .4f, 1f)),
      HSBColor.ToColor(new HSBColor(2.57f/8.0f, 1f, .6f, 1f)),
      HSBColor.ToColor(new HSBColor(2.57f/8.0f, 1f, .8f, 1f)),
      HSBColor.ToColor(new HSBColor(2.57f/8.0f, .8f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(2.57f/8.0f, .6f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(2.57f/8.0f, .4f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(2.57f/8.0f, .2f, 1f, 1f))*/
    };
    public static readonly Color[] firedColors = new Color[]
   {
      HSBColor.ToColor(new HSBColor(0.0f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(.65f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(1.5f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(2.57f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(4.0f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(4.9f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(6.1f/8.0f, 1f, 1f, 1f)),
      HSBColor.ToColor(new HSBColor(7.35f/8.0f, 1f, 1f, 1f))
   };
}
