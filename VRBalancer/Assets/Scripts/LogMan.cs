using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMan : MonoBehaviour
{

    //transforms
    public Transform sphere;
    public Transform rod;
    public Transform rodTip;
    public Transform leftHand;
    public Transform rightHand;
    public Transform hmd;
    public Transform pivot;
    public Transform stage;


    // Update is called once per frame
    void Update()
    {

        List<string> data = new List<string>() {
            sphere.position.ToString(),
            sphere.rotation.ToString(),

            rod.position.ToString(),
            rod.rotation.ToString(),

            rodTip.position.ToString(),
            rodTip.rotation.ToString(),

            leftHand.position.ToString(),
            leftHand.rotation.ToString(),

            rightHand.position.ToString(),
            rightHand.rotation.ToString(),

            hmd.position.ToString(),
            hmd.rotation.ToString(),

            pivot.position.ToString(),
            pivot.rotation.ToString(),

            stage.position.ToString(),
            stage.rotation.ToString(),
        };

        unityutilities.Logger.LogRow("Transforms", data);
    }
}
