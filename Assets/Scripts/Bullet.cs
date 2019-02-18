using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

	public float damage = 0f;
	public float speed = 15f;
	public float decayTime = 0f;

    public string ownerID;

    [ServerCallback]
	void Update ()
    {
        decayTime -= Time.deltaTime;
        transform.position += transform.forward * speed/96;
        if (decayTime < 0f) NetworkServer.Destroy(this.gameObject);
	}

    void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.tag);
        if (!isServer) return;
        if (other.gameObject.CompareTag("GameController"))
        {
            Debug.Log("tutaj kurwa");
            if (other.gameObject.GetComponent<NetworkIdentity>().netId.ToString() != ownerID){
                other.gameObject.GetComponent<PlayerNetworkActions>().TakeDamage(damage, other.gameObject.tag + "_" + ownerID);
                other.gameObject.GetComponent<PlayerNetworkActions>().CmdExplode(gameObject.transform.position);
                Debug.Log("Bullet with owner:" + ownerID + ", hit object:" + other.gameObject.name);
                Debug.Log("damage dealt: " + damage.ToString());
                NetworkServer.Destroy(this.gameObject);
            }
        }
        else if(other.gameObject.CompareTag("Enemy")){
            Debug.Log("a teraz tutaj kurwa");
            if (other.gameObject.GetComponent<NetworkIdentity>().netId.ToString() == ownerID) return;
            other.gameObject.GetComponent<EnemyAI>().TakeDamage(damage);
            NetworkServer.Destroy(this.gameObject);
        }
        else if(other.gameObject.CompareTag("Asteroid")){
            Debug.Log("dupa2");
            if (other.gameObject.GetComponent<NetworkIdentity>().netId.ToString() == ownerID) return;
            other.gameObject.GetComponent<MeteorMotion>().TakeDamage(damage);
            NetworkServer.Destroy(this.gameObject);
        }
        else if(other.gameObject.CompareTag("Unicorn")){
            Debug.Log("no chyba tutaj kurwa");
            if (other.gameObject.GetComponent<NetworkIdentity>().netId.ToString() == ownerID) return;
            other.gameObject.GetComponent<NetworkUnicornActions>().TakeDamage(damage);
            NetworkServer.Destroy(this.gameObject);
        }
    }

	public void InitBullet(float bulletDamage, float bulletSpeed, string _ownerID)
    {
        ownerID = _ownerID;
		this.damage = bulletDamage;
		this.speed = bulletSpeed * 400000f;
		this.decayTime = 5f;
	}
}
