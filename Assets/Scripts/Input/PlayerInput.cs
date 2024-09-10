using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    PlayerInputActions playerInputActions;

    #region PLayerControl
    private Vector2 axes => playerInputActions.Gameplay.Axes.ReadValue<Vector2>();
    public float AxesX => axes.x;
    public bool Move => AxesX != 0f;
    public bool Jump => playerInputActions.Gameplay.Jump.WasPressedThisFrame();
    public bool StopJump => playerInputActions.Gameplay.Jump.WasReleasedThisFrame();
    public bool KeepJump => playerInputActions.Gameplay.Jump.IsPressed();
    public bool Fire1 => playerInputActions.Gameplay.Fire1.WasPressedThisFrame();
    public bool KeepFire1 => playerInputActions.Gameplay.Fire1.IsPressed();
    public bool Dash => playerInputActions.Gameplay.Dash.WasPressedThisFrame();
    public bool Interact => playerInputActions.Gameplay.Interact.WasPressedThisFrame();
    public bool useAction => playerInputActions.Gameplay.UseAction.WasPressedThisFrame();
    public bool dialogueContinue => playerInputActions.Dialogue.Continue.WasPressedThisFrame();
    public bool EscMenu => playerInputActions.Gameplay.EscMenu.WasPressedThisFrame();
    #endregion

    #region UIControl
    public bool playerPanel => playerInputActions.Gameplay.PlayerPanel.WasPressedThisFrame();
    public bool closePanel => playerInputActions.PlayerPanel.ClosePanel.WasPressedThisFrame();
    public bool goBack => playerInputActions.PlayerPanel.GoBack.WasPressedThisFrame();
    #endregion

    private void OnEnable()
    {
        playerInputActions = new PlayerInputActions();
        //playerInputActions.Gameplay.SetCallbacks(this);
        //playerInputActions.PlayerPanel.SetCallbacks(this);
    }
    private void OnDisable()
    {
        if (playerInputActions != null)
        {
            //DisableAllInputs();
        }
    }

    private void SwitchActionMap(InputActionMap actionMap, bool isUIInput)
    {
        playerInputActions.Disable();
        actionMap.Enable();
        if (!isUIInput)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void EnableGamePlayInputs() => SwitchActionMap(playerInputActions.Gameplay, false);

    public void EnablePlayerPanelInputs() => SwitchActionMap(playerInputActions.PlayerPanel, true);

    public void EnableDialogueInputs() => SwitchActionMap(playerInputActions.Dialogue, true);

    public void DisableAllInputs() => playerInputActions.Disable();
}
