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
    private List<SteamPlayerListItem> _playerListItems = new List<SteamPlayerListItem>();
    public SteamPlayerController _localPlayerController;

    // Manager
    private CustomSteamNetworkManager _NetworkManager;
    private CustomSteamNetworkManager _networkManager {
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
        _localPlayerObject = GameObject.Find("LocalGamePlayer");
        _localPlayerController = _localPlayerObject.GetComponent<SteamPlayerController>();
    }

    public void CreateHostPlayerItem() {
        foreach(SteamPlayerController player in _networkManager._players) {
            GameObject newPlayerItem = Instantiate(_playerListItemPrefab) as GameObject;
            SteamPlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<SteamPlayerListItem>();

            newPlayerItemScript._playerName = player._playerName;
            newPlayerItemScript._connectionID = player._connectionID;
            newPlayerItemScript._playerSteamID = player._playerSteamID;
            newPlayerItemScript.SetPlayerValue();

            newPlayerItem.transform.SetParent(_playerListViewContent.transform);
            newPlayerItem.transform.localScale = Vector3.one;

            _playerListItems.Add(newPlayerItemScript);
        }

        _playerItemCreated = true;
    }

    public void CreateClientPlayerItem() {
        foreach(SteamPlayerController player in _networkManager._players) {
            if(!_playerListItems.Any(b => b._connectionID == player._connectionID)) {
                GameObject newPlayerItem = Instantiate(_playerListItemPrefab);
                SteamPlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<SteamPlayerListItem>();

                newPlayerItemScript._playerName = player._playerName;
                newPlayerItemScript._connectionID = player._connectionID;
                newPlayerItemScript._playerSteamID = player._playerSteamID;
                newPlayerItemScript.SetPlayerValue();

                newPlayerItem.transform.SetParent(_playerListViewContent.transform);
                newPlayerItem.transform.localScale = Vector3.one;

                _playerListItems.Add(newPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem() {
        foreach(SteamPlayerController player in _networkManager._players) {
            foreach(SteamPlayerListItem playerListItemScript in _playerListItems) {
                if(playerListItemScript._connectionID == player._connectionID) {
                    playerListItemScript._playerName = player._playerName;
                    playerListItemScript.SetPlayerValue();
                }
            }
        }
    }

    public void RemoveClientPlayerItem() {
        List<SteamPlayerListItem> playerListItemsToRemove = new List<SteamPlayerListItem>();

        foreach(SteamPlayerListItem playerlistItem in _playerListItems) {
            if(!_networkManager._players.Any(b => b._connectionID == playerlistItem._connectionID)){
                playerListItemsToRemove.Add(playerlistItem);
            }
        }
        if(playerListItemsToRemove.Count > 0) {
            foreach(SteamPlayerListItem playerListItemToRemove in playerListItemsToRemove) {
                GameObject objectToRemove = playerListItemToRemove.gameObject;
                _playerListItems.Remove(playerListItemToRemove);
                Destroy(objectToRemove);
            }
        }
    }
}
