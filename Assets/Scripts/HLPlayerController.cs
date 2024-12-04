using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class HLPlayerController : MonoBehaviour
{
    PlayerControl _playerControl;
    InputAction _movementAction;
    InputAction _runAction;
    InputAction _respawnAction;
    InputAction _jumpAction;

    [Header("Girl Body Params")]
    [SerializeField] private GameObject _GirlModel;
    [SerializeField] private Vector3 _girlSize;

    [Header("Boy Body Params")]
    [SerializeField] private GameObject _BoyModel;
    [SerializeField] private Vector3 _BoySize;

    [Header("Ground Movement Parameters")]
    [SerializeField] float _walkSpeed = 2f;
    [SerializeField] float _runSpeed = 5f;
    [SerializeField] float _accelerationTime = 0.2f;
    [SerializeField] float _decelerationTime = 0.2f;

    [Header("Air Movement Parameters")]
    [SerializeField] float _airWalkSpeed = 2f;
    [SerializeField] float _airRunSpeed = 5f;
    [SerializeField] float _airAccelerationTime = 0.2f;
    [SerializeField] float _airDecelerationTime = 0.2f;

    float _currentSpeed = 0f;

    [Header("Jump Parameters")]
    [SerializeField, Tooltip("Maximum jump height")] float _jumpHeight = 2f;
    [SerializeField, Tooltip("Time to reach the apex of the jump")] float _timeToJumpApex = 0.5f;
    [SerializeField, Tooltip("Desired horizontal distance to cover during a jump when walking")] float _desiredJumpLengthWalk = 5f;
    [SerializeField, Tooltip("Desired horizontal distance to cover during a jump when running")] float _desiredJumpLengthRun = 7f;
    [SerializeField, Tooltip("Gravity multiplier when going up")] float _upwardMovementMultiplier = 1f;
    [SerializeField, Tooltip("Gravity multiplier when coming down")] float _downwardMovementMultiplier = 4f;
    [SerializeField, Tooltip("Gravity multiplier when jump button is released early")] float _jumpCutOff = 2f;
    [SerializeField, Tooltip("Maximum fall speed")] float _fallSpeedLimit = 20f;
    [SerializeField, Tooltip("Maximum number of air jumps")] int _maxAirJumps = 0;



    [SerializeField] bool _enableCoyoteTime = true;
    [SerializeField] bool _enableJumpBuffer = true;
    [SerializeField] float _coyoteTimeDuration = 0.15f;
    [SerializeField] float _jumpBufferDuration = 0.15f;
    float _coyoteTimeCounter;
    float _jumpBufferCounter;
    int _airJumpsPerformed = 0;
    bool _isJumping = false;
    bool _isGrounded = false;
    bool _pressingJump = false;
    bool _wasGroundedLastFrame = false;

    float _gravity;
    float _jumpVelocity;

    [SerializeField] Transform _cameraPosition;
    Rigidbody _rigidbody;
    CapsuleCollider _capsuleCollider;

    [SerializeField] bool _isBoy = false;
    [SerializeField] bool _isGirl = false;

    // LayerMask for ground detection
    [SerializeField] LayerMask _groundLayer;

    private Vector3 _spawnPosition;
    private Vector3 _lastInputDirection = Vector3.zero;

    // Variables to track changes in serialized fields
    private float _prevJumpHeight;
    private float _prevTimeToJumpApex;
    private float _prevDesiredJumpLengthWalk;
    private float _prevDesiredJumpLengthRun;
    private float _previousMovementInputMagnitude = 0f;




    private bool _isRunning = false; // Variable to track running state

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        Transform cameraTransform = Camera.main.transform;
        cameraTransform.position = _cameraPosition.position;
        cameraTransform.rotation = _cameraPosition.rotation;
        cameraTransform.parent = transform;

        _playerControl = new PlayerControl();
        _movementAction = _playerControl.Player.Movement;
        _movementAction.Enable();

        _jumpAction = _playerControl.Player.Jump;
        _jumpAction.Enable();

        _runAction = _playerControl.Player.Run;
        _runAction.Enable();

        // Initialize respawn action
        _respawnAction = _playerControl.Player.Respawn;
        _respawnAction.performed += OnRespawn;
        _respawnAction.Enable();

        // Calculate gravity and jump velocity
        _gravity = (-2f * _jumpHeight) / (_timeToJumpApex * _timeToJumpApex);
        _jumpVelocity = Mathf.Abs(_gravity) * _timeToJumpApex;

        // Calculate total time in air
        float totalJumpTime = 2f * _timeToJumpApex;

        // Calculate required horizontal speed for walking
        float requiredHorizontalSpeedWalk = _desiredJumpLengthWalk / totalJumpTime;

        // Calculate required horizontal speed for running
        float requiredHorizontalSpeedRun = _desiredJumpLengthRun / totalJumpTime;

        // Set air movement speeds
        _airWalkSpeed = requiredHorizontalSpeedWalk;
        _airRunSpeed = requiredHorizontalSpeedRun;

        // Disable default gravity
        _rigidbody.useGravity = false;

        // Set initial spawn position
        _spawnPosition = transform.position;

        // Initialize previous values
        _prevJumpHeight = _jumpHeight;
        _prevTimeToJumpApex = _timeToJumpApex;
        _prevDesiredJumpLengthWalk = _desiredJumpLengthWalk;
        _prevDesiredJumpLengthRun = _desiredJumpLengthRun;

        // Calculate initial derived parameters
        CalculateDerivedParameters();
    }


    private void Update()
    {
        GroundCheck();

        HandleInput();

        HandleTimers();

        Movement();

        HandleModelSwitching();

        // Check for changes in parameters and recalculate if needed
        if (_jumpHeight != _prevJumpHeight ||
            _timeToJumpApex != _prevTimeToJumpApex ||
            _desiredJumpLengthWalk != _prevDesiredJumpLengthWalk ||
            _desiredJumpLengthRun != _prevDesiredJumpLengthRun)
        {
            // Recalculate derived parameters
            CalculateDerivedParameters();

            // Update previous values
            _prevJumpHeight = _jumpHeight;
            _prevTimeToJumpApex = _timeToJumpApex;
            _prevDesiredJumpLengthWalk = _desiredJumpLengthWalk;
            _prevDesiredJumpLengthRun = _desiredJumpLengthRun;
        }
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    private void HandleInput()
    {
        // Read jump input
        _pressingJump = _jumpAction.IsPressed();

        // Check if jump was pressed this frame
        if (_jumpAction.WasPressedThisFrame())
        {
            _jumpBufferCounter = _jumpBufferDuration;
        }

        // Read run input
        _isRunning = _runAction.IsPressed();
    }

    private void HandleTimers()
    {
        // Handle coyote time counter
        if (_isGrounded)
        {
            _coyoteTimeCounter = _coyoteTimeDuration;
            _airJumpsPerformed = 0; // Reset air jumps
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        // Handle jump buffer counter
        if (_jumpBufferCounter > 0f)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void Movement()
    {
        Vector2 movementInput = _movementAction.ReadValue<Vector2>();
        Vector3 inputDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        float movementInputMagnitude = movementInput.magnitude;

        // Determine target speed and acceleration based on grounded state
        float targetSpeed;
        float accelerationTime;
        float decelerationTime;
        float maxAllowedSpeed;

        if (_isGrounded)
        {
            // Grounded movement parameters
            targetSpeed = _isRunning ? _runSpeed : _walkSpeed;
            accelerationTime = _accelerationTime;
            decelerationTime = _decelerationTime;

            // Adjust maxAllowedSpeed to prevent immediate clamping when decelerating
            maxAllowedSpeed = Mathf.Max(targetSpeed, _currentSpeed);
        }
        else
        {
            // Airborne movement parameters
            targetSpeed = _isRunning ? _airRunSpeed : _airWalkSpeed;
            accelerationTime = _airAccelerationTime;
            decelerationTime = _airDecelerationTime;
            maxAllowedSpeed = float.MaxValue; // No speed limit in the air
        }

        if (movementInputMagnitude > 0f)
        {
            // Update last input direction
            _lastInputDirection = inputDirection;

            // Check if movement input just started
            if (_previousMovementInputMagnitude == 0f)
            {
                // Starting from standstill, set current speed to walk speed
                _currentSpeed = _walkSpeed;
            }

            // Determine if we are accelerating or decelerating
            float speedDifference = targetSpeed - _currentSpeed;

            float rate;
            if (speedDifference > 0f)
            {
                // Accelerate
                rate = Mathf.Abs(speedDifference) / accelerationTime;
            }
            else if (speedDifference < 0f)
            {
                // Decelerate
                rate = Mathf.Abs(speedDifference) / decelerationTime;
            }
            else
            {
                rate = 0f;
            }

            // Adjust current speed towards target speed
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, rate * Time.deltaTime);
        }
        else
        {
            // Decelerate to zero when no movement input
            float decelerationRate = _currentSpeed / decelerationTime;
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, decelerationRate * Time.deltaTime);

            // Set _currentSpeed to zero if it's very close to zero
            if (Mathf.Abs(_currentSpeed) < 0.01f)
            {
                _currentSpeed = 0f;
            }
        }

        // Clamp _currentSpeed to maxAllowedSpeed when accelerating
        if (_isGrounded && _currentSpeed > maxAllowedSpeed)
        {
            _currentSpeed = maxAllowedSpeed;
        }

        Vector3 playerMovement = _lastInputDirection * _currentSpeed;

        // Apply horizontal movement
        _rigidbody.velocity = new Vector3(
            playerMovement.x,
            _rigidbody.velocity.y,
            playerMovement.z
        );

        // Handle jumping
        bool canJump = (_isGrounded || (_enableCoyoteTime && _coyoteTimeCounter > 0f) || (_maxAirJumps > 0 && _airJumpsPerformed < _maxAirJumps));

        if (_jumpBufferCounter > 0f && canJump)
        {
            // Reset vertical velocity before jumping
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
            _rigidbody.velocity += Vector3.up * _jumpVelocity;
            _jumpBufferCounter = 0f;
            _coyoteTimeCounter = 0f;
            _isJumping = true;

            if (!_isGrounded && !(_enableCoyoteTime && _coyoteTimeCounter > 0f))
            {
                _airJumpsPerformed++;
            }
        }

        // Update previous movement input magnitude
        _previousMovementInputMagnitude = movementInputMagnitude;
    }




    private void ApplyGravity()
    {
        float gravityMultiplier = 1f;
        float verticalVelocity = _rigidbody.velocity.y;

        if (verticalVelocity > 0.01f)
        {
            // Ascending
            if (_pressingJump && _isJumping)
            {
                gravityMultiplier = 1f; // Normal gravity during ascent when holding jump
            }
            else
            {
                gravityMultiplier = _jumpCutOff; // Apply extra gravity when jump is released
            }
        }
        else if (verticalVelocity < -0.01f)
        {
            // Descending
            gravityMultiplier = _downwardMovementMultiplier; // Apply increased gravity when descending
        }
        else
        {
            gravityMultiplier = 1f;
        }

        // Apply custom gravity
        Vector3 gravityForce = new Vector3(0f, _gravity * gravityMultiplier, 0f);
        _rigidbody.AddForce(gravityForce, ForceMode.Acceleration);

        // Limit fall speed
        if (_rigidbody.velocity.y < -_fallSpeedLimit)
        {
            Vector3 vel = _rigidbody.velocity;
            vel.y = -_fallSpeedLimit;
            _rigidbody.velocity = vel;
        }
    }

    private void GroundCheck()
    {
        float extraHeight = 0.1f;
        float radius = _capsuleCollider.radius * 0.9f; // Slightly reduce radius to avoid clipping issues
        Vector3 origin = transform.position + Vector3.up * (_capsuleCollider.height / 2 - _capsuleCollider.radius);
        float maxDistance = (_capsuleCollider.height / 2 - _capsuleCollider.radius) + extraHeight;

        // Use SphereCast to detect ground
        RaycastHit hit;
        if (Physics.SphereCast(origin, radius, Vector3.down, out hit, maxDistance, _groundLayer))
        {
            _isGrounded = true;

            if (!_wasGroundedLastFrame)
            {
                // We just landed
                _isJumping = false;

                // Reset _currentSpeed to ground target speed
                Vector2 movementInput = _movementAction.ReadValue<Vector2>();
                float movementMagnitude = movementInput.magnitude;
                float targetGroundSpeed = (_isRunning ? _runSpeed : _walkSpeed) * movementMagnitude;

                _currentSpeed = targetGroundSpeed;
            }
        }
        else
        {
            _isGrounded = false;
        }

        _wasGroundedLastFrame = _isGrounded;
    }

    private void CalculateDerivedParameters()
    {
        // Calculate gravity and jump velocity
        _gravity = (-2f * _jumpHeight) / (_timeToJumpApex * _timeToJumpApex);
        _jumpVelocity = Mathf.Abs(_gravity) * _timeToJumpApex;

        // Calculate total time in air
        float totalJumpTime = 2f * _timeToJumpApex;

        // Calculate required horizontal speed for walking
        float requiredHorizontalSpeedWalk = _desiredJumpLengthWalk / totalJumpTime;

        // Calculate required horizontal speed for running
        float requiredHorizontalSpeedRun = _desiredJumpLengthRun / totalJumpTime;

        // Set air movement speeds
        _airWalkSpeed = requiredHorizontalSpeedWalk;
        _airRunSpeed = requiredHorizontalSpeedRun;
    }


    private void HandleModelSwitching()
    {
        if (_isGirl)
        {
            _GirlModel.SetActive(true);
            _BoyModel.SetActive(false);
            _capsuleCollider.radius = _girlSize.x / 2f;
            _capsuleCollider.height = _girlSize.y;

            _isGirl = false;
        }
        else if (_isBoy)
        {
            _BoyModel.SetActive(true);
            _GirlModel.SetActive(false);
            _capsuleCollider.radius = _BoySize.x / 2f;
            _capsuleCollider.height = _BoySize.y;

            _isBoy = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the ground check sphere
        if (_capsuleCollider != null)
        {
            Gizmos.color = Color.red;
            float radius = _capsuleCollider.radius * 0.9f;
            Vector3 origin = transform.position + Vector3.up * (_capsuleCollider.height / 2 - _capsuleCollider.radius);
            float maxDistance = (_capsuleCollider.height / 2 - _capsuleCollider.radius) + 0.1f;
            Gizmos.DrawWireSphere(origin + Vector3.down * maxDistance, radius);
        }
    }

    private void OnRespawn(InputAction.CallbackContext context)
    {
        // Respawn the character at the spawn position
        transform.position = _spawnPosition;
        _rigidbody.velocity = Vector3.zero;
    }
}