using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyScraperComponent : MonoBehaviour
{
    Camera camMain;

    [SerializeField] float offset;
    [SerializeField] float disEnabledOffset;
    [SerializeField] Transform trMesh;
    [SerializeField] SkyScraperTransparent[] scraperScripts;
    ObjectAssembly objectAssemble;

    float tanAngle = -0.839099631177f;

    private void Start()
    {
        objectAssemble = GetComponent<ObjectAssembly>();
        camMain = Camera.main;

        Init();

        GameManager.manager.screenMove += (vec) => PositionCheck();
        if (objectAssemble != null)
            objectAssemble.init += Init;
    }
    public void Init()
    {
        scraperScripts = trMesh.GetComponentsInChildren<SkyScraperTransparent>();
    }
    public void PositionCheck()
    {
        float fLength = camMain.transform.position.z - transform.position.z - offset;

        float fTransparent = tanAngle * camMain.transform.position.y - fLength;

        SetTransparent(fTransparent);
        SetEnable(fTransparent + disEnabledOffset > 0);
    }
    public void SetTransparent(float distance)
    {
        int convertNum = System.Convert.ToInt32(distance < 0);
        SkyScraperTransparent.EnumMaterial enumNum = (SkyScraperTransparent.EnumMaterial)convertNum;
        foreach (var script in scraperScripts)
        {
            script.ChangeTransparent(enumNum);
            if (enumNum == SkyScraperTransparent.EnumMaterial.transparentMaterials)
                script.DistanceToTransparent((distance + disEnabledOffset) / disEnabledOffset);
        }
    }
    void SetEnable(bool onoff)
    {
        foreach (var script in scraperScripts)
        {
            script.SetRendererEnable(onoff);
        }
    }
}
