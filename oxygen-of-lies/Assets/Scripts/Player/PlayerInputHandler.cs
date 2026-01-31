using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls _playerControls;
    
    private Vector2 _moveInput;
    public Vector2 MoveInput => _moveInput;
    
    public event Action OnJump;

    private void Awake()
    {
        _playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
        _playerControls.Player.Move.performed += OnMovePerformed;
        _playerControls.Player.Move.canceled += OnMoveCanceled;
        _playerControls.Player.Jump.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        _playerControls.Player.Move.performed -= OnMovePerformed;
        _playerControls.Player.Move.canceled -= OnMoveCanceled;
        _playerControls.Player.Jump.performed -= OnJumpPerformed;
        _playerControls.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }
}