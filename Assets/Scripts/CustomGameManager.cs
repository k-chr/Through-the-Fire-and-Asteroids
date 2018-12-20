using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomGameManager : MonoBehaviour
{
    private const string PLAYER_ID_PREFIX = "Player_";
    private static Dictionary<string, ShipStats> players = new Dictionary<string, ShipStats>();

    public static void RegisterPlayer(string _netID, ShipStats _playerShipStats)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_playerID, _playerShipStats);
    }

    public static void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
    }

    public static ShipStats GetPlayerShip(string _playerID)
    {
        return players[_playerID];
    }

    public static int GetNumberOfPlayers()
    {
        return players.Count;
    }

    public static void ClearPlayers()
    {
        players.Clear();
    }

    public static string[] ReturnKeys()
    {
        return players.Keys.ToArray();
    }

    public static void UpdatePlayer(string _player, string what)
    {
        switch(what)
        {
            case "kills":
                players[_player]._matchStats.UpdateKills();
                players[_player]._matchStats.UpdatePoints();
                break;
            case "deaths": players[_player]._matchStats.UpdateDeaths(); break;
        }
    }
}