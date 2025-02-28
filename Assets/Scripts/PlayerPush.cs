using Steamworks;
using System;
using UnityEngine;

public class PlayerPush {
    private Transform _playerTransform;
    private Rigidbody _rigidbody;

    RuntimePushConfig _config;

    PushableObject _currentPushedObject;
    Vector3 _objRelativePos = Vector3.zero;

    bool isPushing = false;
    public bool _isPushing {
        get {
            return isPushing;
        }
    }
    float _pushingSpeed = 0f;


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

                // Register current relative position.
                _objRelativePos = _currentPushedObject.transform.position - _playerTransform.position;

                //_playerTransform.rotation = Quaternion.LookRotation(_pushingDirection);
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

        Vector3 convertedMovenentInput = new Vector3(movementInput.x, 0f, movementInput.y).normalized;

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
