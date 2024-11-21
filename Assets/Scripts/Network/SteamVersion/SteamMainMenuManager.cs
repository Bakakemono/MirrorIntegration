using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SteamMainMenuManager : MonoBehaviour {
    SteamGameManager _gameManager;

    [SerializeField] GameObject _mainMenuGroup;
    [SerializeField] GameObject _connectionGroup;

    private void Start() {
        _gameManager = FindObjectOfType<SteamGameManager>();
    }

    public void SwitchToConnectionSelection() {
        _mainMenuGroup.SetActive(false);
        _connectionGroup.SetActive(true);
    }

    public void SwitchToMainMenuSelection() {
        _mainMenuGroup.SetActive(true);
        _connectionGroup.SetActive(false);
    }

    public void Quit() {
        _gameManager.QuitGame();
    }

    public void StartHost() {
        _gameManager.StartHost();
    }
}
