using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomSteamNetworkManager : NetworkManager {
    [SerializeField] private PlayerControllerSteam _playerController;
    public List<PlayerControllerSteam> _players { get; } = new List<PlayerControllerSteam>();

    public override void OnServerAddPlayer(NetworkConnectionToClient connection) {
        if(SceneManager.GetActiveScene().name == "Lobby") {
            PlayerControllerSteam playerInstance = Instantiate(_playerController);

            playerInstance._connectionID = connection.connectionId;
            playerInstance._playerIdNumber = _players.Count + 1;
            playerInstance._playerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby._instance._currentLobbyID, _players.Count);

            NetworkServer.AddPlayerForConnection(connection, playerInstance.gameObject);
        }
    }

    public void StartGame(string sceneName) {
        ServerChangeScene(sceneName);
    }
}
