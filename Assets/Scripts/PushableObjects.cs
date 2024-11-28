using JetBrains.Annotations;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

// Force the object to have a Rigidbody
[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    [Header("Pushable Object Settings")]
    [SerializeField] private float pushForce = 100f; // Increase this force if the object is not moving as expected
    private Rigidbody _rigidbody;
    private bool _isBeingPushed = false;
    private Vector3 _pushDirection;

    [SerializeField, Range(0.1f, 10f)] float _pushSpeed = 1.0f;
    [SerializeField, Range(0.1f, 10f)] float _pullSpeed = 1.0f;
    Transform _playerTransform;
    Vector3 _relativePosPlayer;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _pushDirection = transform.forward;
    }

    private void FixedUpdate()
    {
        //if (_isBeingPushed)
        //{
        //    // Apply force continuously in the push direction
        //    _rigidbody.AddForce(_pushDirection * pushForce, ForceMode.Acceleration);
        //    Debug.Log("Applying push force in direction: " + _pushDirection);
        //}
    }

    private void Update() {
        if(_playerTransform == null)
            return;

        Vector3 newPos = _relativePosPlayer + _playerTransform.position;
        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
    }

    /// <summary>
    /// Starts the push or pull on the object.
    /// </summary>
    /// <param name="direction">The direction to push or pull the object.</param>
    public void StartPushPull(Vector3 direction)
    {
        _isBeingPushed = true;
        _pushDirection = direction.normalized; // Ensure direction is normalized
        Debug.Log("Start pushing/pulling with direction: " + _pushDirection);
    }

    /// <summary>
    /// Stops the push or pull.
    /// </summary>
    public void StopPushPull()
    {
        _isBeingPushed = false;
        _rigidbody.velocity = Vector3.zero; // Stop the object when push/pull is deactivated
        Debug.Log("Stopped pushing/pulling.");

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
        return _pushDirection;
    }
}