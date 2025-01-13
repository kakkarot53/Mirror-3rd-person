using UnityEngine;
using Mirror;
using TMPro;
public class NamePlate : NetworkBehaviour
{
    private Transform _cam;

    [SerializeField]
    private TMP_Text nameTxt;

    [SyncVar(hook = nameof(OnNameDisplay))] //=> calls this
    private string playerNickName;
    
    private PlayerMultSetup setup;

    private void Start()
    {
        if (Camera.main != null)
        {
            _cam = Camera.main.transform;
        }
    }

    public override void OnStartClient()
    {
        if (!isOwned)
            return;

        setup = NetworkClient.localPlayer.GetComponent<PlayerMultSetup>();
        //_cam = setup.cameraTransform;

        UpdateDisplayName();
    }

    public void UpdateDisplayName()
    {
        if (setup == null || nameTxt == null)
            return;

        CmdSetName(LocalPlayerNick.Instance.nickName);
    }

    void Update()
    {
        if (_cam != null)
        transform.LookAt(_cam.transform);
    }

    [Command]
    public void CmdSetName(string name)
    {
        playerNickName = name;
    }

    private void OnNameDisplay(string oldDisplayName, string newDisplayName)
    {
        playerNickName = newDisplayName;
        nameTxt.text = playerNickName;
    }

    public void SetCam(Camera cam)
    {
        _cam = cam.transform;
    }
}
