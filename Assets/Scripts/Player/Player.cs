using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private MovementConfig movementConfigAsset;
    [SerializeField] private JumpConfig jumpConfigAsset;
    [SerializeField] private CharacterConfig characterConfig;
    [SerializeField] private RopeConfig ropeConfigAsset;

    [Header("Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask beamLayer;
    [SerializeField] private LayerMask ropeLayer;
    [SerializeField] private Transform cameraPosition;

    [Header("Runtime Configuration")]
    [SerializeField] private RuntimeMovementConfig RuntimeMovementConfig;
    [SerializeField] private RuntimeJumpConfig RuntimejumpConfig;
    [SerializeField] private RuntimeRopeConfig RuntimeRopeConfig;

    private PlayerInputHandler _input;
    private PlayerMovement _movement;
    private PlayerJump _jump;
    private RopeSystem _rope;
    private GroundDetector _groundDetector;
    private CharacterModel _characterModel;
    private Vector3 _spawnPosition;
    private Transform _nearestRope;

    private void Awake()
    {
        ValidateComponents();
        InitializeComponents();
    }

    private void ValidateComponents()
    {
        if (movementConfigAsset == null) throw new System.Exception("MovementConfig is not assigned!");
        if (jumpConfigAsset == null) throw new System.Exception("JumpConfig is not assigned!");
        if (characterConfig == null) throw new System.Exception("CharacterConfig is not assigned!");
        if (cameraPosition == null) throw new System.Exception("Camera Position is not assigned!");
    }

    private void InitializeComponents()
    {
        RuntimeMovementConfig = new RuntimeMovementConfig(movementConfigAsset);
        RuntimejumpConfig = new RuntimeJumpConfig(jumpConfigAsset);
        RuntimeRopeConfig = new RuntimeRopeConfig(ropeConfigAsset);

        var rb = GetComponent<Rigidbody>();
        var capsuleCollider = GetComponent<CapsuleCollider>();

        _input = new PlayerInputHandler();
        _movement = new PlayerMovement(rb, RuntimeMovementConfig,RuntimejumpConfig);
<<<<<<< Updated upstream
        _jump = new PlayerJump(rb, RuntimejumpConfig, _movement);
        _groundDetector = new GroundDetector(capsuleCollider, groundLayer,beamLayer);
=======
        _jump = new PlayerJump(rb, RuntimejumpConfig, _movement, RuntimeMovementConfig);
        _movement.SetJumpReference(_jump);
        _groundDetector = new GroundDetector(capsuleCollider, groundLayer);
>>>>>>> Stashed changes
        _characterModel = new CharacterModel(capsuleCollider, characterConfig,transform);

        _spawnPosition = transform.position;
        SetupCamera();
    }

    private void Start()
    {
        // Disable default gravity as we're using custom gravity
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = false;
        }
    }

    // Bouton dans l'inspecteur pour mettre à jour les paramètres
    [ContextMenu("Update Jump Parameters")]
    public void UpdateJumpConfig()
    {
        if (_jump != null)
        {
            _jump.UpdateJumpParameters();
        }
    }

    private void Update()
    {
        if (!_input.IsEnabled)
            return;

        var inputData = _input.GetInput();
        bool isGrounded = _groundDetector.CheckGround();
        bool isOnBeam = (_groundDetector as GroundDetector).IsOnBeam;

<<<<<<< Updated upstream
        _movement.UpdateMovement(inputData, isGrounded,isOnBeam);
        _jump.UpdateJump(inputData, isGrounded, _movement.LastInputDirection, _movement.WasAtRunSpeed, isOnBeam);
=======
        // First handle ground movement
        if (isGrounded)
        {
            _movement.UpdateMovement(inputData, isGrounded);
        }

        // Then handle jump - this will preserve jump velocity
        _jump.UpdateJump(inputData, isGrounded, _movement.LastInputDirection, _movement.WasAtRunSpeed);

        // After jump, only update movement if we're not jumping
        if (!isGrounded && !_jump.IsJumping)
        {
            _movement.UpdateMovement(inputData, isGrounded);
        }
>>>>>>> Stashed changes

        if (inputData.IsRespawnPressed)
        {
            Respawn();
        }
    }
    private void SetupCamera()
    {
        if (Camera.main != null && cameraPosition != null)
        {
            Transform cameraTransform = Camera.main.transform;
            cameraTransform.position = cameraPosition.position;
            cameraTransform.rotation = cameraPosition.rotation;
            cameraTransform.parent = transform;
        }
        else
        {
            Debug.LogWarning("Main camera not found or camera position not set!");
        }
    }

    private void Respawn()
    {
        _movement.Reset();
        _jump.Reset();
        transform.position = _spawnPosition;

        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = Vector3.zero;
        }

        _characterModel.Reset();
    }

    private void OnEnable()
    {
        _input?.Enable();
    }

    private void OnDisable()
    {
        _input?.Disable();
    }

    private void OnDestroy()
    {
        _input?.Disable();
    }

    private void OnValidate()

    {
        if (Application.isPlaying && _jump != null)
        {
            _jump.UpdateJumpParameters();
        }

        if (movementConfigAsset == null)
            Debug.LogWarning("MovementConfig is not assigned on " + gameObject.name);
        if (jumpConfigAsset == null)
            Debug.LogWarning("JumpConfig is not assigned on " + gameObject.name);
        if (characterConfig == null)
            Debug.LogWarning("CharacterConfig is not assigned on " + gameObject.name);
        if (cameraPosition == null)
            Debug.LogWarning("Camera Position is not assigned on " + gameObject.name);
    }

    private void OnDrawGizmos()
    {
        if (_groundDetector != null)
        {
            (_groundDetector as GroundDetector)?.DrawDebugGizmos();
        }
    }
}