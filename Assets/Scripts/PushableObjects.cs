using JetBrains.Annotations;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

// Force the object to have a Rigidbody
[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour {
    [Header("Pushable Object Settings")]
    [SerializeField] private float pushForce = 100f; // Increase this force if the object is not moving as expected
    private Rigidbody _rigidbody;

    [SerializeField, Range(0.1f, 10f)] float _pushSpeed = 1.0f;
    [SerializeField, Range(0.1f, 10f)] float _pullSpeed = 1.0f;
    Transform _playerTransform;
    Vector3 _relativePosPlayer;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        if(_playerTransform == null)
            return;

        Vector3 newPos = _relativePosPlayer + _playerTransform.position;
        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
    }

    // TO REMOVE ------
    /// <summary>
    /// Starts the push or pull on the object.
    /// </summary>
    /// <param name="direction">The direction to push or pull the object.</param>
    public void StartPushPull(Vector3 direction) {
    }
    // ----------------

    /// <summary>
    /// Stops the push or pull.
    /// </summary>
    public void StopPushPull() {
        // TO REMOVE ------
        _rigidbody.velocity = Vector3.zero; // Stop the object when push/pull is deactivated
        // ----------------

        _playerTransform = null;
        _rigidbody.mass = 100f;
    }

    public void StartPushPull(GameObject _pushingPlayer) {
        _playerTransform = _pushingPlayer.transform;
        _relativePosPlayer = transform.position - _playerTransform.position;
        _rigidbody.mass = 1.0f;
    }

    public float GetPullingSpeed() {
        return _pullSpeed;
    }

    public float GetPushingSpeed() {
        return _pushSpeed;
    }

    public Vector3 GetPushDirection() {
        return transform.forward;
    }
}