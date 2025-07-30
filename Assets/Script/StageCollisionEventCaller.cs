using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCollisionEventCaller : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 7)
            return;

        int stageIndex = transform.GetSiblingIndex();
        GameManager.manager.objectManager.GetNode(other.gameObject).Value.stageIndex = stageIndex;
        GameManager.manager.onPlayerEnterStage.eventAction?.Invoke(
            GameManager.manager.battleClearManager.nowFloorIndex - stageIndex, transform.position);
    }
}
