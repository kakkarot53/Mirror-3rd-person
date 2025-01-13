using UnityEngine;
using Mirror;

public class PlayerModel : NetworkBehaviour
{
    [SerializeField] private Transform modelParent; 

    [SyncVar(hook = nameof(OnModelUpdated))]
    private string currentModelName;    

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
    public void SetBaseModel(GameObject baseModel)
    {
        baseModelGO = baseModel.transform;
    }

    [Command]
    public void CmdChangeModel(string modelName)
    {
        currentModelName = modelName;
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
