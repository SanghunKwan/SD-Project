using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class MouseOnImage : MonoBehaviour
{
    [SerializeField] InfoBox box;
    Hero mhero;

    public void SetHero(Hero hero)
    {
        mhero = hero;
    }

    void SetBox(in Vector3 vec, bool onOff)
    {
        box.transform.GetChild(0).gameObject.SetActive(onOff);
        box.transform.position = vec;
    }
    public void OnPointerEnter<T>(Vector3 vec, T image) where T : struct, Enum
    {
        box.transform.GetChild(0).gameObject.SetActive(true);
        box.transform.position = vec;
        box.SetTextMessage(mhero, image);
    }
    public void OnPointerExit()
    {
        SetBox(Vector3.zero, false);
    }
}
