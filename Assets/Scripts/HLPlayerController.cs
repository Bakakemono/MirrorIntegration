using Org.BouncyCastle.Asn1.Esf;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class HLPlayerController : MonoBehaviour
{
    PlayerControl _playerControl;
    InputAction _movementAction;
    InputAction _runAction;
    InputAction _respawnAction;
    InputAction _jumpAction;
    InputAction _interactAction;
    InputAction _grabAction;

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
    private const float RUN_SPEED_THRESHOLD = 0.90f; // 90% de la vitesse de course

    [Header("Air Movement Parameters")]
    [SerializeField] float _airWalkSpeed = 2f;
    [SerializeField] float _airRunSpeed = 5f;
    [SerializeField] float _airAccelerationTime = 0.2f;
    [SerializeField] float _airDecelerationTime = 0.2f;

    float _currentSpeed = 0f;

    [Header("Jump Parameters")]
    [SerializeField, Tooltip("Maximum jump height")] float _jumpHeight = 2f;
    [SerializeField, Tooltip("Time to reach the apex of the jump")] float _timeToJumpApex = 0.5f;
    [SerializeField, Tooltip("Gravity multiplier when going up")] float _upwardMovementMultiplier = 1f;
    [SerializeField, Tooltip("Gravity multiplier when coming down")] float _downwardMovementMultiplier = 4f;
    [SerializeField, Tooltip("Gravity multiplier when jump button is released early")] float _jumpCutOff = 2f;
    [SerializeField, Tooltip("Maximum fall speed")] float _fallSpeedLimit = 20f;
    [SerializeField, Tooltip("Maximum number of air jumps")] int _maxAirJumps = 0;
    [SerializeField] PushConfig _pushConfig;

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
    Vector2 _lastMoveDirection = Vector2.right;

    float _gravity;
    float _jumpVelocity;

    [Header("Surface Detection")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _beamLayer;
    [SerializeField] private LayerMask _beamLayerZ;
    private bool _isOnBeam;
    private bool _isOnBeamZ;  // Add this

    [SerializeField] Transform _cameraPosition;
    Rigidbody _rigidbody;
    CapsuleCollider _capsuleCollider;

    [SerializeField] bool _isBoy = false;
    [SerializeField] bool _isGirl = false;

    private Vector3 _spawnPosition;
    private Vector3 _lastInputDirection = Vector3.zero;
    private float _previousMovementInputMagnitude = 0f;
    private bool _isRunning = false;

    PlayerPush _playerPush;

    private void Start()
    {
        InitializeComponents();
        InitializeInputs();
        InitializeMovementParameters();
        SetupCamera();
    }

    private void InitializeComponents()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _playerPush = new PlayerPush(new RuntimePushConfig(_pushConfig), transform, _rigidbody);
        _rigidbody.useGravity = false;
        _spawnPosition = transform.position;
    }

    private void InitializeInputs()
    {
        _playerControl = new PlayerControl();

        _movementAction = _playerControl.Player.Movement;
        _jumpAction = _playerControl.Player.Jump;
        _runAction = _playerControl.Player.Run;
        _respawnAction = _playerControl.Player.Respawn;
        _interactAction = _playerControl.Player.Interact;

        _movementAction.Enable();
        _jumpAction.Enable();
        _runAction.Enable();
        _respawnAction.Enable();
        _interactAction.Enable();

        _grabAction = _playerControl.Player.Grab;
        _grabAction.performed += OnGrab;
        _grabAction.Enable();

        _respawnAction.performed += OnRespawn;
    }

    private void InitializeMovementParameters()
    {
        CalculateJumpParameters();
    }

    private void SetupCamera()
    {
        if (Camera.main != null && _cameraPosition != null)
        {
            Transform cameraTransform = Camera.main.transform;
            cameraTransform.position = _cameraPosition.position;
            cameraTransform.rotation = _cameraPosition.rotation;
            cameraTransform.parent = transform;
        }
    }

    private void Update()
    {

        if (_playerPush._isPushing)
        {
            _playerPush.WhilePushing(_movementAction.ReadValue<Vector2>());
            return;
        }

        GroundCheck();
        HandleInput();
        HandleTimers();
        Movement();
        HandleModelSwitching();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    private void HandleInput()
    {
        _pressingJump = _jumpAction.IsPressed();

        if (_jumpAction.WasPressedThisFrame())
        {
            _jumpBufferCounter = _jumpBufferDuration;
        }

        _isRunning = _runAction.IsPressed();
    }

    private void HandleTimers()
    {
        if (_isGrounded || _isOnBeam)
        {
            _coyoteTimeCounter = _coyoteTimeDuration;
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

    private void Movement()
    {
        Vector2 movementInput = _movementAction.ReadValue<Vector2>();
        _lastMoveDirection = movementInput != Vector2.zero ? movementInput.normalized : _lastMoveDirection;

        transform.rotation =
            Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(new Vector3(_lastMoveDirection.x, 0f, _lastMoveDirection.y), Vector3.up),
                Time.deltaTime * 10f
                );
        Vector3 inputDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        float movementInputMagnitude = movementInput.magnitude;

        // Handle beam movement
        if (_isOnBeam)
        {
            HandleBeamMovement(inputDirection);
            return;
        }
        if (_isOnBeamZ)
        {
            HandleBeamMovementZ(inputDirection);
            return;
        }

        HandleNormalMovement(inputDirection, movementInputMagnitude);
        HandleJump();

        _previousMovementInputMagnitude = movementInputMagnitude;
    }

    private void HandleBeamMovement(Vector3 inputDirection)
    {
        // Only use X component and reduce speed
        Vector3 beamMovement = new Vector3(inputDirection.x, 0, 0);
        _currentSpeed = _walkSpeed * 0.6f;
        _rigidbody.velocity = new Vector3(beamMovement.x * _currentSpeed, _rigidbody.velocity.y, 0);
    }

    private void HandleBeamMovementZ(Vector3 inputDirection)
    {
        // Only use Z component, ignore X and Y inputs completely
        _currentSpeed = _walkSpeed * 0.6f;
        _rigidbody.velocity = new Vector3(
            0, // Force X to 0
            _rigidbody.velocity.y,
            inputDirection.z * _currentSpeed // Only use Z movement
        );
    }

    private void HandleNormalMovement(Vector3 inputDirection, float movementInputMagnitude)
    {
        if (movementInputMagnitude > 0f)
        {
            _lastInputDirection = inputDirection;

            float targetSpeed = DetermineTargetSpeed();
            float accelerationTime = _isGrounded ? _accelerationTime : _airAccelerationTime;

            _currentSpeed = CalculateNewSpeed(targetSpeed, accelerationTime);
        }
        else
        {
            DecelerateToStop();
        }

        Vector3 movement = _lastInputDirection * _currentSpeed;
        _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
    }

    private float DetermineTargetSpeed()
    {
        if (_isGrounded)
        {
            return _isRunning ? _runSpeed : _walkSpeed;
        }
        return _isRunning ? _airRunSpeed : _airWalkSpeed;
    }

    private float CalculateNewSpeed(float targetSpeed, float accelerationTime)
    {
        float speedDifference = targetSpeed - _currentSpeed;
        float rate = Mathf.Abs(speedDifference) / accelerationTime;
        return Mathf.MoveTowards(_currentSpeed, targetSpeed, rate * Time.deltaTime);
    }

    private void DecelerateToStop()
    {
        float decelerationTime = _isGrounded ? _decelerationTime : _airDecelerationTime;
        float decelerationRate = _currentSpeed / decelerationTime;
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, decelerationRate * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (_isOnBeam) return; // No jumping on beams

        bool canJump = (_isGrounded ||
                      (_enableCoyoteTime && _coyoteTimeCounter > 0f) ||
                      (_maxAirJumps > 0 && _airJumpsPerformed < _maxAirJumps));

        if (_jumpBufferCounter > 0f && canJump)
        {
            ExecuteJump();
        }
    }

    private void ExecuteJump()
    {
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

    private void ApplyGravity()
    {
        float gravityMultiplier = DetermineGravityMultiplier();
        Vector3 gravityForce = new Vector3(0f, _gravity * gravityMultiplier, 0f);
        _rigidbody.AddForce(gravityForce, ForceMode.Acceleration);
        LimitFallSpeed();
    }

    private float DetermineGravityMultiplier()
    {
        float verticalVelocity = _rigidbody.velocity.y;

        if (verticalVelocity > 0.01f)
        {
            return _pressingJump && _isJumping ? _upwardMovementMultiplier : _jumpCutOff;
        }
        else if (verticalVelocity < -0.01f)
        {
            return _downwardMovementMultiplier;
        }

        return 1f;
    }

    private void LimitFallSpeed()
    {
        if (_rigidbody.velocity.y < -_fallSpeedLimit)
        {
            Vector3 velocity = _rigidbody.velocity;
            velocity.y = -_fallSpeedLimit;
            _rigidbody.velocity = velocity;
        }
    }

    private void GroundCheck()
    {
        float extraHeight = 0.1f;
        float radius = _capsuleCollider.radius * 0.9f;
        Vector3 origin = transform.position;
        float maxDistance = (_capsuleCollider.height / 2 - _capsuleCollider.radius) + extraHeight;

        // Ground check
        _isGrounded = Physics.OverlapSphere(origin, radius, _groundLayer).Length > 0;

        // Beam check
        _isOnBeam = Physics.OverlapSphere(origin, radius, _beamLayer).Length > 0;
        _isOnBeamZ = Physics.OverlapSphere(origin, radius, _beamLayerZ).Length > 0;

        bool isOnSurface = _isGrounded || _isOnBeam;
        if (isOnSurface && !_wasGroundedLastFrame)
        {
            OnLanding();
        }

        _wasGroundedLastFrame = isOnSurface;
    }

    private void OnLanding()
    {
        _isJumping = false;
        Vector2 movementInput = _movementAction.ReadValue<Vector2>();
        float movementMagnitude = movementInput.magnitude;
        float targetSpeed = (_isRunning ? _runSpeed : _walkSpeed) * movementMagnitude;
        _currentSpeed = targetSpeed;
    }

    private void CalculateJumpParameters()
    {
        _gravity = (-2f * _jumpHeight) / (_timeToJumpApex * _timeToJumpApex);
        _jumpVelocity = Mathf.Abs(_gravity) * _timeToJumpApex;
    }

    private void HandleModelSwitching()
    {
        if (_isGirl)
        {
            _GirlModel.SetActive(true);
            _BoyModel.SetActive(false);
            _capsuleCollider.radius = _girlSize.x / 2f;
            _capsuleCollider.height = _girlSize.y;
            _capsuleCollider.center = new Vector3(0, _girlSize.y / 2f, 0);
            _isGirl = false;
        }
        else if (_isBoy)
        {
            _BoyModel.SetActive(true);
            _GirlModel.SetActive(false);
            _capsuleCollider.radius = _BoySize.x / 2f;
            _capsuleCollider.height = _BoySize.y;
            _capsuleCollider.center = new Vector3(0, _BoySize.y / 2f, 0);
            _isBoy = false;
        }
    }

    private void OnRespawn(InputAction.CallbackContext context)
    {
        transform.position = _spawnPosition;
        _rigidbody.velocity = Vector3.zero;
        _currentSpeed = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (_capsuleCollider != null)
        {
            Gizmos.color = Color.red;
            float radius = _capsuleCollider.radius * 0.9f;
            Vector3 origin = transform.position + Vector3.up * (_capsuleCollider.height / 2 - _capsuleCollider.radius);
            float maxDistance = (_capsuleCollider.height / 2 - _capsuleCollider.radius) + 0.1f;
            Gizmos.DrawWireSphere(origin + Vector3.down * maxDistance, radius);
        }
        _playerPush.DrawGizmos();
    }

    private void OnGrab(InputAction.CallbackContext context)
    {
        _playerPush.Grab();
    }

    private void OnDestroy()
    {
        _respawnAction.performed -= OnRespawn;
        _playerControl?.Dispose();
    }
}