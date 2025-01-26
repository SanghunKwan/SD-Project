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

        PrintText();
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
    public void ResetText()
    {


        PrintText();
    }
}
