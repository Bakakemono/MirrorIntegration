using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour {

    [SerializeField] GameObject playerModel;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
    }

    private void Start() {
        playerModel.SetActive(false);
    }

    private void Update() {
        if(SceneManager.GetActiveScene().name == "Game") {
            if(!playerModel.activeSelf) {
                playerModel.SetActive(true);
            }

            Movement();
        }
    }

    private void Movement() {
        if(!isLocalPlayer)
            return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 playerMovement = new Vector3(h, 0, v) * 0.5f;

        transform.position += playerMovement;

    }
}
