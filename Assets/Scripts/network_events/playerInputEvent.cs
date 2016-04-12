using UnityEngine;
using System.Collections;
using System.Text;

// http://docs.unity3d.com/Manual/JSONSerialization.html
// everything is in json format
[System.Serializable]
public class PlayerInputEvent {
    public float left_x, left_y;
    public float right_x, right_y;
    public bool shoot;

    public PlayerInputEvent(float left_input_x, float left_input_y, float right_input_x, float right_input_y, bool input_shoot) {
        left_x = left_input_x;
        left_y = left_input_y;

        right_x = right_input_x;
        right_y = right_input_y;

        shoot = input_shoot;
    }

    public static PlayerInputEvent CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<PlayerInputEvent>(jsonString);
    }

    public byte[] getBytes() {
        string thisJson = JsonUtility.ToJson(this);
        return Encoding.UTF8.GetBytes(thisJson);
    }
}
