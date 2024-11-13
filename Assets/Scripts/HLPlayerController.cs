using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HLPlayerController : MonoBehaviour{
    PlayerControl _playerControl;
    InputAction _movementAction;
    Vector2 _previousMovementInputValue = Vector2.zero;

    [Header("Girl Body Params")]
    [SerializeField] private GameObject _girlModel;
    [SerializeField] private Vector3 _girlSize;

    [Header("Boy Body Params")]
    [SerializeField] private GameObject _boyModel;
    [SerializeField] private Vector3 _boySize;


    [Header("Parameters")]
    [SerializeField] float _speed = 1;
    [SerializeField] float _jumpHeight = 1f;
    bool _doJump = false;
    [SerializeField] Transform _cameraPosition;
    Rigidbody _rigidbody;
    CapsuleCollider _capsuleCollider;

    [SerializeField] bool _isBoy = false;
    [SerializeField] bool _isGirl = false;


    private void Start() {
        DontDestroyOnLoad(gameObject);
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        Transform cameraTransform = Camera.main.transform;
        cameraTransform.position = _cameraPosition.position;
        cameraTransform.rotation = _cameraPosition.rotation;
        cameraTransform.parent = transform;

        _playerControl = new PlayerControl();
        _movementAction = _playerControl.Player.Movement;
        _movementAction.Enable();

        _playerControl.Player.Jump.performed += OnJump;
        _playerControl.Player.Jump.Enable();

        _playerControl.Player.Grab.Enable();
    }

    //private void OnEnable() {
    //    _movementAction = _playerControl.Player.Movement;
    //    _movementAction.Enable();

    //    _playerControl.Player.Jump.performed += OnJump;
    //    _playerControl.Player.Jump.Enable();

    //    _playerControl.Player.Grab.Enable();
    //}

    //private void OnDisable() {
    //    _playerControl.Player.Movement.Disable();
    //    _playerControl.Player.Jump.Disable();
    //    _playerControl.Player.Grab.Disable();
    //}

    private void Update() {

        Movement();

        if(_isGirl) {
            _girlModel.SetActive(true);
            _boyModel.SetActive(false);
            _capsuleCollider.radius = _girlSize.x / 2f;
            _capsuleCollider.height = _girlSize.y;

            _isGirl = false;
        }
        else if(_isBoy) {
            _boyModel.SetActive(true);
            _girlModel.SetActive(false);
            _capsuleCollider.radius = _boySize.x / 2f;
            _capsuleCollider.height = _boySize.y;

            _isBoy = false;
        }
    }

    private void Movement() {
        Vector2 movement = _movementAction.ReadValue<Vector2>();
        if(movement != Vector2.zero)
            movement = movement / movement.magnitude * Mathf.Clamp(movement.magnitude, 0f, 1f);

        movement = Vector2.Lerp(_previousMovementInputValue, movement, 0.08f);
        _previousMovementInputValue = movement;

        Vector3 playerMovement = new Vector3(movement.x, 0, movement.y) * _speed;

        _rigidbody.velocity =
            new Vector3(
                Mathf.Lerp(_rigidbody.velocity.x, playerMovement.x, 0.3f),
                _rigidbody.velocity.y,
                Mathf.Lerp(_rigidbody.velocity.z, playerMovement.z, 0.3f));

        if(_doJump) {
            _rigidbody.velocity += Vector3.up * Mathf.Sqrt(2 * -Physics.gravity.y * (_jumpHeight));
            _doJump = false;
        }
    }

    private void OnJump(InputAction.CallbackContext context) {
        _doJump = true;
    }

    private void OnGrab(InputAction.CallbackContext context) {

    }
}
