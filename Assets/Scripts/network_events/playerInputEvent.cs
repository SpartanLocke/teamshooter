using UnityEngine;
using System.Collections;

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
}
