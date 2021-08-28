using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereFollower : MonoBehaviour
{

    public GameObject sphere;
    public Vector3 offset;
    void Start() {
        
    }
    

    void Update() {
        transform.position = sphere.transform.position - offset;
    }

}
