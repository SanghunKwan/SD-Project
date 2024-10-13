using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLightImage : InitInterface
{
    RectTransform recTransform;
    Animator anim;
    int hashNum;

    public void CallPosition(RectTransform callTransform)
    {
        Vector3 tempVector = callTransform.position;
        tempVector.y -= 10;

        transform.position = tempVector;
        anim.SetTrigger(hashNum);
    }
    public void CallFloor(int floor)
    {
        Vector3 tempVector = recTransform.anchoredPosition;
        tempVector.y = -80 + (floor - 100) * 120;

        recTransform.anchoredPosition = tempVector;
        anim.SetTrigger(hashNum);
    }

    public override void Init()
    {
        recTransform = GetComponent<RectTransform>();
        anim = GetComponent<Animator>();
        hashNum = Animator.StringToHash("twinkle");
    }
}
