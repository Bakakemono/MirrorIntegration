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

        
    }
}
