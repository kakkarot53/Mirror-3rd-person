using UnityEngine;
using Mirror;

public class PlayerModel : NetworkBehaviour
{
    [SerializeField] private Transform modelParent; 

    [SyncVar(hook = nameof(OnModelUpdated))]
    private string currentModelName;

    private Animator anim;
    private NetworkAnimator networkAnim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        networkAnim = GetComponent<NetworkAnimator>();
    }

    public void SetAvatar(Avatar avatar)
    {
        anim.avatar = avatar;
    }

    [Command]
    public void CmdChangeModel(string modelName)
    {
        currentModelName = modelName;
    }

    private void OnModelUpdated(string oldModelName, string newModelName)
    {
        currentModelName = newModelName;

        foreach (Transform child in modelParent)
        {
            child.gameObject.SetActive(false);
        }

        Transform targetModel = modelParent.Find(currentModelName);
        if (targetModel != null)
        {
            targetModel.gameObject.SetActive(true);

            anim.Rebind();

            networkAnim.animator = anim;

        }
        else
        {
            Debug.LogWarning($"Model with name {currentModelName} not found under modelParent.");
        }
    }
}
