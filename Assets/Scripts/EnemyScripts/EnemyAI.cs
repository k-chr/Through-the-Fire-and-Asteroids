using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyAI : NetworkBehaviour {

	[SyncVar]
    public float curHealth;
    public enum State {chaseOpponent, randomWalk, runAway};
	public State state = State.randomWalk;
	
	public Transform shotSpawn;
	public float timeToFindLittleSassy;
	
	public Quaternion target;
	public float shotTimer;
	
	public float rotationSpeed;
	public float speed = 2f;
	public float lookRange = 40.0f;
	//public Quaternion targetRotation = default(Quaternion);
	public GameObject player = null;
	public GameObject shot = null;
	private float timeToDirectionChange = 3.0f;

	void Start () {
		timeToFindLittleSassy = 15f;
		curHealth = 250.0f;
		timeToDirectionChange = -1.0f;
		rotationSpeed = 90.0f;
		shotTimer = 1.3f;	
	}
	
	void Update () {
		if(state == State.chaseOpponent){
			if(player.GetComponent<PlayerController>().Zombie == true){
				state = State.randomWalk;
				return;
			}
			if (shotTimer >= 0f) shotTimer -= Time.deltaTime;
			Vector3 tmp = player.transform.position - shotSpawn.position;
			target = Quaternion.LookRotation(tmp);
			transform.rotation = Quaternion.Slerp(transform.rotation, target, speed * Time.deltaTime);
			float dist = Vector3.Distance(transform.position, player.transform.position);
			if(dist <= 80f)
				transform.position += transform.forward * speed/6 * Time.deltaTime;
			else
				transform.position += transform.forward * speed*2 * Time.deltaTime;
			if(dist <= lookRange){
				 if (shotTimer <= 0f){
       			    EnemyShoot();
            		shotTimer = 1.3f;
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
		GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
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
	public void EnemyShoot() { 
		CmdShoot(shotSpawn.position, shotSpawn.rotation, gameObject.tag + "_" + this.GetComponent<NetworkIdentity>().netId.ToString()); 
	}
	[Command]
	public void CmdShoot(Vector3 _shotPos, Quaternion _shotRot, string _Owner)
    {
        GameObject newBullet = Instantiate(shot, _shotPos, _shotRot) as GameObject;
        //static values for testing on multiple instances on one computer
        newBullet.GetComponent<Bullet>().InitBullet(
                                                   //damage,
                                                   35,
                                                   //shotSpeed,
                                                   -10f,
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
