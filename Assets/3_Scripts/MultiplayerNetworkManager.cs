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

        GameObject _unitSpwn = Instantiate(playerFab, conn.identity.transform.position, conn.identity.transform.rotation);
        NetworkServer.Spawn(_unitSpwn, conn);

        Debug.Log($"player Id: {conn.connectionId}'s {_unitSpwn.name} has been spawned usccessfully");

        GameObject _mng = conn.identity.gameObject;
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
