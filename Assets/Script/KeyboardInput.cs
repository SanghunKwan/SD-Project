using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class KeyboardInput : MonoBehaviour
{
    [SerializeField] menuWindow menuWindow;
    [SerializeField] WindowManager windowManager;


    public void OnSelect(InputAction.CallbackContext inputAction)
    {
        if (inputAction.phase == InputActionPhase.Started)
            GameManager.manager.ArmySelect(inputAction.control.displayName);
    }
    public void OnArmyAdd(InputAction.CallbackContext inputAction)
    {
        if (inputAction.phase == InputActionPhase.Started)
        {
            GameManager.manager.TeamAdd(inputAction.control.displayName);
        }

    }
    public void OnArmySet(InputAction.CallbackContext inputAction)
    {
        if (inputAction.phase == InputActionPhase.Started)
        {
            GameManager.manager.RegroupTeam(inputAction.control.displayName);
        }
    }

    public void OnFormation(InputAction.CallbackContext inputAction)
    {
        if (inputAction.phase == InputActionPhase.Started)
            GameManager.manager.Formation(inputAction.control.displayName);

        PlayerInputManager input = GetComponent<PlayerInputManager>();
    }
    public void OnTimeDelay(InputAction.CallbackContext inputAction)
    {
        if (inputAction.phase == InputActionPhase.Started)
            GameManager.manager.InputSpace();
        else if (inputAction.phase == InputActionPhase.Canceled)
            GameManager.manager.SpaceUp();
    }
    public void OnSimpleAct(InputAction.CallbackContext inputAction)
    {
        if (inputAction.phase == InputActionPhase.Started)
        {
            GameManager.manager.UnitStop(inputAction.control.displayName);
        }
    }
    public void OnActionAdd(InputAction.CallbackContext inputAction)
    {
        if (inputAction.phase == InputActionPhase.Started)
        {
            GameManager.manager.OrderAdd(inputAction.control.displayName);
        }
    }
    public void OnDoubleArmySelect(InputAction.CallbackContext inputAction)
    {
        if (inputAction.phase == InputActionPhase.Performed &&
            PlayerNavi.nav.PlayerCharacter[GameManager.manager.GetConvert(inputAction.control.displayName)].Count > 0)
        {

            GameManager.manager.ScreenToPoint(PlayerNavi.nav.getCenter);
        }
    }
    public void OnEscape(InputAction.CallbackContext inputAction)
    {
        if (inputAction.phase == InputActionPhase.Performed)
        {
            menuWindow.OnOffWindow();
        }
    }
    public void OnWindowClose(InputAction.CallbackContext inputAction)
    {
        if (inputAction.phase == InputActionPhase.Performed)
        {
            windowManager.CloseWindow();
        }
    }
}
