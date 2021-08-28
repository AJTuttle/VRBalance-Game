using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandleGrabber : MonoBehaviour
{
    public Side side;
    public Rod rod;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other) {
        if (InputMan.Grip(side)) {
            Debug.Log("grip");
            rod.followController = true;
            rod.controllerToFollow = transform;
        }
    }

    private void OnTriggerExit(Collider other) {
        //if (InputMan.Grip(side)) {
        //    if (side == Side.Right) {
        //        OVRHaptics.RightChannel.Preempt(new OVRHapticsClip(new byte[] { 255, 255, 255, 255, 255 }, 5));
        //    }
        //    else if (side == Side.Left) {
        //        OVRHaptics.LeftChannel.Preempt(new OVRHapticsClip(new byte[] { 255, 255, 255, 255, 255 }, 5));
        //    }
        //}


        if (InputMan.Grip(side)) {
            if (side == Side.Right) {

                InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(0, 1, .1f);
            }
            else if (side == Side.Left) {

                InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).SendHapticImpulse(0, 1, .1f);
            }
        }

    }
}
