using UnityEngine;
using Mirror;

public class PlayerModel : NetworkBehaviour
{
    [SerializeField] private Transform modelParent; 

    [SyncVar(hook = nameof(OnModelUpdated))]
    private string currentModelName;    
    [SyncVar(hook = nameof(OnBaseModelUpdated))]
    private string currentBaseModelName;

    private Animator anim;
    private NetworkAnimator networkAnim;
    private Transform baseModelGO;

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
    public void CmdChangeModel(string baseModelName, string modelName)
    {
        currentBaseModelName = baseModelName;
        currentModelName = modelName;
    }

    private void OnBaseModelUpdated(string oldModelName, string newModelName)
    {
        currentBaseModelName = newModelName;

        foreach (Transform child in modelParent)
        {
            child.gameObject.SetActive(false);
        }

        baseModelGO = modelParent.Find(currentBaseModelName);
        if (baseModelGO != null)
        {
            baseModelGO.gameObject.SetActive(true);
            anim.Rebind();
            networkAnim.animator = anim;
        }
        else
        {
            Debug.LogWarning($"Model with name {baseModelGO} not found under modelParent.");
        }
    }

    private void OnModelUpdated(string oldModelName, string newModelName)
    {
        if (baseModelGO == null)
            Debug.Log($"{baseModelGO}has not been set");

        currentModelName = newModelName;

        foreach (Transform child in baseModelGO)
        {
            child.gameObject.SetActive(false);
        }

        Transform targetModel = baseModelGO.Find(currentModelName);
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
