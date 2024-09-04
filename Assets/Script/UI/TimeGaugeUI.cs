using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeGaugeUI : MonoBehaviour
{
    Scrollbar scrollbar;
    float delayTime;

    IEnumerator timeCor;

    private void Start()
    {
        scrollbar = GetComponent<Scrollbar>();
        delayTime = 1;
        scrollbar.size = delayTime;

        GameManager.manager.timeUIEvent[0] += DelayStart;
        GameManager.manager.timeUIEvent[1] += DelayEnd;
    }
    IEnumerator TimeCheck()
    {
        while (delayTime > 0)
        {
            delayTime -= 0.001f;
            scrollbar.size = delayTime;
            yield return null;
        }
        GameManager.manager.TimeDelayEnd();
        DelayEnd();
    }

    IEnumerator TimeRecharge()
    {
        while (delayTime < 1)
        {
            delayTime += 0.0005f;
            scrollbar.size = delayTime;
            yield return null;
        }
    }
    void DelayStart()
    {
        if (timeCor != null)
            StopCoroutine(timeCor);
        timeCor = TimeCheck();
        StartCoroutine(timeCor);
    }
    void DelayEnd()
    {
        if (timeCor != null)
            StopCoroutine(timeCor);
        timeCor = TimeRecharge();
        StartCoroutine(timeCor);
    }
}
