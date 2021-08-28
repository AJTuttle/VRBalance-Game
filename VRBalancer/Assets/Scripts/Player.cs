
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public GameManager gm;

    public Transform pivot;
    public GameObject sphere;
    public SphereFollower sf;
    public Transform stage;

    public Rod rod;

    void Update() {

        if (rod.canTilt) tiltWorld();

        if (Vector3.Distance(sphere.transform.position, stage.position) > 150) gm.restart(); 

    }

    void tiltWorld() {

        Vector3 rotate = -new Vector3(0, InputMan.ThumbstickX(Side.Left), 0) * 0.4f;

        pivot.Rotate(rotate);


        Vector2 controllerTilt = new Vector2(InputMan.ThumbstickX(Side.Right), -InputMan.ThumbstickY(Side.Right));
        Vector2 worldTiltX = new Vector2(Mathf.Sin(Mathf.Deg2Rad * pivot.transform.eulerAngles.y), Mathf.Cos(Mathf.Deg2Rad * pivot.transform.eulerAngles.y));
        Vector2 worldTiltZ = new Vector2(-Mathf.Cos(Mathf.Deg2Rad * pivot.transform.eulerAngles.y), Mathf.Sin(Mathf.Deg2Rad * pivot.transform.eulerAngles.y));

        //Debug.Log(controllerTilt + "     " + worldTilt);

        float xTilt = Vector2.Dot(controllerTilt, worldTiltX);
        float zTilt = Vector2.Dot(controllerTilt, worldTiltZ);

        //Debug.Log(Vector2.Dot(controllerTilt, worldTiltX));



        pivot.Rotate(new Vector3(xTilt, 0, zTilt) * 0.3f);

    }

    public void resetPos() {

        sphere.transform.position = sf.offset;
        sphere.GetComponent<Rigidbody>().isKinematic = true;
        sphere.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        pivot.transform.position = Vector3.zero;
        pivot.transform.rotation = Quaternion.identity;
        stage.transform.position = Vector3.zero;
        stage.transform.rotation = Quaternion.identity;

    }
}

