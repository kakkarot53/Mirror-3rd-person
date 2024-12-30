using UnityEngine;
using Mirror;
using Cinemachine;
public class CamSetup : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform = null;

    private Transform localPlayerModel;

    private void Start()
    {
        if (!isOwned)
            return;
        playerCameraTransform.tag = "MainCamera";
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

        playerCameraTransform.gameObject.SetActive(true);

        //setup 3rd person cam
        ThirdPersonOrbitCamBasic _brain = playerCameraTransform.GetComponent<ThirdPersonOrbitCamBasic>();
        _brain.player = localPlayerModel;
        if (_brain.enabled == false) _brain.enabled = true;
    }
}
