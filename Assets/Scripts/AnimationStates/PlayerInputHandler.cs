using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent (typeof(PlayerMovement))]
public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private float _deadZone = 0.05f;

    private Animator _animator;
    private PlayerInput _playerInput;
    private PlayerMovement _playerMovement;
    private bool _isIdle;
    private bool _isWalk;
    private bool _isRun;
    private Vector2 _directoinVector;


    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _animator = GetComponent<Animator>();
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
        if(_playerMovement.IsLockedMovement)
        {
            return;
        }

        _directoinVector = _playerInput.Player.Walk.ReadValue<Vector2>();

        if (_directoinVector.magnitude < _deadZone)
        {
            SetIdle();
            _playerMovement.Stay();
        }
        else
        {
            if (_playerInput.Player.Sprint.IsPressed())
            {
                SetRun();
            }
            else
            {
                SetWalk();
            }

            _playerMovement.Move(_directoinVector, _isWalk);
        }

        SetParams();
    }

    private void SetParams()
    {
        
        _animator.SetBool("IsIdle", _isIdle);
        _animator.SetBool("IsWalk", _isWalk);
        _animator.SetBool("IsRun", _isRun);

    }

    private void SetIdle()
    {
        _isIdle = true;
        _isWalk = false;
        _isRun = false;
    }

    private void SetWalk()
    {
        _isIdle = false;
        _isWalk = true;
        _isRun = false;
    }
    private void SetRun()
    {
        _isIdle = false;
        _isWalk = false;
        _isRun = true;
    }

    private void OnRoll()
    {
        if(_playerMovement.IsLockedMovement)
        {
            return;
        }

        _playerMovement.IsLockedMovement = true;
        _animator.SetTrigger("Roll");
        _playerMovement.Roll(_playerInput.Player.Walk.ReadValue<Vector2>());
    }

    private void OnJump()
    {
        if (_playerMovement.IsLockedMovement)
        {
            return;
        }

        _animator.SetTrigger("JumpStart");
        _playerMovement.Jump();
    }
}
