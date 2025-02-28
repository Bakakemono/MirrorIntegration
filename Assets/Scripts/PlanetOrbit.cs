using UnityEngine;
using System;

public class PlanetOrbit : MonoBehaviour
{
    [SerializeField] private Transform sun;
    [SerializeField] private float orbitSpeed = 10f;
    [SerializeField] private float transitionDuration = 2f; // Duration to stop/start

    private bool _isOrbiting = true;
    private float _currentSpeed;
    private float _targetSpeed;

    // Speed state tracking
    public bool IsAtFullSpeed { get; private set; } = true;
    public bool IsStopped { get; private set; } = false;

    // Events for state changes
    public event Action OnFullSpeedReached;
    public event Action OnFullStop;

    private void Start()
    {
        _currentSpeed = orbitSpeed;
        _targetSpeed = orbitSpeed;
        IsAtFullSpeed = true;
    }

    void Update()
    {
        if (sun != null)
        {
            // Calculate how much to change speed this frame
            float speedChange = (orbitSpeed / transitionDuration) * Time.deltaTime;

            // Smoothly adjust speed
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, _targetSpeed, speedChange);

            // Check for state changes
            CheckSpeedState();

            // Rotate around the sun
            transform.RotateAround(sun.position, Vector3.up, _currentSpeed * Time.deltaTime);
        }
    }

    private void CheckSpeedState()
    {
        // Check if reached full speed
        if (!IsAtFullSpeed && Mathf.Approximately(_currentSpeed, orbitSpeed))
        {
            IsAtFullSpeed = true;
            IsStopped = false;
            OnFullSpeedReached?.Invoke();
        }

        // Check if fully stopped
        if (!IsStopped && Mathf.Approximately(_currentSpeed, 0f))
        {
            IsStopped = true;
            IsAtFullSpeed = false;
            OnFullStop?.Invoke();
        }
    }

    public void SetOrbiting(bool value)
    {
        _isOrbiting = value;
        _targetSpeed = value ? orbitSpeed : 0f;

        // Reset state flags when changing direction
        if (value)
        {
            IsStopped = false;
        }
        else
        {
            IsAtFullSpeed = false;
        }
    }
}