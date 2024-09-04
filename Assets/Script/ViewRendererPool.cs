using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewRendererPool : MonoBehaviour
{
    public static ViewRendererPool pool;
    [SerializeField] GameObject filter;
    Transform folder;


    private void Awake()
    {
        pool = this;
        folder = new GameObject(filter.name).transform;
        folder.SetParent(transform);
    }
    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            Instantiate(filter, folder.transform);
        }

    }
    public GameObject Call()
    {
        if (folder.childCount >= 1)
        {
            return folder.GetChild(0).gameObject;
        }
        return Instantiate(filter, folder.transform);
    }
}
