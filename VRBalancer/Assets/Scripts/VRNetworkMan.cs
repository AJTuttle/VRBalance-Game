using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VRNetworkMan : MonoBehaviourPunCallbacks {

    public Transform sphere;
    public Transform stage;

    PhotonView pv;

    // Start is called before the first frame update
    void Start() {
        //sphere.TransferOwnership(PhotonNetwork.LocalPlayer);
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update() {

    }

    public override void OnJoinedRoom() {


        //sphere.RequestOwnership();
        //sphere.TransferOwnership(PhotonNetwork.LocalPlayer);

        GameObject obj = PhotonNetwork.Instantiate("Playa", new Vector3(0, 0, 0), Quaternion.identity);
        Transform t = obj.transform;
        t.parent = sphere.transform;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;


        GameObject objS = PhotonNetwork.Instantiate("StageLoc", new Vector3(0, 0, 0), Quaternion.identity);
        Transform ts = objS.transform;
        ts.parent = stage.transform;
        ts.localPosition = Vector3.zero;
        ts.localRotation = Quaternion.identity;


    }

    [PunRPC]
    void spawnRock(Vector3 pos) {
        Debug.Log("rock spawn");
        GameObject obj = PhotonNetwork.Instantiate("Rock", pos, Quaternion.identity);
        obj.GetComponent<Rigidbody>().isKinematic = false;
    }


    public void startLevel(int levelID) {
        pv.RPC("loadLevel", RpcTarget.Others, levelID);
    }

    public void restart() {
        pv.RPC("restart", RpcTarget.Others);
        GameObject[] objs = GameObject.FindGameObjectsWithTag("rock");
        foreach(GameObject obj in objs) {
            PhotonNetwork.Destroy(obj);
        }
    }
}
