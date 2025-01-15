using System;
using UnityEngine;

public class PlayerPush {
    private Transform _transform;
    private Rigidbody _rigidbody;

    RuntimePushConfig _config;
    
    Vector3 _pushingDirection = Vector3.zero;
    PushableObject _currentPushedObject;
    
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
        _transform = transform;
        _rigidbody = rigidBody;
    }

    // Grab the object in the detection area.
    // Return true if a grabable object has been found the lock classic control from player then call WhilePushing(Vector2 movementInput).
    // Return false if there is no object to grab or if the player in release on in hand.
    public void Grab() {
        if(isPushing) {
            isPushing = false;
            _currentPushedObject.StopPushPull();
            return;
        }
        else {
            Collider[] _detectedObjects =
                Physics.OverlapBox(
                    _transform.TransformPoint(_config._grabBoxDetectionPostion),
                    _config._grabBoxDetectionDimension / 2f,
                    _transform.rotation,
                    _config._pushableObjectLayerMask
                    );

            if(_detectedObjects.Length > 0) {
                _currentPushedObject = _detectedObjects[0].GetComponent<PushableObject>();
                _currentPushedObject.StartPushPull(_transform.gameObject);
                _pushingSpeed = _currentPushedObject.GetPushingSpeed();
                _pullingSpeed = _currentPushedObject.GetPullingSpeed();

                // Register current elevation for fall detection.
                _pushedObjectHeight = _currentPushedObject.transform.position.y;
                _heightWhenPushing = _transform.position.y;

                // Determine the direction in which the player is going to push.
                Vector3 localPos = _currentPushedObject.transform.InverseTransformPoint(_transform.position);

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

                _transform.rotation = Quaternion.LookRotation(_pushingDirection);
                isPushing = true;
                return;
            }
            return;
        }
    }

    public void WhilePushing(Vector2 movementInput) {
        if(!isPushing)
            return;

        if((_transform.position.y < _heightWhenPushing - 0.1f || _transform.position.y > _heightWhenPushing + 0.1f) ||
            (_currentPushedObject.transform.position.y < _pushedObjectHeight - 0.1f || _currentPushedObject.transform.position.y > _pushedObjectHeight + 0.1f)
            ) {
            isPushing = false;
            _currentPushedObject.StopPushPull();
            return;
        }

        Vector3 convertedMovenentInput = new Vector3(movementInput.x, 0f, movementInput.y).normalized;

        Vector3 projectedDirection = Vector3.Project(convertedMovenentInput, _pushingDirection).normalized;

        if(projectedDirection == _pushingDirection) {
            _rigidbody.velocity = projectedDirection * _pushingSpeed + Vector3.up * _rigidbody.velocity.y;
        }
        else {
            _rigidbody.velocity = projectedDirection * _pullingSpeed + Vector3.up * _rigidbody.velocity.y;
        }

        _transform.rotation = Quaternion.LookRotation(_pushingDirection);
    }

    public PushableObject GetPushedObject() {
        return _currentPushedObject;
    }

    public void DrawGizmos() {
        if(_config._grabBoxDetectionPostion == null)
            return;

        Gizmos.matrix = _transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            _config._grabBoxDetectionPostion,
            _config._grabBoxDetectionDimension
            );

        Gizmos.matrix = _transform.worldToLocalMatrix;
    }
}
