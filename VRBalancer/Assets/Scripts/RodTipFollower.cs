using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodTipFollower : MonoBehaviour
{

    public Transform follow;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = follow.position;
        transform.rotation = follow.rotation;
        transform.rotation.eulerAngles.Set(transform.rotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);
    }
}
