using UnityEngine;
using Mirror;
using System;

public class AvatarSetup : NetworkBehaviour
{
    [SerializeField]
    private CharMove move;
    [SerializeField]
    private NamePlate namePlate;
    [SerializeField]
    private Transform camTransform;

    [SerializeField]
    private Transform avatarParent;
    [SerializeField]
    private Avatar[] skeleton;

    [SerializeField]
    private Animator anim;

    private Transform baseAvatar;

    [SyncVar(hook = nameof(OnBaseAvatarIndexChanged))]
    public int baseAvatarIndex;
    [SyncVar(hook = nameof(OnAvatarIndexChanged))] 
    public int avatarIndex;

    private PlayerMultSetup setup;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isOwned)
        {
            Debug.Log($"{netIdentity.netId} Avatar: is not yours");
            return;
        }

        setup = NetworkClient.localPlayer.GetComponent<PlayerMultSetup>();

        //setup charmove
        move.enabled = true;
        move.thirdPersonCam = camTransform.gameObject;

        namePlate.SetCam(camTransform.GetComponent<Camera>());
        GetComponent<CamSetup>().SetCamFollow(this.transform);

        // Setup Base Avatar
        baseAvatarIndex = LocalPlayerNick.Instance.playerBaseModelId;
        ShowBaseAvatar();
        CmdChangeBaseAvatarIndex(baseAvatarIndex); // Request to update server avatar

        // Setup Base Avatar
        avatarIndex = LocalPlayerNick.Instance.playerModelId;
        ShowAvatar(); // Show local avatar
        CmdChangeAvatarIndex(avatarIndex); // Request to update server avatar
    }

    #region base avatar
    [Command]
    public void CmdChangeBaseAvatarIndex(int newId)
    {
        baseAvatarIndex = newId;
        Debug.Log($"command base avatar index change to {baseAvatarIndex}");
    }
    public void OnBaseAvatarIndexChanged(int oldValue, int newValue)
    {
        baseAvatarIndex = newValue;
        Debug.Log($"avatar index changed to {baseAvatarIndex}");
        ShowBaseAvatar();
    }

    private void ShowBaseAvatar()
    {
        baseAvatar = avatarParent.GetChild(baseAvatarIndex);
        baseAvatar.gameObject.SetActive(true);
        anim.avatar = skeleton[baseAvatarIndex];

        Debug.Log($"activating {baseAvatar.name}");
    }
    #endregion

    #region avatar
    [Command]
    public void CmdChangeAvatarIndex(int newId)
    {
        avatarIndex = newId;
        Debug.Log($"command avatar index change to {avatarIndex}");
    }
    public void OnAvatarIndexChanged(int oldValue, int newValue)
    {
        avatarIndex = newValue;
        Debug.Log($"avatar index changed to {avatarIndex}");
        ShowAvatar();
    }

    private void ShowAvatar()
    {
        Transform avatar = baseAvatar.GetChild(0).GetChild(avatarIndex);
        avatar.gameObject.SetActive(true);
        anim.Rebind();
    }
    #endregion
}
