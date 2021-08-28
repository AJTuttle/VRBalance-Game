using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{

    public Side side;
    LineRenderer lr;
    

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        updateLine();
        if (InputMan.TriggerDown(side)) sendLaser();
    }

    void updateLine() {

        lr.SetPosition(0, transform.position);

        if (InputMan.TriggerValue(side) > 0.1f) {
            lr.enabled = true;

            RaycastHit hit;

            Physics.Raycast(transform.position, transform.forward, out hit, 10);

            if (hit.collider!= null) {

                lr.SetPosition(1, hit.point);

            }

            else lr.SetPosition(1, transform.position + transform.forward * 10);

        }
        else lr.enabled = false;

    }

    void sendLaser() {

        RaycastHit hit;

        Physics.Raycast(transform.position, transform.forward, out hit, 10);

        if (hit.collider != null) {

            Button btn = hit.collider.GetComponent<Button>();
            if (btn != null) {

                btn.onClick.Invoke();

            }

        }

    }
}
