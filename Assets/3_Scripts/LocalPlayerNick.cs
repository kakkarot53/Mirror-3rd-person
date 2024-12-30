using UnityEngine;
using TMPro;

public class LocalPlayerNick : MonoBehaviour
{
    public static LocalPlayerNick Instance;

    public string nickName { private set; get; }
    public PlayerModelData playerModelPrefab { private set; get; }

    private void Awake()
    {
        // Ensure only one instance of this object exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SetNickname(string name)
    {
        nickName = name;
    }    
    public void SetModel(PlayerModelData data)
    {
        playerModelPrefab = data;
    }
}