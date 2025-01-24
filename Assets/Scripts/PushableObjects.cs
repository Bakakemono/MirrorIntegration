using JetBrains.Annotations;
using Mirror;
using Steamworks;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

// Force the object to have a Rigidbody
[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour {
    [Header("Pushable Object Settings")]
    [SerializeField] private float pushForce = 100f; // Increase this force if the object is not moving as expected
    private Rigidbody _rigidbody;
    [SerializeField] LayerMask _groundLayer;

    [SerializeField, Range(0.1f, 10f)] float _pushSpeed = 1.0f;
    [SerializeField, Range(0.1f, 10f)] float _pullSpeed = 1.0f;
    Transform _playerTransform;

    Transform _secondPlayerTransform;
    Vector3 _relativePosPlayer;

    [SerializeField] bool _twoPlayerNeeded = false;

    bool _canBePush = false;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        if(_playerTransform == null || (_twoPlayerNeeded && _secondPlayerTransform == null))
            return;

        Vector3 newPos = _relativePosPlayer + _playerTransform.position;
        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
    }

    /// <summary>
    /// Stops the push or pull.
    /// </summary>
    public void StopPushPull(Transform pushingPlayerTransform) {
        if(_playerTransform == pushingPlayerTransform) {
            _playerTransform = _secondPlayerTransform;
            if(_playerTransform != null)
                _relativePosPlayer = transform.position - _playerTransform.position;
        }
        else if(_secondPlayerTransform == pushingPlayerTransform) {
            _secondPlayerTransform = null;
        }

        _rigidbody.mass = 100f;
        _canBePush = false;
    }

    public bool StartPushPull(Transform pushingPlayerTransform) {
        if(_playerTransform == null) {
            _playerTransform = pushingPlayerTransform;
            _relativePosPlayer = transform.position - _playerTransform.position;
            _rigidbody.mass = 1.0f;
            _canBePush = !_twoPlayerNeeded;
            return true;
        }
        else if(_playerTransform != pushingPlayerTransform && _twoPlayerNeeded) {
            _secondPlayerTransform = pushingPlayerTransform;
            _canBePush = true;
            return true;
        }
        else {
            return false;
        }
    }

    public float GetPullingSpeed() {
        return _pullSpeed;
    }

    public float GetPushingSpeed() {
        return _pushSpeed;
    }

    public Vector3 GetPushDirection() {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, 10f, _groundLayer);

        Vector3 perpendicularToNormal = new Vector3(hit.normal.x, -hit.normal.z, hit.normal.y);

        Vector3 projectedForward = Vector3.Project(perpendicularToNormal, transform.forward);
        Vector3 projectedVRight = Vector3.Project(perpendicularToNormal, transform.right);
        Vector3 projectedUp = Vector3.Project(perpendicularToNormal, transform.up);
        
        if(projectedForward.sqrMagnitude > projectedVRight.sqrMagnitude && projectedForward.sqrMagnitude > projectedUp.sqrMagnitude) {
            return transform.forward;
        }
        else if(projectedVRight.sqrMagnitude > projectedForward.sqrMagnitude && projectedVRight.sqrMagnitude > projectedUp.sqrMagnitude) {
            return transform.right;
        }
        else {
            return transform.up;
        }
    }
    
    public bool IsPushable() {
        return _canBePush;
    }
}