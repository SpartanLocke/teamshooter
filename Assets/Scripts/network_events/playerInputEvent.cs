using UnityEngine;
using System.Collections;
using System.Text;

// http://docs.unity3d.com/Manual/JSONSerialization.html
// everything is in json format
[System.Serializable]
public class PlayerInputEvent {
    public float x, y;
    public bool shoot;

    public PlayerInputEvent(float input_x, float input_y, bool input_shoot) {
        x = input_x;
        y = input_y;
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
