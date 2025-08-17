using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFadeOutDisable : MonoBehaviour
{
    #region �ִϸ��̼� �̺�Ʈ
    public void AnimationEnd()
    {
        gameObject.SetActive(false);
    }
    public void AnimationStart()
    {
        Fade(false);
    }
    public void FadeInEnd()
    {
        Fade(true);
    }

    protected virtual void Fade(bool onoff)
    {
        int count = transform.GetChild(1).childCount;
        for (int i = 0; i < count; i++)
            transform.GetChild(1).GetChild(i).gameObject.SetActive(onoff);
    }

    public void OnDescriptionEnabled()
    {

    }
    #endregion
}
