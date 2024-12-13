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


    private void Start()
    {
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
        foreach (var hero in spawnManager.heroDatas)
        {
            unitSpawner.SpawnHeroData(hero);
            await Task.Delay(spawnCoolTime);
        }

    }

}
