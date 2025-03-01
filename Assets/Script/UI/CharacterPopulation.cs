using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class CharacterPopulation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    int heroNum;
    int heroMaxPopulation;
    StringBuilder stringBuilder;
    [SerializeField] int[] populationChange;


    private void Start()
    {
        stringBuilder = new StringBuilder();

        if (GameManager.manager.battleClearManager != null)
            LateStart();
        else
            GameManager.manager.onBattleClearManagerRegistered += LateStart;

    }
    void LateStart()
    {
        heroNum = GameManager.manager.battleClearManager.SaveDataInfo.hero.Length;
        heroMaxPopulation = 0;

        GameManager.manager.onVilligeBuildingHeroAllocation.eventAction += AddNewBuildingEvent;
        GameManager.manager.onVilligeBuildingHeroCancellation.eventAction += CancelHeroAllocation;

        ResetText();
    }
    void PrintText()
    {
        stringBuilder.Clear();

        if (heroNum > heroMaxPopulation)
            stringBuilder.Append("<color=#BA3434FF>");

        stringBuilder.Append(heroNum.ToString());
        stringBuilder.Append("</color>");
        stringBuilder.Append("/");
        stringBuilder.Append(heroMaxPopulation.ToString());

        text.text = stringBuilder.ToString();
    }
    void AddNewBuildingEvent(int buildingType, Vector3 vec)
    {
        AddMaxPopulation(populationChange[buildingType]);
    }
    void CancelHeroAllocation(int buildingType, Vector3 vec)
    {
        AddMaxPopulation(-populationChange[buildingType]);
    }
    public void ResetText()
    {
        PrintText();
    }
    public void AddMaxPopulation(int addNum)
    {
        heroMaxPopulation += addNum;
        ResetText();
    }
}
