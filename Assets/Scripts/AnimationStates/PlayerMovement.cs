using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour, IMovement
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float airSpeed = 1.7f;
    
    private Animator _animator;
    private Vector3 _jumpVerticalVelocity = Vector3.zero;
    private Vector3 _airVelocity = Vector3.zero; 
    private CharacterController _characterController;
    private bool _isGrounded = false;
    private bool _isInJump = false;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CheckIsGroundedChange();

        Vector3 velocity = Vector3.zero;

        if(!_isGrounded)
        {
            velocity += _airVelocity;
        }

        if(_isInJump)
        {
            velocity += _jumpVerticalVelocity;
        }
        else
        {
            velocity += Physics.gravity * Time.deltaTime;
        }
        
        _characterController.Move(velocity * Time.deltaTime);
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
        if (!_isGrounded || direction == Vector2.zero)
        {
            return;
        }

        _animator.SetTrigger("Roll");
    }

    public void Jump()
    {
        if (!_isGrounded)
        {
            return;
        }
            
        _jumpVerticalVelocity = _characterController.velocity;
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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Move(Vector2 direction, float speedRatio)
    {
        if(_isGrounded)
        {
            _animator.SetFloat("MoveSpeed", speedRatio);
            Rotate(InputToDirection(direction));
            _airVelocity = Vector3.zero;
        }
        else
        {
            _airVelocity = InputToDirection(direction) * airSpeed;
        }
    }

    private void CheckIsGroundedChange()
    {
        if(_isGrounded != _characterController.isGrounded)
        {
            _isGrounded = _characterController.isGrounded;
            _animator.SetBool("IsGrounded", _isGrounded);
        }
    }

    private void JumpStart()
    {
        _isInJump = true;
        _animator.SetBool("IsInJump", true);
    }
    
    private void JumpEnd()
    {
        _isInJump = false;
        _animator.SetBool("IsInJump", false);
    }
}