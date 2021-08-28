using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodCalibrator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            transform.localPosition -= new Vector3(0, 0.01f, 0);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            transform.localPosition += new Vector3(0, 0.01f, 0);
        }
    }
}
