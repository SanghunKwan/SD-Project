using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAssembleClick : MonoBehaviour
{
    CapsuleCollider capsuleCollider;
    TowerComponent towerComponent;
    public bool isLastClick { private get; set; }

    SkyScraperTransparent skyTransparent;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        skyTransparent = GetComponent<SkyScraperTransparent>();
        towerComponent = transform.parent.parent.parent.GetComponent<TowerComponent>();
    }
    private void OnMouseUpAsButton()
    {
        if (isLastClick)
            return;

        int siblingIndex = transform.GetSiblingIndex();
        towerComponent.AssembleClick(siblingIndex);
        isLastClick = true;
        GameManager.manager.onVilligeTowerFloorSelect.eventAction?.Invoke(siblingIndex + 1, Vector3.zero);
    }
    private void OnMouseEnter()
    {
        skyTransparent.ChangeTransparent(SkyScraperTransparent.EnumMaterial.highLightMaterials);
    }
    private void OnMouseExit()
    {
        skyTransparent.ChangeTransparent(SkyScraperTransparent.EnumMaterial.originalMaterials);

    }

    public void SetColliderActive(bool onoff)
    {
        capsuleCollider.enabled = onoff;

        if (!onoff)
            OnMouseExit();
    }

}
