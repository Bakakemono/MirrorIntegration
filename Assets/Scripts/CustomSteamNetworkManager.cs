using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CustomSteamNetworkManager : NetworkManager {
    [SerializeField] private PlayerControllerSteam _playerController;
    public List<PlayerControllerSteam> _players { get; } = new List<PlayerControllerSteam>();

    public override void OnServerAddPlayer(NetworkConnectionToClient connection) {
        if(SceneManager.GetActiveScene().name == "Lobby") {
            PlayerControllerSteam playerInstance = Instantiate(_playerController);
        }
    }

    public void StartGame(string sceneName) {
        ServerChangeScene(sceneName);
    }
}
