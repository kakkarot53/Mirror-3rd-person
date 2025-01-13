using UnityEngine;
using Mirror;
using Cinemachine;
public class CamSetup : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform = null;

    public void SetCamFollow(Transform follow)
    {
        if (!isOwned)
            return;

        playerCameraTransform.gameObject.SetActive(true);
        playerCameraTransform.tag = "MainCamera";

        ThirdPersonOrbitCamBasic _brain = playerCameraTransform.GetComponent<ThirdPersonOrbitCamBasic>();
        _brain.player = follow;
        if (_brain.enabled == false) _brain.enabled = true;
    }
}
