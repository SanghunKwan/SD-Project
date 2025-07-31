using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class StageCollisionEventCaller : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 7)
            return;

        int stageIndex = transform.parent.parent.GetSiblingIndex();

        GameManager gameManager = GameManager.manager;
        BattleClearManager battleClearManager = gameManager.battleClearManager;
        CObject heroObject = gameManager.objectManager.GetNode(other.gameObject).Value;

        //stageIndex를 참조하면 이전에 있는 스테이지가 어딘지 알 수 있음.
        int beforeIndex = heroObject.stageIndex;
        heroObject.stageIndex = stageIndex;

        gameManager.onPlayerEnterStage.eventAction?.Invoke(
            battleClearManager.nowFloorIndex - stageIndex, transform.position);

        //battleClearManager.herosInStages[beforeIndex].Add(heroObject);
    }
}
