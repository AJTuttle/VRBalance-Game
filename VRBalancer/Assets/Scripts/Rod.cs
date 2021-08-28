
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rod : MonoBehaviour
{

    public Transform follower;

    Stick stick;

    public Collider handle;

    Quaternion lastRot;

    public Text timerText;

    public Toggle isOnToggle;

    Quaternion target = Quaternion.identity;

    bool rot = false;
    public bool canTilt;
    public bool followController;
    public Transform controllerToFollow;

    public Material pressing;
    public Material notPressing;
    public GameObject[] modelObjs;


    private void Start() {
        stick = GetComponent<Stick>();
    }


    private void Update() {

        colorShift();

        if (canTilt) handle.enabled = true;
        else handle.enabled = false;

        if (followController) {
            stick.transform.up = controllerToFollow.position - stick.transform.position;
            followController = false;
        }

        if (isOnToggle.isOn) tilt();
    }

    void colorShift() {

        if (stick.get()) {
            foreach (GameObject obj in modelObjs) {
                obj.GetComponent<Renderer>().material = pressing;
            }
        }
        else {
            foreach (GameObject obj in modelObjs) {
                obj.GetComponent<Renderer>().material = notPressing;
            }
        }

    }

    void tilt() {
        Vector3 shaftAxis = transform.up;
        Vector3 crossShaftRight = Vector3.Cross(shaftAxis, Vector3.right);
        Quaternion newRot = Quaternion.LookRotation(crossShaftRight, shaftAxis);


        if (lastRot != null && stick.get() && canTilt) {

            Quaternion change = lastRot * Quaternion.Inverse(newRot);


            Quaternion changeApplied = Quaternion.Inverse(change) * target;

            target = Quaternion.Slerp(target, changeApplied, 1f);
        }

        if (target != null) {
            follower.rotation = Quaternion.Slerp(follower.rotation, target, 0.2f);
        }

        lastRot = newRot;
    }

    public void restart() {
        target = Quaternion.identity;
        canTilt = false;

        handle.enabled = false;
        followController = false;
        transform.up = Vector3.up;


    }

    public void startTimer() {
        StartCoroutine(timer());
    }

    IEnumerator timer() {

        timerText.enabled = true;
        timerText.text = "3";
        timerText.color = Color.red;
        yield return new WaitForSeconds(1);
        timerText.text = "2";
        timerText.color = new Color(1,.30f,0);
        yield return new WaitForSeconds(1);
        timerText.text = "1";
        timerText.color = Color.yellow;
        yield return new WaitForSeconds(1);
        timerText.text = "GO";
        timerText.color = Color.green;

        canTilt = true;

        yield return new WaitForSeconds(3);
        timerText.enabled = false;

        

    }

    //void Update()
    //{

    //    rot = InputMan.Grip(Side.Right);

    //    if (lastRot != null && rot){

    //        Quaternion change = lastRot *  Quaternion.Inverse(transform.rotation);


    //        Quaternion changeApplied = Quaternion.Inverse(change) *   target ;

    //        Debug.Log(changeApplied.x + "    " + Quaternion.Inverse(change).x + "     " + target.x);

    //        target = Quaternion.Slerp(target, changeApplied, 1f);
    //    }

    //    if (target != null) {
    //        follower.rotation = Quaternion.Slerp(follower.rotation, target, 0.25f);
    //    }

    //    lastRot = transform.rotation;
    //}
}
