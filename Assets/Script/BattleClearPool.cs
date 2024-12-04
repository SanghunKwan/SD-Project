using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattleClearManager))]
public class BattleClearPool : MonoBehaviour
{
    [SerializeField] GameObject[] objects;
    [SerializeField] int poolNum;

    void Start()
    {
        new GameObject("Objects").transform.SetParent(transform);
        new GameObject("Stages").transform.SetParent(transform);
        Transform objectsFolder = transform.GetChild(0);
        int length = objects.Length;

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < poolNum; j++)
            {
                Instantiate(objects[i], objectsFolder);
            }
        }

    }
}