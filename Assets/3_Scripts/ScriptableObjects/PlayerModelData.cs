using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerModelData", menuName = "ScriptableObjects/PlayerModelData")]
public class PlayerModelData : ScriptableObject
{
    public GameObject playerPrefab;
    public Avatar playerAvatar;
}
