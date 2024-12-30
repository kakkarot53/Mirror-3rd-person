using UnityEngine;
using Mirror;
using TMPro;
public class NamePlate : NetworkBehaviour
{
    private Camera _cam;
    [SerializeField]
    private TMP_Text nameTxt;
    [SyncVar(hook = nameof(ChangeName))] //=> calls this
    private string playerNickName;
    void Start()
    {
        //if (transform.root.GetComponent<NetworkIdentity>().isOwned == true)
        //    gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_cam == null)
            _cam = Camera.main;
        transform.LookAt(_cam.transform);
    }
    [Command]
    public void CmdSetName(string name)
    {
        playerNickName = name;
    }

    private void ChangeName(string oldDisplayName, string newDisplayName)
    {
        playerNickName = newDisplayName;
        nameTxt.text = playerNickName;
    }

    public void SetCam(Camera cam)
    {
        _cam = cam;
    }
}
