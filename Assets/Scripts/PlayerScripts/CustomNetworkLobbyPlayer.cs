using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkLobbyPlayer : NetworkBehaviour
{
    [SerializeField]
    private NetworkLobbyPlayer thisPlayer;
    [SerializeField]
    [SyncVar]
    private string name = null;

    public void Start()
    {
        if (isLocalPlayer)
        {
            name = FindObjectOfType<MenuUI>().GetPlayerName();
        }
        CmdSendName(name);
    }

    [Command]
    public void CmdSendName(string _name)
    {
        RpcSendName(_name);
    }

    [ClientRpc]
    private void RpcSendName(string _name)
    {
        name = _name;
        thisPlayer.name = name;
    }
}