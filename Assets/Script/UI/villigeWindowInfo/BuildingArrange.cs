using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingArrange : MonoBehaviour
{

    public bool isChanged { get; private set; }

    Dictionary<bool, Action> dicState = new Dictionary<bool, Action>();
    HashSet<Collider> colliders = new HashSet<Collider>();
    SpriteRenderer backGround;

    [SerializeField] Color[] colors = new Color[2];


    private void Awake()
    {
        backGround = transform.Find("BackGround").GetComponent<SpriteRenderer>();

        dicState.Add(false, ColNumZero);
        dicState.Add(true, ColNumOverZero);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
            colliders.Add(other);
        ChangeConstructState();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
            colliders.Remove(other);

        ChangeConstructState();
    }

    void ChangeConstructState()
    {
        dicState[Convert.ToBoolean(colliders.Count)]();
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