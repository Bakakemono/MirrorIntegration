using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PushingBehavior : MonoBehaviour {
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

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Grab() {
        if(_isPushing) {
            _isPushing = false;
            _currentPushedObject.StopPushPull();
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
            }
        }
    }

    void WhilePushing(Vector2 movementInput) {
        Vector3 convertedMovenentInput = new Vector3(movementInput.x, 0f, movementInput.y).normalized;

        Vector3 projectedDirection = Vector3.Project(convertedMovenentInput, _pushingDirection).normalized;

        if(projectedDirection == _pushingDirection) {
            _rigidbody.velocity = projectedDirection * _pushingSpeed + Vector3.up * _rigidbody.velocity.y;
        }
        else {
            _rigidbody.velocity = projectedDirection * _pullingSpeed + Vector3.up * _rigidbody.velocity.y;
        }

    }
}
