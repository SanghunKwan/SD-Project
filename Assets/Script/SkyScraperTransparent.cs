using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyScraperTransparent : MonoBehaviour
{
    MeshRenderer mRenderer;
    List<Material> originalMaterials = new List<Material>();
    [SerializeField] List<Material> transparentMaterials;
    List<Material> nullMaterials = new List<Material>();

    List<List<Material>> listMaterial = new List<List<Material>>();
    public enum EnumMaterial
    {
        originalMaterials,
        transparentMaterials,
        nullMaterials
    }

    private void Awake()
    {
        mRenderer = GetComponent<MeshRenderer>();
        mRenderer.GetMaterials(originalMaterials);
        for(int i = 0; i < transparentMaterials.Count; i++)
        {
            nullMaterials.Add(null);
        }

        listMaterial.Add(originalMaterials);
        listMaterial.Add(transparentMaterials);
        listMaterial.Add(nullMaterials);

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
