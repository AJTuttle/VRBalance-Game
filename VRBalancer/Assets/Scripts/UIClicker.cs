


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIClicker : MonoBehaviour
{

    public Stick stick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerStay(Collider other) {
        Button btn = other.GetComponent<Button>();
        if (btn != null && stick.getDown()) {

            btn.onClick.Invoke();

        }
    }
}

