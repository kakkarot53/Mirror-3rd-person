using UnityEngine;
using System;
using Mirror;

public class PlayerMultSetup : NetworkBehaviour
{
    //this script is going to be used to caintain all the necessary datas to set up avatars and nickname display
    [SyncVar(hook = nameof(OnPlayerNameChange))]
    public string playerName;

    [SyncVar(hook = nameof(OnPlayerBaseAvatarChange))]
    public int baseAvatarIndex;
    [SyncVar(hook = nameof(OnPlayerAvatarChange))]
    public int avatarIndex;

    [SerializeField] private GameObject playerAvatarPrefab; // The prefab of player avatar to spawn to the world

    #region Server
    [Command]
    public void CmdChangePlayerName(string newName)
    {
        playerName = newName;
    }

    [Command]
    public void CmdChangeBaseAvatarIndex(int newBaseAvatarIndex)
    {
        baseAvatarIndex = newBaseAvatarIndex;
    }

    [Command]
    public void CmdChangeAvatarIndex(int newAvatarIndex)
    {
        avatarIndex = newAvatarIndex;
    }

    [Command]
    public void CmdRequestSpawnPlayerAvatar()
    {
        if (!isServer) return;
        Debug.Log("Request server to spawn Avatar");

        // Get Spawn location
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();

        // Spawn Avatar on the server
        GameObject localPlayerGameObject = Instantiate(playerAvatarPrefab, spawnPoint.position, spawnPoint.rotation);

        // Spawn the object on all clients
        NetworkServer.Spawn(localPlayerGameObject, connectionToClient);
    }
    #endregion

    #region Client
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (LocalPlayerNick.Instance != null)
        {
            CmdChangePlayerName(LocalPlayerNick.Instance.nickName); // Set Player Name based on offline scene input
            CmdChangeBaseAvatarIndex(LocalPlayerNick.Instance.playerBaseModelId); // Set Base Player Index based on offline scene input
            CmdChangeAvatarIndex(LocalPlayerNick.Instance.playerModelId); // Set Player Index based on offline scene input
        }
        else
        {
            CmdChangePlayerName($"Player {netId}"); // Set Player Name by netId
        }

        SpawnPlayerAvatar(); // Spawn Player Avatar
    }
    public void SpawnPlayerAvatar()
    {
        Debug.Log($"Connected: {NetworkClient.isConnected}");
        Debug.Log($"LocalPlayer: {NetworkClient.localPlayer != null}");

        CmdRequestSpawnPlayerAvatar();
    }
    public void OnPlayerNameChange(string oldName, string newName)
    {
        playerName = newName;
    }

    public void OnPlayerBaseAvatarChange(int oldValue, int newValue)
    {
        baseAvatarIndex = newValue;
    }

    public void OnPlayerAvatarChange(int oldValue, int newValue)
    {
        avatarIndex = newValue;
    }
    #endregion
}
