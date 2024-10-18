using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    
    public static GameManager _instance;
    List<PlayerController> _players = new List<PlayerController>();
    bool _arePlayersSpawned = false;

    void Start () {
        // Making sure there is only one instance of Game Manager
        if(_instance == null) {
            _instance = this;
        }
        else {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(gameObject);
        CustomDefaultNetworkManager.singleton.networkAddress = "localHost";

        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ChangeIpAdress(string ipAdress) {
        CustomDefaultNetworkManager.singleton.networkAddress = ipAdress;
    }
    
    public void StartHost() {
        if(!CustomDefaultNetworkManager.singleton.isNetworkActive)
            CustomDefaultNetworkManager.singleton.StartHost();
        CustomDefaultNetworkManager.singleton.ServerChangeScene("S_Lobby");
        //CustomDefaultNetworkManager.singleton.OnServerAddPlayer()
    }

    public void StartClient() {
        CustomDefaultNetworkManager.singleton.StartClient();
    }
    
    public void SpawnPlayers() {
        CustomDefaultNetworkManager.singleton.SpawnPlayers();
    }
    public void RegisterPlayer(PlayerController player) {
        _players.Add(player);
    }

    void ChangedActiveScene(Scene current, Scene next) {
        Debug.Log("Scene Changed to " + next.name);
        //if(SceneManager.GetActiveScene().name == "S_Lobby")
        //    SpawnPlayer();
    }
    public void OnPlayerSpawned() {
        CustomDefaultNetworkManager.singleton.ServerChangeScene("S_Game");
    }
}
