using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TabletEventMan : MonoBehaviour
{

    PhotonView pv;
    public Transform pivot;

    TabletGameManager gm;

    GameObject currentLevel;
    public GameObject waitingScreen;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        gm = GetComponent<TabletGameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void rockEvent(Vector3 pos) {

        pv.RPC("spawnRock", RpcTarget.Others, pos);
    }


    [PunRPC]
    void loadLevel(int level) {

        Debug.Log("load " + level);

        if (currentLevel != null) currentLevel.SetActive(false);
        currentLevel = pivot.GetChild(level - 1).gameObject;
        currentLevel.SetActive(true);

        waitingScreen.SetActive(false);

        gm.startLevel();
    }

    [PunRPC]
    void restart() {
        if (currentLevel != null) currentLevel.SetActive(false);
        waitingScreen.SetActive(true);


        

    }
}
