using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraBehavior : MonoBehaviour {
    [SerializeField] Vector3 _cameraPos;
    Transform _playerTransform;

    public static CameraBehavior _instance;

    void Start () {
        DontDestroyOnLoad(gameObject);
        if(_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }
    private void FixedUpdate() {
        if(_playerTransform == null)
            return;

        transform.position = _playerTransform.position + _cameraPos;
        transform.LookAt(_playerTransform.position);
    }

    public void SetPlayer(Transform playerTransform) {
        _playerTransform = playerTransform;
    }
}
