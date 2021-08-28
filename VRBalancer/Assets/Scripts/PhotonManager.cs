using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class PhotonManager : MonoBehaviourPunCallbacks
{
    public GameObject photonHUD;
    public Button connectButton;
    public InputField roomNameInput;
    int maxNumInRoom = 4;
    // Start is called before the first frame update
    void Start()
    { 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void connectToServerClicked()
    {
        PhotonNetwork.ConnectUsingSettings(); //settings can be changed in the inspector: assets> photon > PhotonUnityNewtworking > PhotonServerSettings
        //connectButton.interactable = false;
        photonHUD.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to photon");
        RoomOptions options = new RoomOptions();
        options.IsVisible = false; // public room?
        options.MaxPlayers = (byte) maxNumInRoom;
        PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, options, TypedLobby.Default);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        photonHUD.SetActive(true);
        //connectButton.interactable = true;
        //base.OnDisconnected(cause);
    }
    public override void OnJoinedRoom()
    {
        // we can start sending messages to people in the room. we cans start doing things
        Debug.Log("connected to room");
        PhotonNetwork.Instantiate("Player", new Vector3(0,0,0), Quaternion.identity);

    }
    
}
