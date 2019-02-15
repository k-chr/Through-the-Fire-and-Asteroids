using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkLobbyPlayer : NetworkBehaviour
{
    [SerializeField]
    private NetworkLobbyPlayer thisPlayer;
    private string name = null;

    void Start()
    {
        if(isLocalPlayer)
            name = FindObjectOfType<MenuUI>().GetPlayerName();
        if(name != null)
            thisPlayer.name = name;
    }
}