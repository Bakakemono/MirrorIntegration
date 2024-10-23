using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour {
    [SyncVar] public int _connectionID;
    [SyncVar] public int _playerIdNumber;

    PlayerControl _playerControl;
    InputAction _movementAction;

    [Header("Girl Body Params")]
    [SerializeField] private GameObject _girlModel;
    [SerializeField] private Vector3 _girlSize;

    [Header("Boy Body Params")]
    [SerializeField] private GameObject _boyModel;
    [SerializeField] private Vector3 _boySize;
    

    [Header("Parameters")]
    [SerializeField] float _speed = 1;
    [SerializeField] float _jumpVelocity = 6f;
    bool _doJump = false;
    [SerializeField] Transform _cameraPosition;
    Rigidbody _rigidbody;

    private void Start() {
        DontDestroyOnLoad(gameObject);
        _rigidbody = GetComponent<Rigidbody>();
        if(isLocalPlayer) {
            Transform cameraTransform = Camera.main.transform;
            cameraTransform.position = _cameraPosition.position;
            cameraTransform.rotation = _cameraPosition.rotation;
            cameraTransform.parent = transform;
        }
        _playerControl = new PlayerControl();
    }

    private void OnEnable() {
        _movementAction = _playerControl.Player.Movement;
        _movementAction.Enable();

        _playerControl.Player.Jump.performed += OnJump;
        _playerControl.Player.Jump.Enable();

        _playerControl.Player.Grab.Enable();
    }

    private void OnDisable() {
        _playerControl.Player.Movement.Disable();
        _playerControl.Player.Jump.Disable();
        _playerControl.Player.Grab.Disable();
    }

    private void Register() {
        if(isServer) {
            GameManager._instance.RegisterPlayer(this);
        }
    }

    private void Update() {
        if(!isLocalPlayer)
            return;

        Movement();
    }

    private void Movement() {
        Vector2 movement = _movementAction.ReadValue<Vector2>();

        Vector3 playerMovement = new Vector3(movement.x, 0, movement.y) * _speed;

        _rigidbody.velocity =
            new Vector3(
                Mathf.Lerp(_rigidbody.velocity.x, playerMovement.x, 0.3f),
                _rigidbody.velocity.y,
                Mathf.Lerp(_rigidbody.velocity.z, playerMovement.z, 0.3f));

        if(_doJump) {
            _rigidbody.velocity += Vector3.up * _jumpVelocity;
            _doJump = false;
        }
    }

    private void OnJump(InputAction.CallbackContext context) {
        _doJump = true;
    }

    private void OnGrab(InputAction.CallbackContext context) {
        
    }
}