using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private InputActionsMap inputActionsMap;
    public event EventHandler OnPlayerJump;
    public event EventHandler OnPlayerInteract;
    public event EventHandler OnPlayerDrop;
    public event EventHandler OnPlayerPrimary;
    public event EventHandler OnPlayerInteractAlternative;
    public event EventHandler OnPlayerThrow;

    //Singleton
    public static GameInput Instance;
    private void Start()
    {
        Instance = this;
    }

    private void Awake()
    {
        inputActionsMap = new InputActionsMap();
        inputActionsMap.Player.Enable();
        inputActionsMap.Player.Jump.performed += Jump_performed;
        inputActionsMap.Player.InteractMain.performed += InteractMain_performed;
        inputActionsMap.Player.Drop.performed += Drop_performed;
        inputActionsMap.Player.Primary.performed += Primary_performed;
        inputActionsMap.Player.InteractAlternative.performed += InteractAlternative_performed;
        inputActionsMap.Player.Throw.performed += Throw_performed;

        LockCursor();
    }

    private void Throw_performed(InputAction.CallbackContext obj)
    {
        OnPlayerThrow?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternative_performed(InputAction.CallbackContext obj)
    {
        OnPlayerInteractAlternative?.Invoke(this, EventArgs.Empty);
    }

    private void Primary_performed(InputAction.CallbackContext obj)
    {
        OnPlayerPrimary?.Invoke(this, EventArgs.Empty);
    }

    private void Drop_performed(InputAction.CallbackContext obj)
    {
        OnPlayerDrop?.Invoke(this, EventArgs.Empty);
    }

    private void InteractMain_performed(InputAction.CallbackContext obj)
    {
        OnPlayerInteract?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPlayerJump?.Invoke(this, EventArgs.Empty);
    }

    // Get movement input as a normalized Vector2
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = inputActionsMap.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }

    // Get mouse delta for look input
    public Vector2 GetLookVector()
    {
        return inputActionsMap.Player.Look.ReadValue<Vector2>();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
