using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CubeMovement : NetworkBehaviour
{
	public float DestroyTime = 10f;
	public float damage = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DestroyTime -= Time.deltaTime;
        if(DestroyTime <= 0){
        	NetworkServer.Destroy(this.gameObject);
        }
        transform.position += transform.forward * 2f;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GameController"))
        {
            other.gameObject.GetComponent<PlayerNetworkActions>().TakeDamage(damage, "Debris");
            other.gameObject.GetComponent<PlayerNetworkActions>().CmdExplode(gameObject.transform.position);
            NetworkServer.Destroy(this.gameObject);
        }
        if(other.gameObject.CompareTag("Enemy")){
        	other.gameObject.GetComponent<EnemyAI>().TakeDamage(damage);
        	NetworkServer.Destroy(this.gameObject);
        }
        if(other.gameObject.CompareTag("Asteroid")){
            other.gameObject.GetComponent<MeteorMotion>().TakeDamage(damage);
            NetworkServer.Destroy(this.gameObject);
        }
        if(other.gameObject.CompareTag("Unicorn")){
        	other.gameObject.GetComponent<NetworkUnicornActions>().TakeDamage(damage);
            NetworkServer.Destroy(this.gameObject);
        }
    }
}
