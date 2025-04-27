using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class QuirkButtonController : MonoBehaviour
{
    [System.Serializable]
    public class QuirkButtons
    {
        public Transform buttonParent;
        public QuirkChangeButtonCaller[] QuirkChangeButtonCallers { get; set; }

        public void Init(QuirkButtonType index, QuirkButtonController controller, SetBuildingMat setMat)
        {
            QuirkChangeButtonCallers = buttonParent.GetComponentsInChildren<QuirkChangeButtonCaller>();

            int length = QuirkChangeButtonCallers.Length;
            for (int i = 0; i < length; i++)
            {
                QuirkChangeButtonCallers[i].Init(index, controller, setMat);
            }
        }
    }


    [SerializeField] QuirkButtons[] buttons;
    [SerializeField] QuirkOrderButton quirkOrderButton;
    HeroUpgradeWindow heroUpgradeWindow;
    Hero m_hero;
    [SerializeField] UnityEngine.UI.Button healingButton;

    public System.Action ResetAction { get; set; }
    private void Awake()
    {
        heroUpgradeWindow = transform.parent.GetComponent<HeroUpgradeWindow>();

        int length = buttons.Length;
        for (int i = 0; i < length; i++)
        {
            buttons[i].Init((QuirkButtonType)i, this, heroUpgradeWindow.SetBuildingMat);
        }
        quirkOrderButton.Init();
    }

    public void ButtonCall(QuirkButtonType type, int siblingIndex)
    {
        RectTransform rectTransform = buttons[(int)type].QuirkChangeButtonCallers[siblingIndex].rectTransform;

        Vector3 vec = rectTransform.position + Vector3.left * ((int)type * 2 - 1) * rectTransform.sizeDelta.x * 0.5f;

        quirkOrderButton.SetButtonPosition(vec);
        quirkOrderButton.SetButtonAction(() => OnButtonClick(type, siblingIndex));
        GameManager.manager.onGetMaterials.eventAction?.Invoke((int)System.Enum.Parse<GameManager.GetMaterialsNum>(type.ToString()), Vector3.zero);
    }
    public void SetHero(Hero hero)
    {
        m_hero = hero;
        healingButton.interactable = m_hero.curstat.curHP != m_hero.curstat.HP;
    }
    public void OnButtonClick(QuirkButtonType type, int siblingIndex)
    {
        QuirkData.Quirk[] quirks;
        if (type == QuirkButtonType.Quirk)
            quirks = m_hero.quirks;
        else
            quirks = m_hero.disease;

        TryRemoveAtQuirk(siblingIndex, type, quirks);

    }
    void TryRemoveAtQuirk(int index, QuirkButtonType type, QuirkData.Quirk[] quirks)
    {
        TryCalculate(type, () =>
        {
            RemoveAtQuirk(index, quirks);
            quirkOrderButton.ButtonInActive();
            ResetAction?.Invoke();
        });
    }
    void RemoveAtQuirk(int index, QuirkData.Quirk[] quirks)
    {
        int length = quirks.Length - 1;
        for (int i = index; i < length; i++)
        {
            quirks[i] = quirks[i + 1];
        }
        quirks[^1] = QuirkData.manager.quirkInfo.quirks[0];
    }

    public void OnHealingButtonClick()
    {
        TryCalculate(QuirkButtonType.Healing, () =>
        {
            m_hero.Recovery(1000);
            healingButton.interactable = false;
        });
    }
    void TryCalculate(QuirkButtonType type, System.Action action)
    {
        MaterialsData.NeedMaterials materialData = heroUpgradeWindow.SetBuildingMat.GetData((int)type, SetBuildingMat.MaterialsType.MedecineNeed);

        if (heroUpgradeWindow.SetBuildingMat.isBuildable)
        {
            heroUpgradeWindow.StorageComponent.CalculateMaterials(materialData);
            action();
        }
        else
        {
            heroUpgradeWindow.SetBuildingMat.HighLightNotEnoughMaterials((int)type, SetBuildingMat.MaterialsType.MedecineNeed);
        }
    }
    public enum QuirkButtonType
    {
        Quirk,
        Disease,
        Healing
    }
}
