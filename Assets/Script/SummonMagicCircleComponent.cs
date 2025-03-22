using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonMagicCircleComponent : ClickCamTurningComponent
{
    SummonHeroWindow summonHeroWindow;
    protected override void VirtualAwake()
    {
        summonHeroWindow = camTurningWindow as SummonHeroWindow;
        isUsable = true;
    }
    protected override void SetMemory()
    {
    }

    protected override void SetWindowOpen()
    {
        summonHeroWindow.SetOpen(isWindowOpen);
    }


}
