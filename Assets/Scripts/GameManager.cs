using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private const string PLAYER_ID_PREFIX = "Player_";
    private static Dictionary<string, ShipStats> players = new Dictionary<string, ShipStats>();

    public void RegisterPlayer(string _netID, ShipStats _player)
    {
        string player_id = PLAYER_ID_PREFIX + _netID;
        players.Add(player_id, _player);
        _player.transform.name = player_id;
    }

    public void UnregisterPlayer(string _netID)
    {
        return;
    }

    public ShipStats GetPlayer(string _playerID)
    {
        return null;
    }

}
