
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelecter : MonoBehaviour
{
    GameManager gm;

    private void Start() {
        gm = FindObjectOfType<GameManager>();
    }

    public void levelSelect(int number) {
        gm.levelSelect(number);
    }
}

