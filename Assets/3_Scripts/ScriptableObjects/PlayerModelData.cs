using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerModelData", menuName = "ScriptableObjects/PlayerModelData")]
public class PlayerModelData : ScriptableObject
{
    public GameObject playerBaseModel;
    public GameObject playerModel;
    public Avatar playerAvatar;
}
