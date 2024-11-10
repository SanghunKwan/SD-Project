using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateInit : MonoBehaviour
{
    [SerializeField] CamTuringWindow[] camTuringWindow;
    [SerializeField] InitObject[] initObjects;
    void Start()
    {
        for (int i = 0; i < camTuringWindow.Length; i++)
        {
            camTuringWindow[i].Init();

        }
        for (int i = 0; i < initObjects.Length; i++)
        {
            initObjects[i].Init();
        }
    }

}
