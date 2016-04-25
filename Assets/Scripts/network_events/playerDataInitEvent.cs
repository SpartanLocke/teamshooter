using UnityEngine;
using System.Collections;
using System.Text;

// for all the data a player sends to the server on start
[System.Serializable]
public class playerDataInitEvent {
    public int startingColor;

    public playerDataInitEvent(int _startingColor) {
        startingColor = _startingColor;
    }

    public static playerDataInitEvent CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<playerDataInitEvent>(jsonString);
    }

    public byte[] getBytes() {
        string thisJson = JsonUtility.ToJson(this);
        return Encoding.UTF8.GetBytes(thisJson);
    }
}
