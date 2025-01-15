using UnityEngine;

public class PlayerMovement
{
    private readonly Rigidbody _rigidbody;
    private readonly RuntimeMovementConfig _Moveconfig;
    private RuntimeJumpConfig _jumpConfig;

    private float _currentSpeed = 0f;
    private Vector3 _lastInputDirection = Vector3.zero;
<<<<<<< Updated upstream
    private float _airWalkSpeed;
    private float _airRunSpeed;

    private const float RUN_SPEED_THRESHOLD = 0.75f; // 90% de la vitesse de cours
=======
    private const float RUN_SPEED_THRESHOLD = 0.90f; // 90% de la vitesse de cours
>>>>>>> Stashed changes
    private bool _wasAtRunSpeed;  // Pour tracker si on était à vitesse de course

    public Vector3 LastInputDirection => _lastInputDirection;
    public bool WasAtRunSpeed => _wasAtRunSpeed;

    private PlayerJump _jump;

    public void SetJumpReference(PlayerJump jump)
    {
        _jump = jump;
    }

    public PlayerMovement(Rigidbody rigidbody, RuntimeMovementConfig Moveconfig, RuntimeJumpConfig jumpConfig)
    {
        _rigidbody = rigidbody;
        _Moveconfig = Moveconfig;
        _jumpConfig = jumpConfig;
    }

    private void UpdateCurrentSpeedState(bool isGrounded)
    {
        if (isGrounded)
        {
            _wasAtRunSpeed = _currentSpeed >= (_Moveconfig.runSpeed * RUN_SPEED_THRESHOLD);
        }
<<<<<<< Updated upstream
        // En l'air, on garde l'état précédent
        // Ne pas mettre à false quand on est en l'air
    }

    private void CalculateAirSpeeds()
    {
        // Calculate like in the original script
        float totalJumpTime = 2f * _jumpConfig.timeToJumpApex;
        _airWalkSpeed = _jumpConfig.walkAirVelocityMultiplier * _Moveconfig.walkSpeed;
        _airRunSpeed = _jumpConfig.runAirVelocityMultiplier * _Moveconfig.runSpeed;
=======
>>>>>>> Stashed changes
    }

    public void UpdateMovement(PlayerInputData input, bool isGrounded, bool isOnBeam)
    {
        UpdateCurrentSpeedState(isGrounded);
        if (isOnBeam)
        {
            // Only use X component of input
            float xInput = input.MovementDirection.x;
            _currentSpeed = _Moveconfig.walkSpeed * 0.6f;

            // Only apply X movement
            _lastInputDirection = new Vector3(xInput, 0, 0);
            Vector3 Xmovement = _lastInputDirection * _currentSpeed;

            // Force Z velocity to zero when on beam
            _rigidbody.velocity = new Vector3(Xmovement.x, _rigidbody.velocity.y, 0);
            return;  // Exit early, don't process other movement
        }

        Vector3 inputDirection = input.MovementDirection;
        float movementMagnitude = input.MovementMagnitude;

        // Always update last input direction based on current input
        _lastInputDirection = movementMagnitude > 0f ? inputDirection : Vector3.zero;

        if (movementMagnitude > 0f)
        {
            _lastInputDirection = inputDirection;

            if (isGrounded)
            {
                // Ground movement remains the same
                if (input.IsRunning)
                {
                    if (_currentSpeed < _Moveconfig.walkSpeed)
                    {
                        _currentSpeed = _Moveconfig.walkSpeed;
                    }
                    float targetSpeed = _Moveconfig.runSpeed;
                    AccelerateTowardTargetSpeed(targetSpeed, _Moveconfig.accelerationTime);
                }
                else
                {
                    _currentSpeed = _Moveconfig.walkSpeed;
                }
<<<<<<< Updated upstream
                // Apply ground movement
                Vector3 movementSpeed = _lastInputDirection * _currentSpeed;
                _rigidbody.velocity = new Vector3(movementSpeed.x, _rigidbody.velocity.y, movementSpeed.z);
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

                // When no input in air, maintain momentum with very slight decay
                Vector3 currentHorizontalVelocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
                float decay = 0.999f; // Very slight decay
                _rigidbody.velocity = new Vector3(
                    currentHorizontalVelocity.x * decay,
                    _rigidbody.velocity.y,
                    currentHorizontalVelocity.z * decay
                );
            }
=======
>>>>>>> Stashed changes

                Vector3 movement = _lastInputDirection * _currentSpeed;
                _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
            }
        }
        else if (isGrounded)
        {
            _lastInputDirection = Vector3.zero;
            DecelerateToStop(isGrounded);
        }
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
        // Only set speed if we had previous movement
        if (_lastInputDirection != Vector3.zero)
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
        else
        {
            _currentSpeed = 0f;
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