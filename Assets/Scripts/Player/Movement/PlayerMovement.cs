using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement
{
    private readonly Rigidbody _rigidbody;
    private readonly RuntimeMovementConfig _Moveconfig;
    private RuntimeJumpConfig _jumpConfig;

    private float _currentSpeed = 0f;
    private Vector3 _lastInputDirection = Vector3.zero;
    private float _airWalkSpeed;
    private float _airRunSpeed;

    private const float RUN_SPEED_THRESHOLD = 0.90f; // 90% de la vitesse de cours
    private bool _wasAtRunSpeed;  // Pour tracker si on était à vitesse de course

    public Vector3 LastInputDirection => _lastInputDirection;
    public bool WasAtRunSpeed => _wasAtRunSpeed;


    public PlayerMovement(Rigidbody rigidbody, RuntimeMovementConfig Moveconfig, RuntimeJumpConfig jumpConfig)
    {
        _rigidbody = rigidbody;
        _Moveconfig = Moveconfig;
        _jumpConfig = jumpConfig; ;
    }

    private void UpdateCurrentSpeedState(bool isGrounded)
    {
        // On vérifie la vitesse uniquement quand on est au sol
        if (isGrounded)
        {
            _wasAtRunSpeed = _currentSpeed >= (_Moveconfig.runSpeed * RUN_SPEED_THRESHOLD);
        }
        // En l'air, on garde l'état précédent
        // Ne pas mettre à false quand on est en l'air
    }

    private void CalculateAirSpeeds()
    {
        // Calculate like in the original script
        float totalJumpTime = 2f * _jumpConfig.timeToJumpApex;
        _airWalkSpeed = _jumpConfig.desiredJumpLengthWalk / totalJumpTime;
        _airRunSpeed = _jumpConfig.desiredJumpLengthRun / totalJumpTime;

        Debug.Log($"Air speeds calculated - Walk: {_airWalkSpeed}, Run: {_airRunSpeed}");
    }

    public void UpdateMovement(PlayerInputData input, bool isGrounded)
    {
        CalculateAirSpeeds();
        UpdateCurrentSpeedState(isGrounded);

        Vector3 inputDirection = input.MovementDirection;
        float movementMagnitude = input.MovementMagnitude;

        if (movementMagnitude > 0f)
        {
            _lastInputDirection = inputDirection;

            float targetSpeed;
            if (isGrounded)
            {
                if (input.IsRunning)
                {
                    // Si on commence à courir, on commence à la vitesse de marche
                    if (_currentSpeed < _Moveconfig.walkSpeed)
                    {
                        _currentSpeed = _Moveconfig.walkSpeed;
                    }
                    targetSpeed = _Moveconfig.runSpeed;
                    AccelerateTowardTargetSpeed(targetSpeed, _Moveconfig.accelerationTime);
                }
                else
                {
                    // Vitesse de marche instantanée
                    _currentSpeed = _Moveconfig.walkSpeed;
                }
            }
            else
            {
                // Si on était à vitesse de course au moment du saut, garder la vitesse de course
                if (_wasAtRunSpeed)
                {
                    targetSpeed = _airRunSpeed;
                }
                else
                {
                    targetSpeed = _airWalkSpeed;
                }
                _currentSpeed = targetSpeed;
            }

            float accelerationTime = isGrounded ?
                _Moveconfig.accelerationTime :
                _Moveconfig.airAccelerationTime;
        }
        else
        {
            DecelerateToStop(isGrounded);
        }

        Vector3 movement = _lastInputDirection * _currentSpeed;
        _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
    }

    public void UpdateAirSpeedParameters()
    {
        CalculateAirSpeeds();
    }

    private void AccelerateTowardTargetSpeed(float targetSpeed, float accelerationTime)
    {
        float speedDifference = targetSpeed - _currentSpeed;
        float rate = Mathf.Abs(speedDifference) / accelerationTime;
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, rate * Time.deltaTime);
    }

    private void DecelerateToStop(bool isGrounded)
    {
        float decelerationTime = isGrounded
            ? _Moveconfig.decelerationTime
            : _Moveconfig.airDecelerationTime;

        float decelerationRate = _currentSpeed / decelerationTime;
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, decelerationRate * Time.deltaTime);
    }

    public void OnLanding()
    {
        if (_wasAtRunSpeed)
        {
            _currentSpeed = _Moveconfig.runSpeed;
        }
        else
        {
            _currentSpeed = _Moveconfig.walkSpeed;
        }
    }

    public void Reset()
    {
        _currentSpeed = 0f;
        _lastInputDirection = Vector3.zero;
        if (_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.zero;
        }
    }
}
