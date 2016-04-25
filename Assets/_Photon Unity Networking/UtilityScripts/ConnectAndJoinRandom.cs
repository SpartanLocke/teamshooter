using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// This script automatically connects to Photon (using the settings file),
/// tries to join a random room and creates one if none was found (which is ok).
/// </summary>
public class ConnectAndJoinRandom : Photon.MonoBehaviour {
    private byte Version = 2;

    /// <summary>if we don't want to connect in Start(), we have to "remember" if we called ConnectUsingSettings()</summary>
    private bool ConnectInUpdate = true;

    public string defaultLobbyName = Environment.UserName;

    // only try to join random rooms if we haven't left one already
    private static bool attemptJoinRandomRoom = true;

    public virtual void Start() {
        Debug.Log("start in connJoinRandom");
        ConnectInUpdate = true;
        PhotonNetwork.autoJoinLobby = false;    // we join randomly. always. no need to join a lobby to get the list of rooms.
    }

    public virtual void Update() {
        if (ConnectInUpdate && !PhotonNetwork.connected) {
            Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");

            ConnectInUpdate = false;
            string connectionString = "";
            if (ControllerMenuUiController.lobbyName == "") {
                connectionString = Version + "." + defaultLobbyName;
            } else {
                connectionString = Version + "." + ControllerMenuUiController.lobbyName;
            }

            Debug.Log("connecting to lobby: " + connectionString);
            PhotonNetwork.ConnectUsingSettings(connectionString);
        }
    }

    // below, we implement some callbacks of PUN
    // you can find PUN's callbacks in the class PunBehaviour or in enum PhotonNetworkingMessage
    public virtual void OnConnectedToMaster() {
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        if (attemptJoinRandomRoom) {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public virtual void OnJoinedLobby() {
        Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
        if (attemptJoinRandomRoom) {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public virtual void OnPhotonRandomJoinFailed() {
        Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 20}, null);");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = 20 }, null);
    }

    // the following methods are implemented to give you some context. re-implement them as needed.
    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause) {
        Debug.LogError("Cause: " + cause);
    }

    public void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
    }

    public void OnLeftRoom() {
        Debug.Log("left room");
        //attemptJoinRandomRoom = false;
    }

    public static void setJoinRandomRooms(bool joinRandoms) {
        Debug.Log("join random room?: " + joinRandoms);
        attemptJoinRandomRoom = joinRandoms;
    }
}
