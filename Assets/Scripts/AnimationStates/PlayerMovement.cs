using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour, IMovement
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _groundCheckDistance = 0.1f; // Distance to check for the ground
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField]private Animator _animator;
    
    private Vector3 _jumpVerticalVelocity = Vector3.zero; 
    private Vector3 _velicityInAir = Vector3.zero; 

    private Rigidbody _rigidbody;
    private bool _isGrounded = false;

    private bool _isRoll = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity += _jumpVerticalVelocity + _velicityInAir;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!_isGrounded && CheckIfGrounded())
        {
            SetGrounded();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(_isGrounded && !CheckIfGrounded())
        {
            SetUngrounded();
        }
    }

    public void Walk(Vector2 direction)
    {
        Move(direction, 0.5f);
    }

    public void Run(Vector2 direction)
    {
        Move(direction, 1f);
        
    }

    public void Stay()
    {
        _animator.SetFloat("MoveSpeed", 0f);
    }

    public void Roll(Vector2 direction)
    {
        if (!_isGrounded || _isRoll || direction == Vector2.zero)
        {
            Debug.Log("Canceled roll");
            return;
        }

        _animator.SetTrigger("Roll");
    }

    public void Jump()
    {
        if (!_isGrounded || _isRoll)
        {
            return;
        }
            
        _jumpVerticalVelocity = _rigidbody.velocity;
        _jumpVerticalVelocity.y = 0;
        
        _animator.SetTrigger("JumpStart");
    }

    private Vector3 InputToDirection(Vector2 input)
    {
        return new Vector3(input.x, 0, input.y);
    }

    private void Rotate(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    private void Move(Vector2 direction, float speedRatio)
    {
        // Debug.Log($"move: _isGrounded {_isGrounded} velocity: {_rigidbody.velocity} direction: {direction}");

        if(_isGrounded)
        {
            _animator.SetFloat("MoveSpeed", speedRatio);
            Rotate(InputToDirection(direction));
        }
        else
        {
            _velicityInAir = InputToDirection(direction);
        }
    }

    private void SetGrounded()
    {
        _jumpVerticalVelocity = Vector3.zero;
        _velicityInAir = Vector3.zero;
        _isGrounded = true;
    }

    private void SetUngrounded()
    {
        _isGrounded = false;
    }

    bool CheckIfGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance, _groundLayer);
    }
}