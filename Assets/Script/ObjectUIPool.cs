using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectUIPool : MonoBehaviour
{
    public static ObjectUIPool pool;
    [SerializeField] GameObject[] UIComponent;
    [SerializeField] int init = 10;
    Transform[] folder;
    public static bool isReady { get; private set; } = false;

    public enum Folder
    {
        HPBar,
        UICircle,
        UILineRenderer,
        UILineRendererMiniMap,
        UIItemCircle,
        MentalBar,

        AOESwing,
    }

    private void Awake()
    {
        
        pool = this;

        folder = new Transform[UIComponent.Length];
        for (int i = 0; i < UIComponent.Length; i++)
        {
            folder[i] = new GameObject(UIComponent[i].name).transform;

            folder[i].transform.SetParent(transform);
        }
        isReady = true;
    }
    private void Start()
    {
        for (int i = 0; i < UIComponent.Length; i++)
        {

            for (int k = 0; k < init; k++)
            {
                Instantiate(UIComponent[i], folder[i].transform);
            }
        }
    }
    public GameObject Call(Folder type)
    {
        if (folder[(int)type].transform.childCount >= 1)
        {
            return folder[(int)type].transform.GetChild(0).gameObject;
        }
        return Instantiate(UIComponent[(int)type]);
    }
    public void ReadytoSceneLoad()
    {
        isReady = false;
    }
}

