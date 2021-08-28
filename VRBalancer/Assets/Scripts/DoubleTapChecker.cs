
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTapChecker : MonoBehaviour
{
    public GameManager gm;

    Stick stick;

    float lastClickTime;

    public float min;
    public float max;

    // Start is called before the first frame update
    void Start()
    {
        stick = GetComponent<Stick>();
    }

    // Update is called once per frame
    void Update()
    {

        if (stick.getDown() && gm.rod.canTilt) {
            
            if (lastClickTime != 0) {

                if (lastClickTime  + max > Time.time && lastClickTime + min < Time.time) {
                    gm.restart();
                }

            }

            lastClickTime = Time.time;

        }
        
    }
}

