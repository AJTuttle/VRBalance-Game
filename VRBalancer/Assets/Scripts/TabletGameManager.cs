using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabletGameManager : MonoBehaviour
{

    public Camera camera;

    TabletEventMan eventMan;

    public Text rockText;

    int rocksLeft = 5;

    bool canLaunch = false;

    public Text timer;

    // Start is called before the first frame update
    void Start()
    {
        eventMan = GetComponent<TabletEventMan>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) { 
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {
                Transform objectHit = hit.transform;

                launchRock(hit);


            }
        }
    }

    public void startLevel() {
        rocksLeft = 5;
        rockText.text = "Rocks: " + rocksLeft;
        StartCoroutine(wait3());
    }

    void launchRock(RaycastHit hit) {

        if (rocksLeft < 1 || !canLaunch) return;

        //GameObject.Instantiate(droppablePrefab, hit.point + new Vector3(0, 10, 0), Quaternion.identity);

        eventMan.rockEvent(hit.point + new Vector3(0, 10, 0));

        rocksLeft--;


        rockText.text = "Rocks: " + rocksLeft;



    }

    IEnumerator wait3() {
        canLaunch = false;
        timer.gameObject.SetActive(true);
        timer.text = "3";
        yield return new WaitForSeconds(1);
        timer.text = "2";
        yield return new WaitForSeconds(1);
        timer.text = "1";
        yield return new WaitForSeconds(1);

        timer.gameObject.SetActive(false);
        canLaunch = true;
    }
}
