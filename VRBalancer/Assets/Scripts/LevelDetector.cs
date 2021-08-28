

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDetector : MonoBehaviour
{
    public GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {

        Debug.Log(other.tag);
        if (other.CompareTag("endPoint")) {
            gm.levelSelect(gm.currentLevel + 1);
        }
    }
}

