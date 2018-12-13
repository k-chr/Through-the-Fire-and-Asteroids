#define LASER_DEBUG
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour {


	public Transform shootPoint1 = null;
	public Transform shootPoint2 = null;
	public Transform laserShootPoint = null;

	public GameObject shotPrefab = null;

	public float moveForce = 0f;
	public float angle = 15f;
	public float basicMoveSpeed = 0f;
	public float rotationSpeed = 0f;
	public float shotDelay = 0f;
	public float laserCharge = 1.5f;
	public float shotTimer = 0f;
	public float laserReserve = 100f;

	public bool turbo = false;
	public bool tiltHorizontal = false;
	public bool tiltVertical = false;

	public Camera cam = null;

	public Quaternion rotationA;
	public Quaternion rotationB;
	public Quaternion rotationX;
	public Quaternion rotationY;
	public Quaternion basicRotation;
	
	public ShipStats stats = null;

	public Camera minimapCamera = null;

	private float __x = 100f;

	void Awake () {
		moveForce = basicMoveSpeed;
		//cam.transform.position = this.transform.position;
		minimapCamera.transform.position = this.transform.position + new Vector3(0f, 30f, 0f);
		minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
		// basicRotation = cam.transform.rotation;
		stats = GetComponent<ShipStats>();
		//StartCoroutine("CheckForEnemies");
	}
	
	void Update () {

		// rotationA = Quaternion.Euler(cam.transform.eulerAngles.x, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z + angle);
		// rotationB = Quaternion.Euler(cam.transform.eulerAngles.x, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z - angle);
		// rotationX = Quaternion.Euler(cam.transform.eulerAngles.x - angle, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
		// rotationY = Quaternion.Euler(cam.transform.eulerAngles.x + angle, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);

		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Y)) {
			Debug.Log("in debugowy klawisz hyhy");
			++stats.upgradeInfo;
			Debug.Log(stats.upgradeInfo.ToString());
		}
		#endif
		if (Input.GetKey(KeyCode.Space)) {
			turbo = true;
		} else turbo = false;

		#if (LASER_DEBUG)
		if (Input.GetKeyDown(KeyCode.L)) {
			__x -= 100f * Time.deltaTime;
			Debug.Log(__x.ToString());
			__x = Mathf.Clamp(__x, 0f, 100f);
		}
		#endif

		if (turbo) {
			moveForce = stats._shipStats.turboMultiplier * basicMoveSpeed;
		} else {
			moveForce = basicMoveSpeed;
		}

		transform.Translate(transform.forward * moveForce);

		// if (Input.GetKey(KeyCode.W) && !tiltVertical) {
		// 	//cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, rotationX, 0.1f);
		// 	transform.Translate(transform.forward);
		// 	tiltVertical = true;
		// }

		// if (Input.GetKey(KeyCode.S) && !tiltVertical) {
		// 	//cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, rotationY, 0.1f);	
		// 	tiltVertical = true;
		// }

		// if (Input.GetKey(KeyCode.D) && !tiltHorizontal) {
		// 	cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, rotationB, 0.1f);
		// 	tiltHorizontal = true;
		// }

		// if (Input.GetKey(KeyCode.A) && !tiltHorizontal) {
		// 	cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, rotationA, 0.1f);
		// 	tiltHorizontal = true;
		// }

		// tiltHorizontal = false;
		// tiltVertical = false;

		// if (!tiltHorizontal) {
		// 	cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, basicRotation, 0.1f);
		// }

		// if (!tiltVertical) {
		// 	cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, basicRotation, 0.1f);
		// }

		// cam.transform.position = this.transform.position;

		// if (Input.GetMouseButton(0)) {
		// 	Shoot();
		// }

		// if (Input.GetKey(KeyCode.F)) {
		// 	Debug.Log("HERE");
		// 	if (laserCharge > 0f) laserCharge -= Time.deltaTime;
		// 	else {
		// 		if (laserReserve > 0f) {
		// 			Laser();
		// 			float temp = laserReserve;
		// 			laserReserve -= Mathf.Lerp(100f, 0f, 0.15f);
		// 			Debug.Log("Before: " + temp.ToString() + ", After: " + laserReserve.ToString());
		// 		}
		// 	}
		// } else {
		// 	laserCharge = 1.5f;
		// 	laserReserve = 100f;
		// }

		if (shotTimer > 0f) {
			shotTimer -= Time.deltaTime;
		} 
	}

	void Shoot() {
		if (shotTimer > 0f) return;
		GameObject[] shots = new GameObject[2];
		shots[0] = Instantiate(shotPrefab, shootPoint1.position, Quaternion.identity) as GameObject;
		shots[1] = Instantiate(shotPrefab, shootPoint2.position, Quaternion.identity) as GameObject;
		foreach (GameObject shot in shots) {
			shot.GetComponent<Bullet>().InitBullet(
				this.transform.forward,
				stats._shipStats.damage,
				stats._shipStats.shotSpeed
			);
		}
		shotTimer = stats._shipStats.fireRate;
	}

	void Laser() {
		if (laserCharge > 0f) return;
		RaycastHit трахнуть_ебать;
		float dmg = stats._shipStats.damage * Time.deltaTime;
		if (Physics.Raycast(laserShootPoint.position, transform.forward, out трахнуть_ебать, 150f)) {
			if (трахнуть_ебать.transform != null) {
				if (трахнуть_ебать.transform.gameObject.CompareTag("Player") || трахнуть_ебать.transform.gameObject.CompareTag("Enemy")) {

				}
			}
		}
	}
}
