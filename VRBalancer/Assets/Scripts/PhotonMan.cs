using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class PhotonMan : MonoBehaviourPunCallbacks {
    public string roomNameInput;
    int maxNumInRoom = 8;
    // Start is called before the first frame update
    void Start() {
        connectToServerClicked();
    }

    // Update is called once per frame
    void Update() {

    }
    public void connectToServerClicked() {
        PhotonNetwork.ConnectUsingSettings(); //settings can be changed in the inspector: assets> photon > PhotonUnityNewtworking > PhotonServerSettings
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to photon");
        RoomOptions options = new RoomOptions();
        options.IsVisible = false; // public room?
        options.MaxPlayers = (byte)maxNumInRoom;
        PhotonNetwork.JoinOrCreateRoom(roomNameInput, options, TypedLobby.Default);
    }
    public override void OnDisconnected(DisconnectCause cause) {
    }
    public override void OnJoinedRoom() {
        // we can start sending messages to people in the room. we cans start doing things
        Debug.Log("connected to room");
        //PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 0), Quaternion.identity);



    }

}