using Steamworks;
using System;
using UnityEngine;

public class PlayerPush {
    private Transform _playerTransform;
    private Rigidbody _rigidbody;

    RuntimePushConfig _config;

    Vector3 _pushingDirection = Vector3.zero;
    PushableObject _currentPushedObject;
    Vector3 _objRelativePos = Vector3.zero;

    bool isPushing = false;
    public bool _isPushing {
        get {
            return isPushing;
        }
    }
    float _pushingSpeed = 0f;
    float _pullingSpeed = 0f;

    float _heightWhenPushing = 0f;
    float _pushedObjectHeight = 0f;

    public PlayerPush(RuntimePushConfig config, Transform transform, Rigidbody rigidBody) {
        _config = config;
        _playerTransform = transform;
        _rigidbody = rigidBody;
    }

    // Grab the object in the detection area.
    // Return true if a grabable object has been found the lock classic control from player then call WhilePushing(Vector2 movementInput).
    // Return false if there is no object to grab or if the player in release on in hand.
    public void Grab() {
        if(isPushing) {
            isPushing = false;
            _currentPushedObject.StopPushPull(_playerTransform);
            return;
        }
        else {
            Collider[] _detectedObjects =
                Physics.OverlapBox(
                    _playerTransform.TransformPoint(_config._grabBoxDetectionPostion),
                    _config._grabBoxDetectionDimension / 2f,
                    _playerTransform.rotation,
                    _config._pushableObjectLayerMask
                    );

            if(_detectedObjects.Length > 0) {
                _currentPushedObject = _detectedObjects[0].GetComponent<PushableObject>();
                _currentPushedObject.StartPushPull(_playerTransform);
                _pushingSpeed = _currentPushedObject.GetPushingSpeed();
                _pullingSpeed = _currentPushedObject.GetPullingSpeed();

                // Register current elevation for fall detection.
                _pushedObjectHeight = _currentPushedObject.transform.position.y;
                _heightWhenPushing = _playerTransform.position.y;
                _objRelativePos = _currentPushedObject.transform.position - _playerTransform.position;

                // Determine the direction in which the player is going to push.
                Vector3 localPos = _currentPushedObject.transform.InverseTransformPoint(_playerTransform.position);

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

                _playerTransform.rotation = Quaternion.LookRotation(_pushingDirection);
                isPushing = true;
                return;
            }
            return;
        }
    }

    public void WhilePushing(Vector2 movementInput) {
        if(_currentPushedObject == null) {
            isPushing = false;
            return;
        }

        isPushing = _currentPushedObject.IsPushable();

        if(!isPushing)
            return;



        if(_playerTransform.position.y + _objRelativePos.y < _currentPushedObject.transform.position.y - 0.1f ||
            _playerTransform.position.y + _objRelativePos.y > _currentPushedObject.transform.position.y + 0.1f) {
            _currentPushedObject.StopPushPull(_playerTransform);
            isPushing = _currentPushedObject.IsPushable();
            return;
        }

        _playerTransform.rotation = Quaternion.LookRotation(_pushingDirection);

        if(movementInput == Vector2.zero)
            return;
        Vector3 convertedMovenentInput = new Vector3(movementInput.x, 0f, movementInput.y).normalized;

        /* ////////////// OLD Version /////////////////

        Vector3 forwardProjectedDir = Vector3.Project(convertedMovenentInput, _pushingDirection);

        Vector3 sideProjectedDrection = Vector3.Project(convertedMovenentInput, new Vector3(_pushingDirection.z, 0f, -_pushingDirection.x));

        if(forwardProjectedDir.magnitude > sideProjectedDrection.magnitude) {
            if(forwardProjectedDir == _pushingDirection) {
                _rigidbody.velocity = forwardProjectedDir.normalized * _pushingSpeed + Vector3.up * _rigidbody.velocity.y;
            }
            else {
                _rigidbody.velocity = forwardProjectedDir.normalized * _pullingSpeed + Vector3.up * _rigidbody.velocity.y;
            }
        }
        else {
            _rigidbody.velocity = sideProjectedDrection.normalized * _pullingSpeed + Vector3.up * _rigidbody.velocity.y;
        }
        //////////////////////// */

        _rigidbody.velocity = convertedMovenentInput.normalized * _pushingSpeed + Vector3.up * _rigidbody.velocity.y;
    }

    public PushableObject GetPushedObject() {
        return _currentPushedObject;
    }

    public void DrawGizmos() {
        if(_config._grabBoxDetectionPostion == null)
            return;

        Gizmos.matrix = _playerTransform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            _config._grabBoxDetectionPostion,
            _config._grabBoxDetectionDimension
            );

        Gizmos.matrix = _playerTransform.worldToLocalMatrix;
    }
}
