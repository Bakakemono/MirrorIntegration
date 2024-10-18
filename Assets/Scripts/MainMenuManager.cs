using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {
    GameManager _gameManager;

    [SerializeField] GameObject _mainMenuGroup;
    [SerializeField] GameObject _connectionGroup;

    [SerializeField] TMP_Text _IpAdressText;

    private void Start() {
        _gameManager = FindObjectOfType<GameManager>();
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

    public void OnIpAdressChange() {
        _gameManager.ChangeIpAdress(_IpAdressText.text);
    }

    public void StartHost() {
        _gameManager.StartHost();
    }

    public void StartClient() {
        _gameManager.StartClient();
    }
}
