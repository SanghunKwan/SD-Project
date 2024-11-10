using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuIntro : PlayBase
{
    Image backGround;
    Image logo;
    Animator animator;
    [SerializeField] float logoShowTime;
    [SerializeField] float backGroundFadeTime;
    public Action endEvent { get; set; }

    public override void Init()
    {
        gameObject.SetActive(true);
        backGround = GetComponent<Image>();
        logo = transform.GetChild(0).GetComponent<Image>();
        animator = logo.GetComponent<Animator>();
    }
    public void PlayLogo(Sprite sprite)
    {
        logo.sprite = sprite;
        logo.gameObject.SetActive(true);

        StartCoroutine(WaitingforSeconds(logoShowTime, FadeOutLogo));
    }
    void FadeOutLogo()
    {
        animator.SetTrigger("fadeOut");
        StartCoroutine(WaitingforSeconds(logoShowTime, () => StartCoroutine(BackGroundColorClear())));
    }
    IEnumerator BackGroundColorClear()
    {

        Color dempColor = Color.black / backGroundFadeTime;
        while (backGround.color.a > 0)
        {
            backGround.color -= dempColor * Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
        endEvent();
    }

}
