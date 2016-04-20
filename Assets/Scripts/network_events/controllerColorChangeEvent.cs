using UnityEngine;
using System.Collections;
using System.Text;

// http://docs.unity3d.com/Manual/JSONSerialization.html
// everything is in json format
public class controllerColorChangeEvent {
    public Color newPlayerColor;
    public int sendingPlayerId;

    public controllerColorChangeEvent(Color _newPlayerColor, int _playerId) {
        newPlayerColor = _newPlayerColor;
        sendingPlayerId = _playerId;
    }

    public static controllerColorChangeEvent CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<controllerColorChangeEvent>(jsonString);
    }

    public byte[] getBytes() {
        string thisJson = JsonUtility.ToJson(this);
        return Encoding.UTF8.GetBytes(thisJson);
    }
}
