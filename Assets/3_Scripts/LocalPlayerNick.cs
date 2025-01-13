using UnityEngine;
using TMPro;

public class LocalPlayerNick : MonoBehaviour
{
    public static LocalPlayerNick Instance;

    public string nickName { private set; get; }
    public int playerModelId { private set; get; }
    public int playerBaseModelId { private set; get; }

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
    public void SetModelId(int id)
    {
        playerModelId = id;
    }
    public void SetBaseModelId(int id)
    {
        playerBaseModelId = id;
    }
}