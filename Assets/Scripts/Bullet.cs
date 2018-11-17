using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float damage = 0f;
	public float speed = 0f;
	public float decayTime = 0f;
	public Vector3 direction = Vector3.zero;
	
	void Update () {
		transform.Translate(direction * speed);
	}

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("Player")) {
			//other.gameObject.GetComponent<>
		}

		if (other.gameObject.CompareTag("Enemy")) {

		}
		Destroy(this.gameObject);
	}

	public void InitBullet(Vector3 flightDirection, float bulletDamage, float bulletSpeed) {
		Destroy(this.gameObject, decayTime);
		direction = flightDirection;
		this.damage = bulletDamage;
		this.speed = bulletSpeed;
		this.decayTime = 4f;
	}
}
