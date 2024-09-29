using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyScraperTransparent : MonoBehaviour
{
    MeshRenderer mRenderer;
    List<Material> originalMaterials = new List<Material>();
    [SerializeField] List<Material> transparentMaterials;
    [SerializeField] List<Material> highLightMaterials;

    List<List<Material>> listMaterial = new List<List<Material>>();
    public enum EnumMaterial
    {
        originalMaterials,
        transparentMaterials,
        highLightMaterials
    }

    private void Awake()
    {
        mRenderer = GetComponent<MeshRenderer>();
        mRenderer.GetMaterials(originalMaterials);

        listMaterial.Add(originalMaterials);
        listMaterial.Add(transparentMaterials);
        listMaterial.Add(highLightMaterials);

    }

    public void ChangeTransparent(EnumMaterial materialType)
    {
        mRenderer.SetMaterials(listMaterial[(int)materialType]);
    }
    public void SetRendererEnable(bool onoff)
    {
        mRenderer.enabled = onoff;
    }
}
