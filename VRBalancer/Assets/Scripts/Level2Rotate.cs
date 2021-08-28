using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Rotate : MonoBehaviour
{

    Quaternion startRot;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(rotate());
        startRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable() {
        StartCoroutine(rotate());
    }

    private void OnDisable() {
        StopCoroutine(rotate());
        transform.localRotation = startRot;
    }


    IEnumerator rotate() {
        //yield return new WaitForSeconds(5.00f);

        while (true) {

            float tracker = -90;

            while (tracker < 0) {
                transform.Rotate(new Vector3(0, 7, 0) * Time.deltaTime);
                tracker += 7 * Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(4f);

            while (tracker > -90) { 
                transform.Rotate(new Vector3(0, -7, 0) * Time.deltaTime);
                tracker += -7 * Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(4f);

            
        }
    }
}
