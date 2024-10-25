using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour, IMovement
{
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private Transform _transform;
    [SerializeField] private float _rollDuration = 1f;
    [SerializeField] private float _rollDelay = 0.2f;
    [SerializeField] private float _rollSpeed = 30f;
    [SerializeField] private float _jumpForce = 10f;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private Vector3 _rollDirection = Vector3.zero;
    private bool _isGrounded = false;
    private bool _isWalk = false;
    private bool _isRun = false;
    private bool _isIdle = true;

    private bool _isRoll = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
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

    public void Walk(Vector2 direction)
    {
        if (_isRoll)
        {
            return;
        }
        
        _isWalk = true;
        _isRun = false;
        _isIdle = false;
        SetParams();
        Move(direction, _walkSpeed);
    }

    public void Run(Vector2 direction)
    {
        if (_isRoll)
        {
            return;
        }

        _isWalk = false;
        _isRun = true;
        _isIdle = false;
        SetParams();

        Move(direction, _runSpeed);
    }

    public void Stay()
    {
        if (!_isGrounded || _isRoll)
        {
            return;
        }

        _isWalk = false;
        _isRun = false;
        _isIdle = true;
        SetParams();
    }

    public void Roll(Vector2 direction)
    {
        if (!_isGrounded || _isRoll || direction == Vector2.zero)
        {
            Debug.Log("Canceled roll");
            return;
        }

        _animator.SetTrigger("Roll");
        _rollDirection = new Vector3(direction.x, 0, direction.y);
        StartCoroutine(RollAfterDelay(_rollDelay));
    }

    public void Jump()
    {
        if (!_isGrounded || _isRoll)
        {
            return;
        }

        float _horizontalForce = 0;

        if (_isWalk)
        {
            _horizontalForce = _walkSpeed;
        }
        if (_isRun)
        {
            _horizontalForce = _runSpeed;
        }

        _animator.SetTrigger("JumpStart");
        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }
    private void Move(Vector2 direction, float speed)
    {
        Vector3 direction3 = new Vector3(direction.x, 0, direction.y);
        Quaternion targetRotation = Quaternion.LookRotation(direction3);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        _rigidbody.velocity = direction3 * speed + new Vector3(0, _rigidbody.velocity.y, 0);
    }

    private void MoveInRoll()
    {
        _rigidbody.velocity = _rollDirection * _rollSpeed;
    }

    private void FixedUpdate()
    {
        if(_isRoll)
        {
            MoveInRoll();
        }
    }

    private IEnumerator RollAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetInRoll(_rollDuration);
    }

    private void SetInRoll(float duration)
    {
        _isRoll = true;

        StartCoroutine(UnSetInRollAfterDelay(duration));
    }

    private IEnumerator UnSetInRollAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isRoll = false;
    }

    private void SetParams()
    {
        _animator.SetBool("IsIdle", _isIdle);
        _animator.SetBool("IsWalk", _isWalk);
        _animator.SetBool("IsRun", _isRun);
    }
}