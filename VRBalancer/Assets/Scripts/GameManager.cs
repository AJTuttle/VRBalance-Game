
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;

public class GameManager : MonoBehaviour
{

    public Player player;
    //public UIManager uiMan;

    public Transform stageParent;
    public GameObject levelSelectPrefab;
    VRNetworkMan network;

    public GameObject[] stagePrefabs;

    public Rod rod;

    public Transform rodParent;
    public Transform cameraRig;

    public int currentLevel;

    

    void Start()
    {

        if (!PlayerPrefs.HasKey("level")) PlayerPrefs.SetInt("level", 1);
        //PlayerPrefs.SetInt("level", 5);

        network = GetComponent<VRNetworkMan>();
        //uiMan.updateLocks(PlayerPrefs.GetInt("level"));


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || InputMan.MenuButtonDown(Side.Right)) restart();
        if (Input.GetKeyDown(KeyCode.J) || InputMan.MainThumbstickPressDown()) {

            //levelSelect(1);
            unparentRod();
        }
    }

    public void restart() {
        player.resetPos();

        network.restart();

        rod.restart();
        Destroy(stageParent.GetChild(0).gameObject);
        GameObject ls = GameObject.Instantiate(levelSelectPrefab, stageParent);

        ls.transform.SetSiblingIndex(0);

        //Debug.Log(ls);


        //uiMan.updateLocks(PlayerPrefs.GetInt("level"));

    }

    public void unparentRod() {

        rodParent.parent = cameraRig;
        rodParent.position = new Vector3(0, 0.03f, 0.3f);
        rodParent.rotation = Quaternion.identity;

    }

    public void levelSelect(int level) {

        if (level == 6) {
            restart();
            return;
        }

        

        GameObject[] objs = GameObject.FindGameObjectsWithTag("rock");
        foreach (GameObject obj in objs) {
            PhotonNetwork.Destroy(obj);
        }

        network.startLevel(level);

        if (PlayerPrefs.GetInt("level") < level) PlayerPrefs.SetInt("level", level);

        currentLevel = level;
        player.resetPos();
        player.sphere.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        rod.startTimer();
        rod.restart();

        GameObject stage = stagePrefabs[level - 1];

        Destroy(stageParent.GetChild(0).gameObject);
        GameObject newStage = GameObject.Instantiate(stage, stageParent);
        newStage.transform.SetSiblingIndex(0);



    }

    public void EnableVR()
    {
        StartCoroutine(doEnableVR());
        //var x = XRSettings.supportedDevices;
        //foreach (string name in x)
        //{
        //    Debug.Log(name);
        //}
    }
    IEnumerator doEnableVR()
    {
        Debug.Log("Enabling VR");        
        while (XRSettings.loadedDeviceName != "Oculus")
        {
            XRSettings.LoadDeviceByName("Oculus");
            Debug.Log("LoadedDeviceName: :" + XRSettings.loadedDeviceName);
            yield return null;
        }
        XRSettings.enabled = true;
    }

}

