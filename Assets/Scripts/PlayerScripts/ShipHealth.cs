using UnityEngine;
using UnityEngine.Networking;

public class ShipHealth : NetworkBehaviour
{
    //God damn it, for future use. Methods with [ClientRpc] attribute are called ONLY when server calls them
    //so if you want to do something from client that needs to be executed on all clients use method with [Command]
    //attribute and within method with mentioned attribute call method with attribute [ClientRpc]
    //for now, we can adjust it later if needed
    public float maxHealth = 100f;
    public float health = 0f;

	void Start ()
    {
        health = maxHealth;
	}
	
    [ClientRpc]
    public void RpcTakeDamage(float amount, NetworkInstanceId attacker)
    {
        if (health <= 0f)
            return;

        string thisPlayerID = gameObject.GetComponent<NetworkIdentity>().netId.ToString();
        string attackerPlayerID = attacker.ToString();

        health -= amount;

        if( health <= 0)
        {
            health = 0f;
            CmdPlayerDied(attackerPlayerID, thisPlayerID);
            gameObject.GetComponent<PlayerController>().Died();
        }
    }

    [Command]
    public void CmdPlayerDied(string whoKilled, string whoDied)
    {
        RpcPlayerDied(whoKilled, whoDied);
    }

    [ClientRpc]
    public void RpcPlayerDied(string whoKilled, string whoDied)
    {
        Debug.Log("PlayerID(" + whoKilled + ") died, killed by PlayerID(" + whoDied + ")");
        CustomGameManager.UpdatePlayer(whoKilled, "kills");
        CustomGameManager.UpdatePlayer(whoDied, "deaths");
    }
}
