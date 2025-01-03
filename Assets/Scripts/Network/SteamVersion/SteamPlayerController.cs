using JetBrains.Annotations;
using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamPlayerController : NetworkBehaviour {

    //Player Data
    [SyncVar] public int _connectionID;
    [SyncVar] public int _playerIdNumber;
    [SyncVar] public ulong _playerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string _playerName;   

    private CustomSteamNetworkManager _NetworkManager;

    private CustomSteamNetworkManager _networkManager {
        get {
            if(_NetworkManager != null) {
                return _NetworkManager;
            }
            return _NetworkManager = CustomSteamNetworkManager.singleton as CustomSteamNetworkManager;
        }
    }

    [SerializeField] GameObject playerModel;
    [SerializeField] Transform _cameraPosTransform;

    public override void OnStartLocalPlayer() {
        Transform cameraTransform = Camera.main.transform;
        cameraTransform.parent = _cameraPosTransform;
        cameraTransform.position = _cameraPosTransform.position;
        cameraTransform.rotation = _cameraPosTransform.rotation;
    }


    private void Start() {
        DontDestroyOnLoad(gameObject);
        //playerModel.SetActive(false);
    }

    public override void OnStartAuthority() {
        CmdSetPlayerName(SteamFriends.GetPersonaName());
        gameObject.name = "LocalGamePlayer";
        SteamLobbyController._instance.FindLocalPlayer();
        SteamLobbyController._instance.UpdateLobbyName();
    }

    public override void OnStartClient() {
        _NetworkManager._players.Add(this);
        SteamLobbyController._instance.UpdateLobbyName();
        SteamLobbyController._instance.UpdatePlayerList();
    }

    public override void OnStopClient() {
        _networkManager._players.Remove(this);
        SteamLobbyController._instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string name) {
        this.PlayerNameUpdate(this._playerName, name);
    }
    private void PlayerNameUpdate(string oldValue, string newValue) {
        if(isServer) {
            this._playerName = newValue;
        }
        if(isClient) {
            SteamLobbyController._instance.UpdatePlayerList();
        }
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
