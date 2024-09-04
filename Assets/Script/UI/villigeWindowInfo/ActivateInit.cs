using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateInit : MonoBehaviour
{
    [SerializeField] InitInterface[] initInterfaces;
    void Start()
    {
        for (int i = 0; i < initInterfaces.Length; i++)
        {
            initInterfaces[i].Init();
        }
    }


}
