using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectUIPool : MonoBehaviour
{
    public static ObjectUIPool pool;
    [SerializeField] GameObject[] UIComponent;
    [SerializeField] int init = 10;

    Transform[] canvasArray;
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
        VilligeConstructingUI,

        AOESwing,
    }
    public enum UICanvasType
    {
        UpperCanvas,
        GroundCanvas,
        MinimapCanvas,
        Max
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

        canvasArray = new Transform[]
        {
            GameObject.FindGameObjectWithTag("CanvasWorld").transform,
            GameObject.FindGameObjectWithTag("Canvas").transform,
            Camera.main.transform.GetChild(0)
        };

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

    public GameObject Call(Folder uiType, UICanvasType canvasType)
    {
        return Call(uiType, canvasArray[(int)canvasType]);
    }
    public GameObject Call(Folder uiType, Transform parentTransform)
    {
        GameObject tempObject = CallPooling(uiType);
        tempObject.transform.SetParent(parentTransform, false);

        return tempObject;
    }
    GameObject CallPooling(Folder type)
    {
        if (folder[(int)type].transform.childCount >= 1)
        {
            return folder[(int)type].transform.GetChild(0).gameObject;
        }
        return Instantiate(UIComponent[(int)type]);
    }

    public void BackPooling(GameObject poolingObject, Folder type)
    {
        poolingObject.transform.SetParent(folder[(int)type], false);
    }

    public void ReadytoSceneLoad()
    {
        isReady = false;
    }
}

