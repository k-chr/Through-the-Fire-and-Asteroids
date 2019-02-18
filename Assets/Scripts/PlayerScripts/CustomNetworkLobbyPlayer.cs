using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkLobbyPlayer : NetworkBehaviour
{
    [SerializeField]
    private NetworkLobbyPlayer thisPlayer;
    private string name = null;

    public void Setup(string _name)
    {
        name = _name;
    }

    void Start()
    {
        if (name != null)
        {
            thisPlayer.name = name;
        }
    }

    public override void OnStartLocalPlayer()
    {
        name = FindObjectOfType<MenuUI>().GetPlayerName();
    }

    private void OnDisable()
    {
        if (isLocalPlayer)
            GetComponent<MenuUI>().isReady = false;
    }
}