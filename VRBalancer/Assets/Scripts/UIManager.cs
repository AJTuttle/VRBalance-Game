using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Collider[] btnCols;
    public Image[] btnImgs;

    // Start is called before the first frame update
    void Start()
    {

        updateLocks(PlayerPrefs.GetInt("level"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateLocks(int level) {

        Debug.Log(level + "   " + btnImgs[0]);
        

        for (int x = 0; x < level; x++) {
            btnImgs[x].color = Color.white;
            btnCols[x].enabled = true;
        }

    }
    
}
