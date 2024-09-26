using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour {

    [SerializeField] GameObject playerModel;
    [SerializeField] Transform _cameraPosTransform;

    public override void OnStartLocalPlayer() {
        Transform cameraTransform = Camera.main.transform;
        cameraTransform.parent = _cameraPosTransform;
        cameraTransform.position = _cameraPosTransform.position;
        cameraTransform.rotation = _cameraPosTransform.rotation;
    }


    private void Start() {
        DontDestroyOnLoad(this.gameObject);
        //playerModel.SetActive(false);
    }

    private void Update() {
        //if(SceneManager.GetActiveScene().name == "Game") {
        //    if(!playerModel.activeSelf) {
        //        playerModel.SetActive(true);
        //    }

        //    Movement();
        //}

        if(!isLocalPlayer)
            return;

        Movement();
    }

    private void Movement() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 playerMovement = new Vector3(h, 0, v) * 0.5f;

        transform.position += playerMovement;
    }
}
