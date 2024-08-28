using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PlayerController : NetworkBehaviour {

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
    }
    private void Update() {
        if(!isLocalPlayer)
            return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 playerMovement = new Vector3(h, 0, v) * 0.5f;

        transform.position += playerMovement;
    }
}
