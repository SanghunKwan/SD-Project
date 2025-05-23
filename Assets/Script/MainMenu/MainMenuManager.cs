using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] AddressableManager addressableManager;

    [SerializeField] InitObject[] initObjects;

    [SerializeField] MainMenuIntro intro;
    [SerializeField] MenuClick clickToStart;
    [SerializeField] MenuAnimation meleeAnimation;
    [SerializeField] GameObject loadGames;
    [SerializeField] Animator camAnim;
    int camAnimHash;
    private void Awake()
    {
        foreach (var item in initObjects)
        {
            item.Init();
        }
        EventAllocate();
    }
    void EventAllocate()
    {
        intro.endEvent = WaitForButtonInput;
        clickToStart.buttonEvent = StartButtonClick;
    }
    void WaitForButtonInput()
    {
        clickToStart.ShowTitle();
    }
    void StartButtonClick()
    {

        camAnim.SetTrigger(camAnimHash);
        meleeAnimation.MoveStart();
        loadGames.SetActive(true);
    }



    private void Start()
    {
        camAnimHash = Animator.StringToHash("move");
        addressableManager.AddEvent(AddressableManager.LabelName.MainMenu, IntroPlay);
    }
    void IntroPlay()
    {
        addressableManager.GetData(AddressableManager.LabelName.MainMenu, AddressableManager.MainMenuImage.Logo, out Sprite sprite);
        intro.PlayLogo(sprite);
    }


}