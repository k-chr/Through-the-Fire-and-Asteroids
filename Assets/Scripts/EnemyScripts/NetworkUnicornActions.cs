using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkUnicornActions : NetworkBehaviour {

	[SyncVar]
    public float curHealth;
    public enum State {chaseOpponent, randomWalk, runAway};
	public State state = State.randomWalk;
	
	public Transform shotSpawn1;
	public float timeToFindLittleSassy;
	public Transform shotSpawn2;
	public Quaternion target;
	public float shotTimer1;
	public float shotTimer2;
	public float rotationSpeed;
	public float speed = 2f;
	public float lookRange = 40.0f;
	//public Quaternion targetRotation = default(Quaternion);
	public GameObject player = null;
	public GameObject shot = null;
	private float timeToDirectionChange = 3.0f;

	void Start () {
		timeToFindLittleSassy = 15f;
		curHealth = 200.0f;
		timeToDirectionChange = -1.0f;
		rotationSpeed = 90.0f;
		shotTimer1 = 0.9f;
		shotTimer2 = shotTimer1 + 0.13f;	
	}
	
	void Update () {
		if(state == State.chaseOpponent){
			if(player.GetComponent<PlayerController>().Zombie == true){
				state = State.randomWalk;
				return;
			}
			if (shotTimer1 >= 0f) shotTimer1 -= Time.deltaTime;
			if (shotTimer2 >= 0f) shotTimer2 -= Time.deltaTime;
			Vector3 tmp = player.transform.position - new Vector3((shotSpawn1.position.x + shotSpawn2.position.x)/2, shotSpawn1.position.y, shotSpawn2.position.z);
			target = Quaternion.LookRotation(tmp);
			transform.rotation = Quaternion.Slerp(transform.rotation, target, speed * Time.deltaTime);
			float dist = Vector3.Distance(transform.position, player.transform.position);
			if(dist <= 40f)
				transform.position += transform.forward * speed/6 * Time.deltaTime;
			else
				transform.position += transform.forward * speed*2 * Time.deltaTime;
			if(dist <= lookRange){
				 if (shotTimer1 <= 0f){
       			    EnemyShoot(true);
            		shotTimer1 = 0.9f;
       			 }
       			 if(shotTimer2 <= 0f){
       			 	EnemyShoot(false);
       			 	shotTimer1 = 0.9f;
       			 	shotTimer2 = shotTimer1 + 0.13f;
       			 }
			}
		}
		else if(state == State.randomWalk){
			if(timeToDirectionChange >= 0f) timeToDirectionChange -= Time.deltaTime;
			if(timeToFindLittleSassy >= 0f) timeToFindLittleSassy -= Time.deltaTime;
			
			if(timeToDirectionChange <= 0f){
				timeToDirectionChange = 15.0f;
				Vector3 tempTarget = new Vector3(transform.position.x + Random.Range(-30.0f, 30.0f),
														transform.position.y + Random.Range(-30.0f, 30.0f),
														transform.position.z + Random.Range(-30.0f, 30.0f));
				target = Quaternion.LookRotation(tempTarget - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
			}
			transform.position += transform.forward * speed * Time.deltaTime;
			if(timeToFindLittleSassy <= 0f) {
				player = FindNearestPlayer();
				timeToFindLittleSassy = 15f;
				if(player != null){
					state = State.chaseOpponent;
					return;
				}
			}
		}
		else{
			//run run run run away ...
		}
	}
	public GameObject FindNearestPlayer(){
		GameObject[] playerList = GameObject.FindGameObjectsWithTag("GameController");
		float minDistance = lookRange + 20.0f;
		int index = -1;
		for(int i = 0; i < playerList.Length; ++i){
			float dist = Vector3.Distance(transform.position, playerList[i].transform.position);
			if(dist < minDistance){
				index = i;
				minDistance = dist;
			}
		}
		if(index == -1)
			return null;
		return playerList[index];
	}
	public void EnemyShoot(bool choose) { 
		if(choose == true){
			CmdShoot(shotSpawn1.position, shotSpawn1.rotation, "Unicorn_" + this.GetComponent<NetworkIdentity>().netId.ToString()); 
		}
		else{
			CmdShoot(shotSpawn2.position, shotSpawn2.rotation, "Unicorn_" + this.GetComponent<NetworkIdentity>().netId.ToString()); 
		}
	}
	[Command]
	public void CmdShoot(Vector3 _shotPos, Quaternion _shotRot, string _Owner)
    {
        GameObject newBullet = Instantiate(shot, _shotPos, _shotRot) as GameObject;
        //static values for testing on multiple instances on one computer
        newBullet.GetComponent<Bullet>().InitBullet(
                                                   //damage,
                                                   25,
                                                   //shotSpeed,
                                                   10f,
                                                    _Owner);
        NetworkServer.Spawn(newBullet);
        //RpcSendSoundToClients(0);
    }
    public void TakeDamage(float amount){
        if (curHealth <= 0f)
            return;
        curHealth -= amount;
        if (curHealth <= 0)
        {
            curHealth = 0f;
            NetworkServer.Destroy(this.gameObject);
        }
	}
    // [ClientRpc]
    // public void RpcSendSoundToClients(int id)
    // {
    //     if (id > 0 && id < clips.Length)
    //         audioSource.PlayOneShot(clips[id], 1);
    // }

}
