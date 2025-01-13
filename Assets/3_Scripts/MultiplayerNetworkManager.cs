using Mirror;
using UnityEngine;
using Cinemachine;
public class MultiplayerNetworkManager : NetworkManager
{
    [SerializeField]
    private GameObject playerFab;
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        //GameObject _unitSpwn = Instantiate(playerFab, conn.identity.transform.position, conn.identity.transform.rotation);
        //NetworkServer.Spawn(_unitSpwn, conn);
        //Debug.Log($"player Id: {conn.connectionId}'s {_unitSpwn.name} has been spawned usccessfully");

        //Transform _bmPar = _unitSpwn.transform.GetChild(0);
        //Debug.Log($"model parent name: {_bmPar.name}");

        //GameObject _bm = Instantiate(LocalPlayerNick.Instance.playerModelPrefab.playerBaseModel, _unitSpwn.transform.position, Quaternion.identity, _bmPar);
        //NetworkServer.Spawn(_bm, _unitSpwn);
        //Debug.Log($"base model: {_bm} has been spawned");
    }
    public override void OnClientConnect()
    {
        base.OnClientConnect();
    }


    public override void OnStartServer()
    {
        base.OnStartServer();
    }

}
