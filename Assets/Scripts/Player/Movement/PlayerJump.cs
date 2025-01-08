using UnityEngine;

public class PlayerJump
{
    private readonly Rigidbody _rigidbody;
    private readonly RuntimeJumpConfig _config;
    private readonly PlayerMovement _movement;
    private GroundDetector _groundDetector;

    private bool _isOnBeam;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;
    private int _airJumpsPerformed;
    private bool _isJumping;
    private bool _wasGroundedLastFrame;

    // Base jump parameters
    private float _baseGravity;
    private float _baseJumpVelocity;
    private float _runningJumpVelocity;


    public bool IsJumping => _isJumping;

    public PlayerJump(Rigidbody rigidbody, RuntimeJumpConfig config, PlayerMovement movement)
    {
        _rigidbody = rigidbody;
        _config = config;
        InitializeJumpParameters();
        _movement = movement;
    }

    public void UpdateJump(PlayerInputData input, bool isGrounded, Vector3 lastInputDirection, bool wasAtRunSpeed, bool isOnBeam)
    {
        _isOnBeam = isOnBeam;

        if (_isOnBeam) return;
        UpdateTimers(isGrounded);
        HandleJumpStart(input, isGrounded, lastInputDirection, wasAtRunSpeed);
        HandleJumpCancellation(input);
        ApplyGravity(input.IsJumpHeld);

        _wasGroundedLastFrame = isGrounded;
    }

    private void InitializeJumpParameters()
    {
        // Calculate base jump parameters
        _baseGravity = (-2f * _config.jumpHeight) / (_config.timeToJumpApex * _config.timeToJumpApex);
        _baseJumpVelocity = Mathf.Abs(_baseGravity) * _config.timeToJumpApex;

        // Calculate total time in air
        float totalJumpTime = 2f * _config.timeToJumpApex;

        // Calculate required horizontal speeds
        float walkHorizontalSpeed = _config.desiredJumpLengthWalk / totalJumpTime;
        float runHorizontalSpeed = _config.desiredJumpLengthRun / totalJumpTime;
    }

    private void UpdateTimers(bool isGrounded)
    {
        if (isGrounded)
        {
            if (!_wasGroundedLastFrame)
            {
                CorrectGroundPosition();
                _movement.OnLanding();
            }
            _coyoteTimeCounter = _config.coyoteTimeDuration;
            _airJumpsPerformed = 0;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        if (_jumpBufferCounter > 0f)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void HandleJumpStart(PlayerInputData input, bool isGrounded, Vector3 lastInputDirection,bool wasAtRunSpeed)
    {
        // Add isOnBeam parameter
        bool isOnBeam = (_groundDetector as GroundDetector)?.IsOnBeam ?? false;

        // Don't allow jump if on beam
        if (isOnBeam)
        {
            return;
        }

        if (input.IsJumpPressed)
        {
            _jumpBufferCounter = _config.jumpBufferDuration;
        }

        bool canJump = isGrounded ||
                      (_config.enableCoyoteTime && _coyoteTimeCounter > 0f) ||
                      (_config.maxAirJumps > 0 && _airJumpsPerformed < _config.maxAirJumps);

        if (_jumpBufferCounter > 0f && canJump)
        {
            ExecuteJump(isGrounded, lastInputDirection, input,wasAtRunSpeed);
        }
    }

    private void ExecuteJump(bool isGrounded, Vector3 direction, PlayerInputData input,bool wasAtRunSpeed)
    {
        // Calculate base jump velocity
        Vector3 jumpVelocityVector = Vector3.up * _baseJumpVelocity;
        Debug.Log($"Jump Started - Initial Velocity: {_baseJumpVelocity}");

        // Get current horizontal velocity
        Vector3 horizontalVelocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);

        if (direction != Vector3.zero)
        {
            // Apply the appropriate multiplier based on running state
            float multiplier = wasAtRunSpeed ? _config.runAirVelocityMultiplier : _config.walkAirVelocityMultiplier;
            horizontalVelocity *= multiplier;

            jumpVelocityVector.x = horizontalVelocity.x;
            jumpVelocityVector.z = horizontalVelocity.z;
        }

        // Apply the velocity
        _rigidbody.velocity = jumpVelocityVector;

        _jumpBufferCounter = 0f;
        _coyoteTimeCounter = 0f;
        _isJumping = true;

        if (!isGrounded && !(_config.enableCoyoteTime && _coyoteTimeCounter > 0f))
        {
            _airJumpsPerformed++;
        }
    }

    private void HandleJumpCancellation(PlayerInputData input)
    {
        if (input.WasJumpReleased && _isJumping && _rigidbody.velocity.y > 0)
        {
            var velocity = _rigidbody.velocity;
            velocity.y *= 0.5f;
            _rigidbody.velocity = velocity;
        }
    }

    private void ApplyGravity(bool isJumpHeld)
    {
        float gravityMultiplier = DetermineGravityMultiplier(isJumpHeld);
        Debug.Log($"Going Up - gravityMultiplier: {gravityMultiplier}");
        Vector3 gravity = new Vector3(0f, _baseGravity * gravityMultiplier, 0f);
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);
        LimitFallSpeed();
    }

    private float DetermineGravityMultiplier(bool isJumpHeld)
    {
        float verticalVelocity = _rigidbody.velocity.y;

        if (verticalVelocity > 0.01f)
        {
            return _config.upwardMovementMultiplier; // Full jump height
        }
        if (verticalVelocity < -0.01f)
        {
            if (_wasGroundedLastFrame)
            {
                _isJumping = false;
            }
            // More gradual increase in fall speed
            float fallProgress = Mathf.Abs(verticalVelocity) / _config.fallSpeedLimit;
            return Mathf.Lerp(1f, _config.downwardMovementMultiplier, fallProgress);
        }

        return 1f;
    }

    private void LimitFallSpeed()
    {
        if (_rigidbody.velocity.y < -_config.fallSpeedLimit)
        {
            Vector3 velocity = _rigidbody.velocity;
            velocity.y = -_config.fallSpeedLimit;
            _rigidbody.velocity = velocity;
        }
    }

    private void CorrectGroundPosition()
    {
        if (_rigidbody != null)
        {
            if (Mathf.Abs(_rigidbody.velocity.y) < 0.1f)
            {
                Vector3 velocity = _rigidbody.velocity;

                // Zero out Y velocity
                velocity.y = 0;

                // Reduce horizontal movement on landing if no input
                if (_movement.LastInputDirection == Vector3.zero)
                {
                    velocity.x = 0;
                    velocity.z = 0;
                }

                _rigidbody.velocity = velocity;
            }
        }
    }

    public void UpdateJumpParameters()
    {
        InitializeJumpParameters();
    }

    public void Reset()
    {
        _isJumping = false;
        _airJumpsPerformed = 0;
        _jumpBufferCounter = 0f;
        _coyoteTimeCounter = 0f;
        if (_rigidbody != null)
        {
            var velocity = _rigidbody.velocity;
            velocity.y = 0f;
            _rigidbody.velocity = velocity;
        }
    }
}