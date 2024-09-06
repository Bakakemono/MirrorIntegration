using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CustomSteamNetworkManager : NetworkManager {
    [SerializeField] private PlayerController _playerPrefab;
    public List<PlayerController> _players { get; } = new List<PlayerController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient connection) {
        if(SceneManager.GetActiveScene().name == "Lobby") {
            PlayerController playerInstance = Instantiate(_playerPrefab);
        }
    }

    public void StartGame(string SceneName) {

    }
}
