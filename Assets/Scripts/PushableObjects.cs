using UnityEngine;

public class PushableObject : MonoBehaviour
{
    [Header("Pushable Object Settings")]
    [SerializeField] private float pushForce = 100f; // Increase this force if the object is not moving as expected
    private Rigidbody _rigidbody;
    private bool _isBeingPushed = false;
    private Vector3 _pushDirection;

    private void Awake()
    {
        // Ensure there is a Rigidbody
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("PushableObject requires a Rigidbody component.");
        }
    }

    private void FixedUpdate()
    {
        if (_isBeingPushed)
        {
            // Apply force continuously in the push direction
            _rigidbody.AddForce(_pushDirection * pushForce, ForceMode.Acceleration);
            Debug.Log("Applying push force in direction: " + _pushDirection);
        }
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
    }
}