using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour {

    protected Callback<LobbyCreated_t> _lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> _joinRequest;
    protected Callback<LobbyEnter_t> _lobbyEnter;

    public ulong _currentLobbyID;
    private const string HostAddressKey = "HostAdress";
    private CustomNetworkManager _networkManager;

    public GameObject _hostButton;
    public Text _lobbyName;

    private void OnLobbyCreated(LobbyCreated_t callback) {
        if(callback.m_eResult != EResult.k_EResultOK)
            return;

        Debug.Log("Lobby Created Successfully");

        _networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }
}
