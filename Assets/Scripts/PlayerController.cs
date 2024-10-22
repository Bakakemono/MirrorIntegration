using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour {
    [SyncVar] public int _connectionID;
    [SyncVar] public int _playerIdNumber;

    [Header("Girl Body Params")]
    [SerializeField] private GameObject _girlModel;
    [SerializeField] private Vector3 _girlSize;

    [Header("Boy Body Params")]
    [SerializeField] private GameObject _boyModel;
    [SerializeField] private Vector3 _boySize;

    [SerializeField] GameObject _model;
    Rigidbody _rigidbody;

    [Header("Parameters")]
    [SerializeField] float _speed = 1;
    [SerializeField] float _jumpVelocity = 6f;
    [SerializeField] Transform _cameraPosition;

    private void Start() {
        DontDestroyOnLoad(gameObject);
        _rigidbody = GetComponent<Rigidbody>();
        if(isLocalPlayer) {
            Transform cameraTransform = Camera.main.transform;
            cameraTransform.position = _cameraPosition.position;
            cameraTransform.rotation = _cameraPosition.rotation;
            cameraTransform.parent = transform;
        }
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
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 playerMovement = new Vector3(h, 0, v) * _speed;

        _rigidbody.velocity =
            new Vector3(
                Mathf.Lerp(_rigidbody.velocity.x, playerMovement.x, 0.3f),
                _rigidbody.velocity.y,
                Mathf.Lerp(_rigidbody.velocity.z, playerMovement.z, 0.3f));

        if(Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("jUMP");
            _rigidbody.velocity += Vector3.up * _jumpVelocity;
        }
    }
}