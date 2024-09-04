using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EffectReturn : MonoBehaviour
{

    float time;
    [SerializeField] float usingTime = 0.4f;
    [SerializeField] float moving = 0;
    public Vector3 vec;
    RectTransform rect;
    Action[] action;
    public float speed;

    float tempUsingTimeSave = 0;


    public int type { private get; set; }

    private void OnEnable()
    {
        action = new Action[]

        {
            () =>
            {
                StartCoroutine(NonSliding());
            },
            () =>
            {
                Location();
                StartCoroutine(Sliding());
            },
            () =>
            {
                StartCoroutine(NonSliding());
            },
            () =>
            {
                StartCoroutine(GoForward());
            },
            () =>
            {
                Location2();
                StartCoroutine(Sliding());
            },
            () =>
            {
                
            }
        };
        time = 0;
        action[type]();
    }
    void Location()
    {
        rect = GetComponent<Text>().rectTransform;
        LocationEnd();
    }
    void Location2()
    {
        rect = GetComponent<TextMeshProUGUI>().rectTransform;
        LocationEnd();
    }
    void LocationEnd()
    {
        vec = Camera.main.ScreenToWorldPoint(rect.localPosition - Screen.width * 0.5f * Vector3.left - 0.5f * Screen.height * Vector3.down);
    }
    IEnumerator Sliding()
    {

        while (time < usingTime)
        {
            time += Time.unscaledDeltaTime;

            rect.localPosition = Unit.Data.Instance.CameratoCanvas(vec);

            vec +=  Time.unscaledDeltaTime * speed * Vector3.forward;
            yield return null;
        }
        TempTimeRollBack();
        InputEffect.e.Callback(gameObject, type);
        while (true)
        {
            rect.localPosition = Unit.Data.Instance.CameratoCanvas(vec);
            vec += Time.unscaledDeltaTime * speed / 2 * Vector3.forward;
            yield return null;
        }
    }
    IEnumerator NonSliding()
    {
        while (time < usingTime)
        {
            time += Time.deltaTime;
            transform.localPosition += moving * Time.deltaTime * Vector3.forward;
            yield return null;
        }
        TempTimeRollBack();
        InputEffect.e.Callback(gameObject, type);
    }
    IEnumerator GoForward()
    {
        Transform forward = transform.GetChild(0).transform;
        while (time < usingTime)
        {
            time += Time.deltaTime;
            forward.localPosition += moving * Time.deltaTime * Vector3.forward;
            yield return null;
        }
        TempTimeRollBack();
        InputEffect.e.Callback(gameObject, type);
    }
    public void AnimatorEnd()
    {
        InputEffect.e.Callback(gameObject, type);
    }


    public void TempTimeChange(float time)
    {
        tempUsingTimeSave = usingTime;
        usingTime = time;
    }
    void TempTimeRollBack()
    {
        if (tempUsingTimeSave != 0)
        {
            usingTime = tempUsingTimeSave;
            tempUsingTimeSave = 0;
        }
    }
    

}
