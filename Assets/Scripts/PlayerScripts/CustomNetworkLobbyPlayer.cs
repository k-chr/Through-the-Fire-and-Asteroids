using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkLobbyPlayer : MonoBehaviour
{
    [SerializeField]
    private NetworkLobbyPlayer thisPlayer;

    void Start()
    {
        thisPlayer.name = FindObjectOfType<MenuUI>().GetPlayerName();
    }
}
