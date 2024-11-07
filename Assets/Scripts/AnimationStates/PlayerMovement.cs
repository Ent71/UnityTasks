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
    private bool _isGrounded => _characterController.isGrounded;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector3 velocity;

        if(_animator.GetBool("IsInJump"))
        {
            velocity = _jumpVerticalVelocity + _airVelocity;
        }
        else
        {
            velocity = Physics.gravity * Time.deltaTime;
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
        }
        else
        {
            _airVelocity = InputToDirection(direction) * airSpeed;
        }
    }
}