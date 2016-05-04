using UnityEngine;
using System.Collections;
using System.Text;

// http://docs.unity3d.com/Manual/JSONSerialization.html
// everything is in json format
[System.Serializable]
public class controllerPerformVibrateEvent {
    public int sendingPlayerId;

    public controllerPerformVibrateEvent(int _playerId) {
        sendingPlayerId = _playerId;
    }

    public static controllerPerformVibrateEvent CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<controllerPerformVibrateEvent>(jsonString);
    }

    public byte[] getBytes() {
        string thisJson = JsonUtility.ToJson(this);
        return Encoding.UTF8.GetBytes(thisJson);
    }
}
