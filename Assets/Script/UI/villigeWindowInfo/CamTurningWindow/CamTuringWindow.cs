using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class CamTuringWindow : InitInterface
{
    public ClickCamTurningComponent clickCamturningComponent { get; set; }
    public static ClickCamTurningComponent ienumOwner { get; set; }

    [SerializeField] GameObject[] UIObjectToggle;
    [SerializeField] GameObject[] UIObjectClose;

    public static GameObject transformObject { get; set; }


    [SerializeField] ClickDrag UIClickDragToggle;
    protected class BuildSetCharacter
    {
        public TextMeshProUGUI team;
        public TextMeshProUGUI heroName;
        string defaultName;
        public GameObject gameObject { get; private set; }


        public BuildSetCharacter(Transform tr)
        {
            team = tr.GetChild(0).GetComponent<TextMeshProUGUI>();
            heroName = tr.GetChild(1).GetComponent<TextMeshProUGUI>();
            gameObject = tr.gameObject;
            defaultName = heroName.text;
        }
        public void ResetTeam()
        {
            team.text = "";
            heroName.text = defaultName;
        }
        public void ChangeTeam(in string nameText, in string teamText)
        {

            team.text = teamText;
            heroName.text = nameText;
        }
    }
    public void ToggleWindow()
    {
        clickCamturningComponent.ToggleWindow();
    }
    public void Collider_UIActive(bool onoff)
    {
        foreach (var item in UIObjectToggle)
        {
            item.SetActive(onoff);
        }
        foreach (var item in UIObjectClose)
        {
            item.SetActive(false);
        }
        UIClickDragToggle.enabled = onoff;
    }
    public void GetTurningComponent(ClickCamTurningComponent getClickComponent)
    {
        clickCamturningComponent = getClickComponent;
    }
}
