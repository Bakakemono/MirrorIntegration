using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class SteamLobbyController : MonoBehaviour {
    public static SteamLobbyController _instance;

    // UI Elements
    public TMP_Text _lobbyNameText;

    // Player Data
    public GameObject _playerListViewContent;
    public GameObject _playerListItemPrefab;
    public GameObject _localPlayerObject;

    // Other Data
    public ulong _currentLobbyID;
    public bool _playerItemCreated = false;
    private List<PlayerListItem> _playerListItems = new List<PlayerListItem>();
    public PlayerControllerSteam _localPlayerController;

    // Manager
    private CustomSteamNetworkManager _NetworkManager;
    private CustomSteamNetworkManager _networkManager{
        get {
            if(_NetworkManager != null)
                return _NetworkManager;

            return _NetworkManager = CustomSteamNetworkManager.singleton as CustomSteamNetworkManager;
        }
    }

    private void Awake() {
        if(_instance == null)
            _instance = this;
    }

    public void UpdateLobbyName() {
        _currentLobbyID = _networkManager.GetComponent<SteamLobby>()._currentLobbyID;
        _lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(_currentLobbyID), "name");
    }

    public void UpdatePlayerList() {
        if(!_playerItemCreated)
            CreateHostPlayerItem();

        if(_playerListItems.Count < _networkManager._players.Count)
            CreateClientPlayerItem();

        if(_playerListItems.Count > _networkManager._players.Count)
            RemoveClientPlayerItem();

        if(_playerListItems.Count == _networkManager._players.Count)
            UpdatePlayerItem();
    }

    public void FindLocalPlayer() {
        //_localPlayerController =  GameObject.Find()
    }

    public void CreateHostPlayerItem() {

    }

    public void CreateClientPlayerItem() {

    }

    public void UpdatePlayerItem() {

    }

    public void RemoveClientPlayerItem() {

    }
}
