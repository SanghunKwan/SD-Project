using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAssembleClick : MonoBehaviour
{
    CapsuleCollider capsuleCollider;
    TowerComponent towerComponent;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        towerComponent = transform.parent.parent.parent.GetComponent<TowerComponent>();
    }
    private void OnMouseUpAsButton()
    {
        towerComponent.AssembleClick(transform.GetSiblingIndex() + 1);
    }

    public void SetColliderActive(bool onoff)
    {
        capsuleCollider.enabled = onoff;
    }

}
