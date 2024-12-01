using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PushingBehavior : MonoBehaviour {

    // Register any methods that need to be called if the player or the box start to fall.
    public Action _onReleasing;

    [Header("Collision detection Box Params")]
    [SerializeField] private Transform _grabBoxDetectionPostion;
    [SerializeField] private Vector3 _grabBoxDetectionDimension;
    
    private Rigidbody _rigidbody;
    Vector3 _pushingDirection = Vector3.zero;
    PushableObject _currentPushedObject;
    bool _isPushing = false;
    float _pushingSpeed = 0f;
    float _pullingSpeed = 0f;

    [SerializeField] LayerMask _pushableObjectLayer;

    float _heightWhenPushing = 0f;
    float _pushedObjectHeight = 0f;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if(_isPushing && (
            (transform.position.y < _heightWhenPushing - 0.1f || transform.position.y > _heightWhenPushing + 0.1f) ||
            (_currentPushedObject.transform.position.y < _pushedObjectHeight - 0.1f || _currentPushedObject.transform.position.y > _pushedObjectHeight + 0.1f)
            )) {
            _onReleasing.Invoke();
            _isPushing = false;
            _currentPushedObject.StopPushPull();
        }
    }

    // Grab the object in the detection area.
    // Return true if a grabable object has been found the lock classic control from player then call WhilePushing(Vector2 movementInput).
    // Return false if there is no object to grab or if the player in release on in hand.
    public bool Grab() {
        if(_isPushing) {
            _isPushing = false;
            _currentPushedObject.StopPushPull();
            return false;
        }
        else {
            Collider[] _detectedObjects =
                Physics.OverlapBox(
                    _grabBoxDetectionPostion.position,
                    _grabBoxDetectionDimension / 2f,
                    transform.rotation,
                    _pushableObjectLayer
                    );

            if(_detectedObjects.Length > 0) {
                _currentPushedObject = _detectedObjects[0].GetComponent<PushableObject>();
                _currentPushedObject.StartPushPull(gameObject);
                _pushingSpeed = _currentPushedObject.GetPushingSpeed();
                _pullingSpeed = _currentPushedObject.GetPullingSpeed();

                // Register current elevation for fall detection.
                _pushedObjectHeight = _currentPushedObject.transform.position.y;
                _heightWhenPushing = transform.position.y;

                // Determine the direction in which the player is going to push.
                Vector3 localPos = _currentPushedObject.transform.InverseTransformPoint(transform.position);

                if(MathF.Abs(localPos.x) > Mathf.Abs(localPos.z)) {

                    Vector3 pushingDirection = _currentPushedObject.GetPushDirection();

                    if(localPos.x < 0) {
                        _pushingDirection = new Vector3(pushingDirection.z, 0f, -pushingDirection.x);
                    }
                    else {
                        _pushingDirection = new Vector3(-pushingDirection.z, 0f, pushingDirection.x);
                    }
                }
                else {
                    if(localPos.z < 0) {
                        _pushingDirection = _currentPushedObject.GetPushDirection();
                    }
                    else {
                        _pushingDirection = _currentPushedObject.GetPushDirection() * -1f;

                    }
                }

                transform.rotation = Quaternion.LookRotation(_pushingDirection);
                _isPushing = true;
                return true;
            }
            return false;
        }
    }

    public void WhilePushing(Vector2 movementInput) {
        if(!_isPushing)
            return;

        Vector3 convertedMovenentInput = new Vector3(movementInput.x, 0f, movementInput.y).normalized;

        Vector3 projectedDirection = Vector3.Project(convertedMovenentInput, _pushingDirection).normalized;

        if(projectedDirection == _pushingDirection) {
            _rigidbody.velocity = projectedDirection * _pushingSpeed + Vector3.up * _rigidbody.velocity.y;
        }
        else {
            _rigidbody.velocity = projectedDirection * _pullingSpeed + Vector3.up * _rigidbody.velocity.y;
        }

        transform.rotation = Quaternion.LookRotation(_pushingDirection);
    }

    public PushableObject GetPushedObject() {
        return _currentPushedObject;
    }
}
