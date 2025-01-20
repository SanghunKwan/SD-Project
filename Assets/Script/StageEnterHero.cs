using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
[RequireComponent(typeof(UnitSpawner))]
public class StageEnterHero : MonoBehaviour
{
    [SerializeField] SpawnManager spawnManager;
    [SerializeField] int waitSeconds;
    [SerializeField] int spawnCoolTime;
    UnitSpawner unitSpawner;
    [SerializeField] Vector3 gatePosition;


    private void Start()
    {
        if (!spawnManager.isEnter)
            return;

        unitSpawner = GetComponent<UnitSpawner>();
        WaitforSeconds(waitSeconds, SpawnHeroswithDelay);
    }
    async void WaitforSeconds(int miliSecond, Action action)
    {
        await Task.Delay(miliSecond);
        action();
    }
    async void SpawnHeroswithDelay()
    {
        InputEffect.e.PrintEffect(gatePosition, 8);

        await Task.Delay(500);
        Unit.Hero tempHero;
        int length = spawnManager.heroDatas.Length;
        int moveTime = 500;
        Vector3 destination = new Vector3(-(length - 1) * 0.5f, 0, -2);
        for (int i = 0; i < length; i++)
        {
            tempHero = unitSpawner.SpawnHeroData(spawnManager.heroDatas[i], i);
            tempHero.unitMove.Navi_Destination(Vector3.back);
            SetAllLayer(tempHero.transform.GetChild(0).gameObject, 18);
            await Task.Delay(moveTime);

            tempHero.unitMove.MoveOrAttack(MoveType.Move, destination);
            destination += Vector3.right;
            SetAllLayer(tempHero.transform.GetChild(0).gameObject, 7);
            await Task.Delay(spawnCoolTime - moveTime);
        }

    }
    void SetAllLayer(GameObject targetObject, int layerNum)
    {
        targetObject.layer = layerNum;
        int childcount = targetObject.transform.childCount;
        for (int i = 0; i < childcount; i++)
        {
            SetAllLayer(targetObject.transform.GetChild(i).gameObject, layerNum);
        }
    }

}
