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

        //Debug.Log($"{conn.connectionId}");

        GameObject _unitSpwn = Instantiate(playerFab, conn.identity.transform.position, conn.identity.transform.rotation);
        NetworkServer.Spawn(_unitSpwn, conn);
    }
    public override void OnClientConnect()
    {
        base.OnClientConnect();
    }


    public override void OnStartServer()
    {
        base.OnStartServer();

        ServerChangeScene("Game Scene");
    }

}
