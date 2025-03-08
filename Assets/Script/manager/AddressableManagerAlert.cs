using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddressableManagerAlert : MonoBehaviour
{
    [SerializeField] NeedAddressable[] needAddressables;


    private void Start()
    {
        AddressableManager.manager.DelayUntilLoadingComplete(InitAllNeedAddressables);
    }

    void InitAllNeedAddressables()
    {
        foreach (var item in needAddressables)
        {
            item.Init();
        }
    }
}
