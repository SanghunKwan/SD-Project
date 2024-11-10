using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] AddressableManager addressableManager;
    [SerializeField] AssetLabelReference[] labels;

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
        LoadData();
        camAnimHash = Animator.StringToHash("move");
    }
    void LoadData()
    {
        for (int i = 0; i < labels.Length; i++)
        {
            addressableManager.LoadData(labels[i].labelString, IntroPlay);
        }
    }
    void IntroPlay()
    {
        addressableManager.GetData(labels[0].labelString, AddressableManager.MainMenuImage.Logo, out Sprite sprite);
        intro.PlayLogo(sprite);
    }
    

}