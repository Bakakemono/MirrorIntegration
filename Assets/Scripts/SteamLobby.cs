using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal;

public class SteamLobby : MonoBehaviour {

    protected Callback<LobbyCreated_t> _lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> _joinRequest;
    protected Callback<LobbyEnter_t> _lobbyEnter;

    public ulong _currentLobbyID;
    private const string HostAddressKey = "HostAdress";
    private CustomSteamNetworkManager _networkManager;

    public GameObject _hostButton;
    public TMP_Text _lobbyName;

    public static SteamLobby _instance;

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }
        else {
            Destroy(this);
        }
    }

    private void Start() {
        if(!SteamManager.Initialized)
            return;

        _networkManager = GetComponent<CustomSteamNetworkManager>();

        _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        _joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinedRequest);
        _lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    public void HostLobby() {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _networkManager.maxConnections);

    }

    private void OnLobbyCreated(LobbyCreated_t callback) {
        if(callback.m_eResult != EResult.k_EResultOK)
            return;

        Debug.Log("Lobby Created Successfully");

        _networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }

    private void OnJoinedRequest(GameLobbyJoinRequested_t callback) {
        Debug.Log("Request to join Lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t callback) {
        //Everyone
        _hostButton.SetActive(false);
        _currentLobbyID = callback.m_ulSteamIDLobby;
        _lobbyName.gameObject.SetActive(true);
        _lobbyName.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        //client
        if(NetworkServer.active)
            return;

        _networkManager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        _networkManager.StartClient();
    }
}
