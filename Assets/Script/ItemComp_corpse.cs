using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;
using System;

public class ItemComp_corpse : ItemComponent
{
    public UnitState unitState;
    Hero hero;
    bool isInitComplete;
    Action delayEnable;

    public void Init(int indexNum, int _circlePad, float Speed, Hero heroComponent)
    {
        hero = heroComponent;
        unitState = hero.unitMove.unit_State;

        SetIndex(indexNum);
        CirclePad = _circlePad;
        getSpeed = Speed;

        isInitComplete = true;
        delayEnable?.Invoke();
    }
    protected override void VirtualEnable()
    {
        Action action = () =>
        {
            unitState.Death(false, true);
            StartCoroutine(DelayAniEnd());
        };

        if (isInitComplete)
            action();
        else
            delayEnable = action;
    }

    IEnumerator DelayAniEnd()
    {
        yield return new WaitForSeconds(1);
        AnimationEnd();
    }

    protected override void InventoryAddItem(in GameObject itemFinder)
    {
        GameManager.manager.storageManager.AddCorpse(itemFinder, hero);
    }
    protected override void StageOutItem()
    {
    }
}
