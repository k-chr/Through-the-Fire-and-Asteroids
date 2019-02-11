using UnityEngine;
using UnityEngine.Networking;

public class ShipHealth : NetworkBehaviour {

    //for now, we can adjust it later if needed
    public float maxHealth = 100f;
    public float health = 0f;

	void Start () {
        health = maxHealth;
	}
	
    public void TakeDamage(float amount, NetworkInstanceId from)
    {
        if (!isServer || health <= 0f)
            return;

        health -= amount;

        if( health <= 0)
        {
            health = 0f;
            RpcDied(from);
            return;
        }
    }

    [ClientRpc]
    public void RpcDied(NetworkInstanceId from)
    {
        Debug.Log("PlayerID(" + from + ") died, killed by PlayerID(" + gameObject.GetComponent<NetworkIdentity>().netId + ")");
    }
}
