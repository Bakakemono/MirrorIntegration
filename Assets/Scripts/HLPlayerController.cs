using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class HLPlayerController : MonoBehaviour
{
    private PlayerControl _playerControl;
    private InputAction _movementAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _togglePushPullAction;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float fallMultiplier = 2.5f;

    [Header("Push/Pull Settings")]
    [SerializeField] private LayerMask pushableLayer;

    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;
    private bool _isGrounded = false;
    private bool _isRunning = false;
    private bool _isPushPulling = false;
    private PushableObject _currentPushableObject;

    public bool IsGrounded => _isGrounded;
    public bool IsPushPulling => _isPushPulling;

    private void Awake()
    {
        _playerControl = new PlayerControl();
        _movementAction = _playerControl.Player.Movement;
        _jumpAction = _playerControl.Player.Jump;
        _runAction = _playerControl.Player.Run;
        _togglePushPullAction = _playerControl.Player.Grab;

        _movementAction.Enable();
        _jumpAction.Enable();
        _runAction.Enable();
        _togglePushPullAction.Enable();

        _jumpAction.performed += OnJump;
        _runAction.performed += ctx => _isRunning = ctx.ReadValue<float>() > 0.5f;
        _runAction.canceled += ctx => _isRunning = false;
        _togglePushPullAction.performed += ctx => TogglePushPullMode();
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        if (_rigidbody == null || _capsuleCollider == null)
        {
            Debug.LogError("Player requires both a Rigidbody and a CapsuleCollider.");
        }
    }

    private void Update()
    {
        CheckGrounded();
        HandleMovement();
        ApplyGravity();
    }

    private void CheckGrounded()
    {
        if (_capsuleCollider == null) return;

        // Define the radius of the sphere based on the capsule collider's radius
        float sphereRadius = _capsuleCollider.radius * 0.9f; // Slightly smaller than the collider radius
        Vector3 sphereCenter = transform.position + Vector3.down * (_capsuleCollider.bounds.extents.y - sphereRadius);

        // Perform a sphere cast just below the player to check for ground contact
        _isGrounded = Physics.CheckSphere(sphereCenter, sphereRadius, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);

        Debug.Log("IsGrounded: " + _isGrounded);
    }

    private void HandleMovement()
    {
        Vector2 movementInput = _movementAction.ReadValue<Vector2>();
        float speed = (_isRunning ? runSpeed : walkSpeed) * movementInput.magnitude;

        Vector3 movementDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        Vector3 targetHorizontalVelocity = Vector3.Lerp(new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z),
                                                        movementDirection * speed,
                                                        (_isGrounded ? acceleration : deceleration) * Time.deltaTime);

        _rigidbody.velocity = new Vector3(targetHorizontalVelocity.x, _rigidbody.velocity.y, targetHorizontalVelocity.z);

        if (_isPushPulling && _currentPushableObject != null)
        {
            _currentPushableObject.StartPushPull(movementDirection);
        }
    }

    private void ApplyGravity()
    {
        if (_rigidbody.velocity.y < 0)
        {
            _rigidbody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else
        {
            _rigidbody.velocity += Vector3.up * Physics.gravity.y * (gravityScale - 1) * Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight), _rigidbody.velocity.z);
            Debug.Log("Player Jumped");
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    private void TogglePushPullMode()
    {
        if (_currentPushableObject != null)
        {
            _isPushPulling = !_isPushPulling;
            if (_isPushPulling)
            {
                Debug.Log("Started pushing/pulling.");
            }
            else
            {
                _currentPushableObject.StopPushPull();
                Debug.Log("Stopped pushing/pulling.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & pushableLayer) != 0)
        {
            _currentPushableObject = other.GetComponent<PushableObject>();
            Debug.Log("Entered pushable object zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_currentPushableObject != null && other.GetComponent<PushableObject>() == _currentPushableObject)
        {
            if (_isPushPulling)
            {
                _currentPushableObject.StopPushPull();
            }
            _currentPushableObject = null;
            _isPushPulling = false;
            Debug.Log("Exited pushable object zone.");
        }
    }
}