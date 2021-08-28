using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureTileInstance : MonoBehaviour
{

    public float width = 1;
    public float height = 1;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.mainTextureScale = new Vector2(width, height);
    }

   
}
