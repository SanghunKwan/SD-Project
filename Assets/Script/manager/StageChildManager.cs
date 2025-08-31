using System.Threading.Tasks;
using UnityEngine;

public class StageChildManager : StageManager
{
    [SerializeField] int editorIndex;
    [SerializeField] GameObject settlementScene;
    protected override void VirtualAwake()
    {
#if UNITY_EDITOR
        base.VirtualAwake();
        saveDataIndex = editorIndex;
#endif
    }

    public void CallSettlementScene()
    {
        sceneFadeAnim.gameObject.SetActive(true);
        SettleCall();
    }
    async void SettleCall()
    {
        Time.timeScale = 0;
        await Task.Delay(1000);
        settlementScene.SetActive(true);

    }
}
