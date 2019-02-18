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
        if (players[_playerID] != null)
            players.Remove(_playerID);
        else
            Debug.Log("You wanted to acces object with key that doesn't exist in this list.");
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

    public static bool isIn(string _playerID)
    {
        if (players.ContainsKey(_playerID))
            return true;
        else
            return false;
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