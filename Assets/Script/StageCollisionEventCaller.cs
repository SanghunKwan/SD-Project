using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCollisionEventCaller : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 7)
            return;

        GameManager.manager.onPlayerEnterStage.eventAction?.Invoke(
            GameManager.manager.battleClearManager.nowFloorIndex - transform.parent.parent.GetSiblingIndex(), transform.position);
    }
}
