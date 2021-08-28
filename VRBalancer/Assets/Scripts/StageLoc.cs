using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLoc : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StageLocFollower obj = GameObject.FindObjectOfType<StageLocFollower>();

        if (obj!= null) {
            obj.target = gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
