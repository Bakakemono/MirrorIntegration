using Mirror;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.BC;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour {
    [SyncVar] public int _connectionID;
    [SyncVar] public int _playerIdNumber;

    PlayerControl _playerControl;
    InputAction _movementAction;
    InputAction _runAction;
    InputAction _respawnAction;
    InputAction _jumpAction;
    InputAction _grabAction;
    Vector2 _previousMovementInputValue = Vector2.zero;

    [Header("Connection Params")]
    [SerializeField] float _loadingTime = 0.2f;
    float _timeWhenLoadingStart = 0f;
    bool _playerBodySelected = false;
    bool _isReady = false;

    [Header("Girl Body Params")]
    [SerializeField] private GameObject _girlModel;
    [SerializeField] private Vector3 _girlSize;

    [Header("Boy Body Params")]
    [SerializeField] private GameObject _boyModel;
    [SerializeField] private Vector3 _boySize;

    [Header("Ground Movement Parameters")]
    [SerializeField] float _walkSpeed = 2f;
    [SerializeField] float _runSpeed = 5f;
    [SerializeField] float _accelerationTime = 0.2f;
    [SerializeField] float _decelerationTime = 0.2f;
    Vector2 _lastMoveDirection = Vector3.zero;

    [Header("Air Movement Parameters")]
    [SerializeField] float _airWalkSpeed = 2f;
    [SerializeField] float _airRunSpeed = 5f;
    [SerializeField] float _airAccelerationTime = 0.2f;
    [SerializeField] float _airDecelerationTime = 0.2f;

    float _currentSpeed = 0f;

    [Header("Jump Parameters")]
    [SerializeField, Tooltip("Maximum jump height")] float _jumpHeight = 2f;
    [SerializeField, Tooltip("Time to reach the apex of the jump")] float _timeToJumpApex = 0.5f;
    [SerializeField, Tooltip("Desired horizontal distance to cover during a jump")] float _desiredJumpLength = 5f;
    [SerializeField, Tooltip("Gravity multiplier when going up")] float _upwardMovementMultiplier = 1f;
    [SerializeField, Tooltip("Gravity multiplier when coming down")] float _downwardMovementMultiplier = 6.17f;
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

    [Header("Grab Params")]
    [SerializeField] Transform _grabBoxDetectionPostion;
    [SerializeField] Vector3 _grabBoxDetectionDimension;
    Vector3 _pushingDirection = Vector3.zero;
    PushableObject _currentPushedObject;
    float _pushingSpeed = 0f;
    float _pullingSpeed = 0f;

    [Header("Gravity")]
    [SerializeField] float _gravityScale = 1f;

    [SerializeField] Transform _cameraPosition;
    Rigidbody _rigidbody;
    CapsuleCollider _capsuleCollider;

    [SerializeField] bool _isBoy = false;
    [SerializeField] bool _isGirl = false;

    // LayerMask for ground detection
    [SerializeField] LayerMask _groundLayer;

    [SerializeField] LayerMask _pushableObjectLayer;

    private Vector3 _spawnPosition;

    private bool _isRunning = false; // Variable to track running state

    PushingBehavior _pushingBehavior;
    bool _isPushing = false;

    private void Start() {
        DontDestroyOnLoad(gameObject);
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        _pushingBehavior = GetComponent<PushingBehavior>();
        _pushingBehavior._onReleasing += OnGrabForceRelease;

        if(isLocalPlayer) {

            CameraBehavior._instance.SetPlayer(transform);

            _playerControl = new PlayerControl();
            _movementAction = _playerControl.Player.Movement;
            _movementAction.Enable();

            _jumpAction = _playerControl.Player.Jump;
            _jumpAction.IsInProgress();
            _jumpAction.Enable();

            _runAction = _playerControl.Player.Run;
            _runAction.performed += OnRunPerformed;
            _runAction.canceled += OnRunCanceled;
            _runAction.Enable();

            _grabAction = _playerControl.Player.Grab;
            _grabAction.performed += OnGrab;
            _grabAction.Enable();

            // Initialize respawn action
            _respawnAction = _playerControl.Player.Respawn;
            _respawnAction.performed += OnRespawn;
            _respawnAction.Enable();

            // Calculate gravity and jump velocity
            _gravity = (-2f * _jumpHeight) / (_timeToJumpApex * _timeToJumpApex);
            _jumpVelocity = Mathf.Abs(_gravity) * _timeToJumpApex;

            // Calculate total time in air
            float totalJumpTime = 2f * _timeToJumpApex;

            // Calculate required horizontal speed
            float requiredHorizontalSpeed = _desiredJumpLength / totalJumpTime;

            // Set air movement speeds
            _airWalkSpeed = requiredHorizontalSpeed;
            _airRunSpeed = requiredHorizontalSpeed;

            // Disable default gravity
            _rigidbody.useGravity = false;

            // Set initial spawn position
            _spawnPosition = transform.position;
        }
        _timeWhenLoadingStart += Time.time;
    }

    //private void OnEnable() {
    //    if(!isLocalPlayer)
    //        return;

    //    _movementAction = _playerControl.Player.Movement;
    //    _movementAction.Enable();

    //    _playerControl.Player.Jump.performed += OnJump;
    //    _playerControl.Player.Jump.Enable();

    //    _playerControl.Player.Grab.Enable();
    //}

    //private void OnDisable() {
    //    if(!isLocalPlayer)
    //        return;

    //    _playerControl.Player.Movement.Disable();
    //    _playerControl.Player.Jump.Disable();
    //    _playerControl.Player.Grab.Disable();
    //}

    private void Register() {
        if(isServer) {
            GameManager._instance.RegisterPlayer(this);
        }
    }

    private void Update() {
        if(!isLocalPlayer)
            return;

        if(!_playerBodySelected && _timeWhenLoadingStart + _loadingTime < Time.time) {
            CMD_ChooseModel(isServer);
        }

        if(_isPushing) {
            _pushingBehavior.WhilePushing(_movementAction.ReadValue<Vector2>());
            return;
        }

        GroundCheck();

        HandleInput();

        HandleTimers();

        Movement();
    }

    private void FixedUpdate() {
        ApplyGravity();
    }

    private void HandleInput() {
        // Read jump input
        _pressingJump = _jumpAction.IsPressed();

        // Check if jump was pressed this frame
        if(_jumpAction.WasPressedThisFrame()) {
            _jumpBufferCounter = _jumpBufferDuration;
        }

        // Read run input
        _isRunning = _runAction.IsPressed();
    }

    private void HandleTimers() {
        // Handle coyote time counter
        if(_isGrounded) {
            _coyoteTimeCounter = _coyoteTimeDuration;
            _airJumpsPerformed = 0; // Reset air jumps
        }
        else {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        // Handle jump buffer counter
        if(_jumpBufferCounter > 0f) {
            _jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void Movement() {
        Vector2 movementInput = _movementAction.ReadValue<Vector2>();

        _lastMoveDirection = movementInput != Vector2.zero ? movementInput.normalized : _lastMoveDirection;

        transform.rotation =
            Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(new Vector3(_lastMoveDirection.x, 0f, _lastMoveDirection.y), Vector3.up),
                Time.deltaTime * 10f
                );

        // Determine target speed and acceleration based on grounded state
        float targetSpeed;
        float accelerationTime;
        float decelerationTime;
        float maxAllowedSpeed;

        if(_isGrounded) {
            // Grounded movement parameters
            targetSpeed = (_isRunning ? _runSpeed : _walkSpeed) * movementInput.magnitude;
            accelerationTime = _accelerationTime;
            decelerationTime = _decelerationTime;
            maxAllowedSpeed = (_isRunning ? _runSpeed : _walkSpeed);
        }
        else {
            // Airborne movement parameters
            targetSpeed = (_isRunning ? _airRunSpeed : _airWalkSpeed) * movementInput.magnitude;
            accelerationTime = _airAccelerationTime;
            decelerationTime = _airDecelerationTime;
            maxAllowedSpeed = float.MaxValue; // No speed limit in the air
        }

        if(movementInput.magnitude > 0f) {
            // Accelerate towards target speed
            float accelerationRate = targetSpeed / accelerationTime;
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, accelerationRate * Time.deltaTime);
        }
        else {
            // Decelerate to zero
            float decelerationRate = _currentSpeed / decelerationTime;
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, decelerationRate * Time.deltaTime);
        }

        // Clamp _currentSpeed to maxAllowedSpeed when grounded
        if(_isGrounded) {
            _currentSpeed = Mathf.Min(_currentSpeed, maxAllowedSpeed);
        }

        movementInput = movementInput.normalized * _currentSpeed;

        // Apply horizontal movement
        _rigidbody.velocity = new Vector3(
            movementInput.x,
            _rigidbody.velocity.y,
            movementInput.y
        );

        // Handle jumping
        bool canJump = (_isGrounded || (_enableCoyoteTime && _coyoteTimeCounter > 0f) || (_maxAirJumps > 0 && _airJumpsPerformed < _maxAirJumps));

        if(_jumpBufferCounter > 0f && canJump) {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
            _rigidbody.velocity += Vector3.up * _jumpVelocity;
            _jumpBufferCounter = 0f;
            _coyoteTimeCounter = 0f;
            _isJumping = true;

            if(!_isGrounded && !(_enableCoyoteTime && _coyoteTimeCounter > 0f)) {
                _airJumpsPerformed++;
            }
        }
    }

    private void ApplyGravity() {
        float gravityMultiplier = 1f;
        float verticalVelocity = _rigidbody.velocity.y;

        if(verticalVelocity > 0.01f) {
            // Ascending
            if(_pressingJump && _isJumping) {
                gravityMultiplier = 1f; // Normal gravity during ascent when holding jump
            }
            else {
                gravityMultiplier = _jumpCutOff; // Apply extra gravity when jump is released
            }
        }
        else if(verticalVelocity < -0.01f) {
            // Descending
            gravityMultiplier = _downwardMovementMultiplier; // Apply increased gravity when descending
        }
        else {
            gravityMultiplier = 1f;
        }

        // Apply custom gravity
        Vector3 gravityForce = new Vector3(0f, _gravity * gravityMultiplier, 0f);
        _rigidbody.AddForce(gravityForce, ForceMode.Acceleration);

        // Limit fall speed
        if(_rigidbody.velocity.y < -_fallSpeedLimit) {
            Vector3 vel = _rigidbody.velocity;
            vel.y = -_fallSpeedLimit;
            _rigidbody.velocity = vel;
        }
    }

    private void GroundCheck() {
        float extraHeight = 0.1f;
        float radius = _capsuleCollider.radius * 0.9f; // Slightly reduce radius to avoid clipping issues
        Vector3 origin = transform.position + Vector3.up * (_capsuleCollider.height / 2 - _capsuleCollider.radius);
        float maxDistance = (_capsuleCollider.height / 2 - _capsuleCollider.radius) + extraHeight;

        // Use SphereCast to detect ground
        RaycastHit hit;
        if(Physics.SphereCast(origin, radius, Vector3.down, out hit, maxDistance, _groundLayer)) {
            _isGrounded = true;

            if(!_wasGroundedLastFrame) {
                // We just landed
                _isJumping = false;

                // Reset _currentSpeed to ground target speed
                Vector2 movementInput = _movementAction.ReadValue<Vector2>();
                float movementMagnitude = movementInput.magnitude;
                float targetGroundSpeed = (_isRunning ? _runSpeed : _walkSpeed) * movementMagnitude;

                _currentSpeed = targetGroundSpeed;
            }
        }
        else {
            _isGrounded = false;
        }

        _wasGroundedLastFrame = _isGrounded;
    }

    void GrabCheck() {
        Vector2 movementInput = _movementAction.ReadValue<Vector2>();

        _rigidbody.velocity = movementInput.normalized;
    }

    private void OnRunPerformed(InputAction.CallbackContext context) {
        _isRunning = true;
    }

    private void OnRunCanceled(InputAction.CallbackContext context) {
        _isRunning = false;
    }

    private void OnDrawGizmosSelected() {
        // Visualize the ground check sphere
        if(_capsuleCollider != null) {
            Gizmos.color = Color.red;
            float radius = _capsuleCollider.radius * 0.9f;
            Vector3 origin = transform.position + Vector3.up * (_capsuleCollider.height / 2 - _capsuleCollider.radius);
            float maxDistance = (_capsuleCollider.height / 2 - _capsuleCollider.radius) + 0.1f;
            Gizmos.DrawWireSphere(origin + Vector3.down * maxDistance, radius);
        }
        if(_grabBoxDetectionPostion != null) {
            Gizmos.matrix = _grabBoxDetectionPostion.localToWorldMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, _grabBoxDetectionDimension);
        }
    }

    private void OnRespawn(InputAction.CallbackContext context) {
        // Respawn the character at the spawn position
        transform.position = _spawnPosition;
        _rigidbody.velocity = Vector3.zero;
    }

    void Climbing() {

    }

    private void OnGrab(InputAction.CallbackContext context) {
        _isPushing =_pushingBehavior.Grab();
        if(!isServer)
            CMD_Grab();
    }

    void OnGrabForceRelease() {
        _isPushing = false;
    }

    [Command]
    void CMD_ChooseModel(bool isGirl) {
        RPC_SwitchModel(isGirl);
    }

    [ClientRpc]
    private void RPC_SwitchModel(bool isGirl) {
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        if(isGirl) {
            _girlModel.SetActive(true);
            capsuleCollider.radius = _girlSize.x / 2f;
            capsuleCollider.height = _girlSize.y;
        }
        else {
            _boyModel.SetActive(true);
            capsuleCollider.radius = _boySize.x / 2f;
            capsuleCollider.height = _boySize.y;
        }
        _playerBodySelected = true;
    }

    [Command]
    void CMD_Grab() {
        _pushingBehavior.Grab();
    }
}