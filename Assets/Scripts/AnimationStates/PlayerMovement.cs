using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private Transform _transform;
    [SerializeField] private float _rollDuration = 1f;
    [SerializeField] private float _rollSpeed = 12f;
    
    private float _elapsedTime = 0;

    private Rigidbody _rigidbody;
    [SerializeField] private float _jumpForce = 10f;

    bool _isGrounded = false;
    bool _isInWalk = false;
    bool _isInRun = false;

    Tweener _rotateTweener;
    bool _isInRotate = false;

    public bool IsLockedMovement { get; set; }

    private void Update()
    {
        if(IsLockedMovement)
        {
            _elapsedTime += Time.deltaTime;
            if(_elapsedTime >= _rollDuration)
            {
                _elapsedTime = 0;
                IsLockedMovement = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Ground>(out _))
        {
            _isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Ground>(out _))
        {
            _isGrounded = false;
        }
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector2 direction, bool isWalk)
    {
        if(IsLockedMovement)
        {
            return;
        }

        Vector3 direction3 = new Vector3(direction.x, 0, direction.y);
        float speed;

        if(isWalk)
        {
            speed = _walkSpeed;
            _isInWalk = true;
            _isInRun = false;
        }
        else
        {
            speed = _runSpeed;
            _isInWalk = false;
            _isInRun = true;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction3);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        _rigidbody.velocity = direction3 * speed + new Vector3(0, _rigidbody.velocity.y, 0);
    }
    public void Stay()
    {
        if (!_isGrounded || IsLockedMovement)
        {
            return;
        }

        _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
        _isInWalk = false;
        _isInRun = false;
    }

    private void OnRotationComplete()
    {
        _isInRotate = false;
    }

    public void Roll(Vector2 direction)
    {
        if (!_isGrounded)
        {
            return;
        }

        IsLockedMovement = true;
        _elapsedTime = 0;
        Vector3 direction3 = new Vector3(direction.x, 0, direction.y);
        _rigidbody.velocity = direction3 * _rollSpeed + new Vector3(0, _rigidbody.velocity.y, 0);
    }

    public void Jump()
    {
        if (!_isGrounded)
        {
            return;
        }

        float _horizontalForce = 0;

        if (_isInWalk)
        {
            _horizontalForce = _walkSpeed;
        }
        if (_isInRun)
        {
            _horizontalForce = _runSpeed;
        }

        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }
}