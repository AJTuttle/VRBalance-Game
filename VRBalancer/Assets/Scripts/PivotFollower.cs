using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotFollower : MonoBehaviour
{

    public GameObject player;
    public GameObject stage;

    
    void Start() {
        
    }
    
    void Update() {

        //return;

        //vars
        Vector3 posDifference = player.transform.position - transform.position;

        //move pivot
        transform.position += posDifference;
        
        //move stage
        stage.transform.position -= posDifference;


        //vertical
        //Vector3 height = new Vector3(0, transform.position.y, 0);

        //player.transform.position -= posDifference;
        //transform.position -= posDifference;
        //stage.transform.position -= posDifference;


    }
}
