using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

	public float damage = 0f;
	public float speed = 15f;
	public float decayTime = 0f;

    public string ownerID;

    private void Start()
    {
    }

	void Update ()
    {
        decayTime -= Time.deltaTime;
        transform.position += transform.forward * 1f;
        if (decayTime < 0f) NetworkServer.Destroy(this.gameObject);
	}

    void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;
        if (other.gameObject.CompareTag("GameController"))
        {
            if (other.gameObject.GetComponent<NetworkIdentity>().netId.ToString() == ownerID) return;
            other.gameObject.GetComponent<PlayerNetworkActions>().TakeDamage(damage, "Player_" + ownerID);
            other.gameObject.GetComponent<PlayerNetworkActions>().CmdExplode(gameObject.transform.position);
            Debug.Log("Bullet with owner:" + ownerID + ", hit object:" + other.gameObject.name);
            Debug.Log("damage dealt: " + damage.ToString());
            NetworkServer.Destroy(this.gameObject);
        }
    }

    public void InitBullet(float bulletDamage, float bulletSpeed, string _ownerID)
    {
        ownerID = _ownerID;
		this.damage = bulletDamage;
		this.speed = bulletSpeed;
		this.decayTime = 2f;
	}
}
