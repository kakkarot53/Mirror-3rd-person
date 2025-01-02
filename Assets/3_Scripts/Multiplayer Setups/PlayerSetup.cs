using UnityEngine;
using Mirror;
using System;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform = null;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;
    [SyncVar] private GameObject playerModelPrefab;

    public static event Action ClientOnInfoUpdated;

    private Transform localPlayerModel;
    private void Start()
    {
        if (!isOwned)
            return;

        GameObject[] _Models = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject m in _Models)
        {
            NetworkIdentity _id = m.GetComponent<NetworkIdentity>();
            if (_id != null && _id.isOwned)
            {
                localPlayerModel = m.transform;
                break;
            }
        }

        //setup model
        PlayerModel _model = localPlayerModel.GetComponent<PlayerModel>();
        if (LocalPlayerNick.Instance != null)
        {
            _model.SetAvatar(LocalPlayerNick.Instance.playerModelPrefab.playerAvatar);
            _model.CmdChangeModel(LocalPlayerNick.Instance.playerModelPrefab.playerBaseModel.name, 
                LocalPlayerNick.Instance.playerModelPrefab.playerModel.name);
        }

        //setup name disp
        localPlayerModel.GetChild(1).TryGetComponent<NamePlate>(out NamePlate _plate);
        if (LocalPlayerNick.Instance != null)
            _plate.CmdSetName(LocalPlayerNick.Instance.nickName);
        Debug.Log($"nameplate nickname is set to {LocalPlayerNick.Instance.nickName}");
        _plate.SetCam(cameraTransform.GetComponent<Camera>());

        //setup player move
        localPlayerModel.TryGetComponent<CharMove>(out CharMove _move);
        _move.enabled = true;
        _move.thirdPersonCam = cameraTransform.gameObject;


    }

    #region Get Variables
    public string GetDisplayName()
    {
        return displayName;
    }
    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }
    #endregion

    [Command]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
    {
        ClientOnInfoUpdated?.Invoke();
    }
}
