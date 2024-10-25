using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(PlayerMovement))]
public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private float _deadZone = 0.05f;
    [SerializeField] private IMovement _playerMovement;

    private PlayerInput _playerInput;
    private bool _isIdle;
    private bool _isWalk;
    private bool _isRun;
    private Vector2 _directoinVector;


    private void Awake()
    {
        _playerMovement = GetComponent<IMovement>();
        _playerInput = new PlayerInput();
        _playerInput.Enable();
    }

    private void OnEnable()
    {
        _playerInput.Player.Jump.performed += ctx => OnJump();
        _playerInput.Player.Roll.performed += ctx => OnRoll();
    }

    private void OnDisable()
    {
        _playerInput.Player.Jump.performed -= ctx => OnJump();
        _playerInput.Player.Roll.performed -= ctx => OnRoll();
    }

    private void FixedUpdate()
    {
        InputHandler();
    }

    private void InputHandler()
    {
        _directoinVector = _playerInput.Player.Walk.ReadValue<Vector2>();

        if (_directoinVector == Vector2.zero || _directoinVector.magnitude < _deadZone)
        {
            _playerMovement.Stay();
        }
        else
        {
            if (_playerInput.Player.Sprint.IsPressed())
            {
                _playerMovement.Run(_directoinVector);
            }
            else
            {
                _playerMovement.Walk(_directoinVector);
            }

        }
    }

    private void OnRoll()
    {
        _playerMovement.Roll(_playerInput.Player.Walk.ReadValue<Vector2>());
    }

    private void OnJump()
    {
        _playerMovement.Jump();
    }
}
