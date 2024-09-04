using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingArrange : MonoBehaviour
{
    BoxCollider coll;
    int enterColNum;
    public bool isChanged { get; private set; }

    Dictionary<bool, Action> dicState = new Dictionary<bool, Action>();
    SpriteRenderer backGround;

    [SerializeField] Color[] colors = new Color[2];


    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
        backGround = transform.Find("BackGround").GetComponent<SpriteRenderer>();

        dicState.Add(false, ColNumZero);
        dicState.Add(true, ColNumOverZero);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
            enterColNum++;
        ChangeConstructState();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
            enterColNum--;

        ChangeConstructState();
    }

    void ChangeConstructState()
    {
        dicState[Convert.ToBoolean(enterColNum)]();
    }

    void ColNumZero()
    {
        isChanged = false;
        backGround.color = colors[0];
    }
    void ColNumOverZero()
    {
        if (isChanged)
            return;

        isChanged = true;
        backGround.color = colors[1];
    }

}