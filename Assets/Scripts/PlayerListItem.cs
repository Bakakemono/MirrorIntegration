using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour {
    public string _playerName;
    public int _connectionID;
    public ulong _playerSteamID;
    private bool _avatarReceived;

    public TMP_Text _playerNameText;
    public RawImage _playerIcon;

    protected Callback<AvatarImageLoaded_t> _imageLoaded;

    private void Start() {
        _imageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback) {
        if(callback.m_steamID.m_SteamID == _playerSteamID) {
            _playerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else {
            // Another Player
            return;
        }
    }

    private Texture2D GetSteamImageAsTexture(int iImage) {
        Texture2D texture = null;
        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

        if(isValid) {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if(isValid) {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        _avatarReceived = true;
        return texture;
    }
}