using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class MouseOnImage : MonoBehaviour
{
    [SerializeField] InfoBox box;
    public Hero mhero { get; private set; }
    public SaveData.HeroData mheroData { get; private set; }

    public void SetHero(Hero hero)
    {
        mhero = hero;
        mheroData = null;
    }
    public void SetHero(SaveData.HeroData heroData)
    {
        mheroData = heroData;
        mhero = null;
    }

    void SetBox(in Vector3 vec, bool onOff)
    {
        box.transform.GetChild(0).gameObject.SetActive(onOff);

        box.transform.position = vec;
    }
    public void OnPointerEnter<T>(Vector3 vec, T image) where T : struct, Enum
    {
        SetBox(vec, true);

        if (mhero != null)
            box.SetTextMessage(mhero, image);
        else
            box.SetTextMessage(mheroData, image);
    }
    public void OnPointerEnter<T>(Vector3 vec, T image, int heroNumArrayValue) where T : struct, Enum
    {
        SetBox(vec, true);
        box.transform.GetChild(1).gameObject.SetActive(true);
        box.SetTextMessage(mhero, image, heroNumArrayValue);
    }
    public void OnPointerExit()
    {
        SetBox(Vector3.zero, false);
        box.CompareBoxTurnOff();
    }
}
