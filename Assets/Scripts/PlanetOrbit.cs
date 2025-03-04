using UnityEngine;
using System;

public class PlanetOrbit : MonoBehaviour
{
    [SerializeField] private Transform sun;
    [SerializeField] private float orbitSpeed = 10f;
    [SerializeField] private float transitionDuration = 2f;

    private bool _isOrbiting = true;
    private float _currentSpeed;
    private float _targetAngle;
    private float _currentAngle;
    private bool _isAligning = false;
    private float _orbitRadius;

    public bool IsAligning => _isAligning;
    public bool IsAtFullSpeed { get; private set; } = true;

    public event Action OnAlignmentComplete;

    private void Start()
    {
        _currentSpeed = orbitSpeed;
        IsAtFullSpeed = true;

        // Calculate initial orbit radius
        if (sun != null)
        {
            _orbitRadius = Vector3.Distance(transform.position, sun.position);
        }
    }

    void Update()
    {
        if (sun == null) return;

        if (_isOrbiting && !_isAligning)
        {
            // Normal orbit mode
            transform.RotateAround(sun.position, Vector3.up, orbitSpeed * Time.deltaTime);
        }
        else if (_isAligning)
        {
            // Smoothly rotate around to target angle
            float angleDelta = Mathf.SmoothDampAngle(_currentAngle, _targetAngle,
                ref _currentSpeed, transitionDuration);

            float angleToRotate = angleDelta - _currentAngle;
            _currentAngle = angleDelta;

            transform.RotateAround(sun.position, Vector3.up, angleToRotate);

            // Check if alignment is complete
            if (Mathf.Abs(Mathf.DeltaAngle(_currentAngle, _targetAngle)) < 0.1f)
            {
                _isAligning = false;
                OnAlignmentComplete?.Invoke();
            }
        }
    }

    public void SetOrbiting(bool value)
    {
        _isOrbiting = value;
        _isAligning = false;

        if (value)
        {
            IsAtFullSpeed = true;
        }
    }

    public void AlignToPosition(Vector3 alignmentPosition)
    {
        if (sun == null) return;

        // Calculate the direction vector from sun to alignment position
        Vector3 alignmentDirection = (alignmentPosition - sun.position).normalized;

        // Calculate the angle in the XZ plane
        float targetAngle = Mathf.Atan2(alignmentDirection.x, alignmentDirection.z) * Mathf.Rad2Deg;

        // Get current angle
        Vector3 relativePos = transform.position - sun.position;
        _currentAngle = Mathf.Atan2(relativePos.x, relativePos.z) * Mathf.Rad2Deg;

        // Set target angle
        _targetAngle = targetAngle;

        // Start alignment process
        _isOrbiting = false;
        _isAligning = true;
        IsAtFullSpeed = false;
    }
}