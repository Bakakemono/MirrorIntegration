using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomSteamNetworkManager : NetworkManager {
    [SerializeField] public SteamPlayerController _playerController;
    
    public List<SteamPlayerController> _players { get; } = new List<SteamPlayerController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient connection) {
        if(SceneManager.GetActiveScene().name == "S_Lobby") {
            SteamPlayerController playerInstance = Instantiate(_playerController);

            playerInstance._connectionID = connection.connectionId;
            //playerInstance._playerIdNumber = _players.Count + 1;
            playerInstance._playerSteamID =
                (ulong)SteamMatchmaking.GetLobbyMemberByIndex(
                    (CSteamID)SteamLobby._instance._currentLobbyID,
                    _players.Count
                    );
            
            NetworkServer.AddPlayerForConnection(connection, playerInstance.gameObject);
        }
    }

    public void StartGame(string sceneName) {
        ServerChangeScene(sceneName);
    }
}
