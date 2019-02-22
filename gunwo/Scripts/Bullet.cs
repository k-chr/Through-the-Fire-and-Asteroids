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
        transform.position += transform.forward * speed * Time.deltaTime;
        if (decayTime < 0f) NetworkServer.Destroy(this.gameObject);
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collider tag = " + other.gameObject.tag + other.gameObject.GetComponent<NetworkIdentity>().netId.ToString() + " _ownerID = " + ownerID);
        if (!isServer) return;
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("tutaj kurwa");
            if (other.gameObject.tag + "_" + other.gameObject.GetComponent<NetworkIdentity>().netId.ToString() != ownerID){
                other.gameObject.GetComponent<PlayerNetworkActions>().TakeDamage(damage, ownerID);
                other.gameObject.GetComponent<PlayerNetworkActions>().CmdExplode(gameObject.transform.position);
                Debug.Log("Bullet with owner:" + ownerID + ", hit object:" + other.gameObject.name);
                Debug.Log("damage dealt: " + damage.ToString());
                NetworkServer.Destroy(this.gameObject);
            }
        }
        else if(other.gameObject.CompareTag("Enemy")){
            
            if (other.gameObject.tag + "_" + other.gameObject.GetComponent<NetworkIdentity>().netId.ToString() == ownerID) return;
            Debug.Log("a teraz tutaj kurwa");
            other.gameObject.GetComponent<EnemyAI>().TakeDamage(damage);
            Debug.Log("Bullet with owner:" + ownerID + ", hit object:" + other.gameObject.name);
            NetworkServer.Destroy(this.gameObject);
        }
        else if(other.gameObject.CompareTag("Asteroid")){
            Debug.Log("dupa2");
            
            other.gameObject.GetComponent<MeteorMotion>().TakeDamage(damage);
            NetworkServer.Destroy(this.gameObject);
        }
        else if(other.gameObject.CompareTag("Unicorn")){
            
            if (other.gameObject.tag + "_" + other.gameObject.GetComponent<NetworkIdentity>().netId.ToString() == ownerID) return;
            Debug.Log("no chyba tutaj kurwa");
            other.gameObject.GetComponent<NetworkUnicornActions>().TakeDamage(damage);
            NetworkServer.Destroy(this.gameObject);
        }
    }

	public void InitBullet(float bulletDamage, float bulletSpeed, string _ownerID)
    {
        ownerID = _ownerID;
		this.damage = bulletDamage;
		this.speed = bulletSpeed*3f;
		this.decayTime = 90f;
	}
}
