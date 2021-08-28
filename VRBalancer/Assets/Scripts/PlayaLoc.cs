using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayaLoc : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayaLocFollower obj = GameObject.FindObjectOfType<PlayaLocFollower>();

        if (obj != null) {
            obj.target = gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
