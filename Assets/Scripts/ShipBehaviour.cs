using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour {

	public Transform shootPoint1 = null;
	public Transform shootPoint2 = null;

	public float moveForce = 0f;
	public float angle = 15f;
	public float basicMoveSpeed = 0f;
	public float rotationSpeed = 0f;
	public float shotDelay = 0f;
	public float shotTimer = 0f;

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

	void Awake () {
        cam.transform.position = this.transform.position + new Vector3(0f, 1f, 3f);
        cam.transform.rotation = this.transform.rotation;

		basicRotation = cam.transform.rotation;
		stats = GetComponent<ShipStats>();
	}
	
	void Update () {

		rotationA = Quaternion.Euler(cam.transform.eulerAngles.x, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z + angle);
		rotationB = Quaternion.Euler(cam.transform.eulerAngles.x, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z - angle);
		rotationX = Quaternion.Euler(cam.transform.eulerAngles.x - angle, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
		rotationY = Quaternion.Euler(cam.transform.eulerAngles.x + angle, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);

		if (Input.GetKey(KeyCode.Space)) {
			turbo = true;
		} else turbo = false;

		if (turbo) {
			moveForce = 1.5f * basicMoveSpeed;
		} else {
			moveForce = basicMoveSpeed;
		}

		transform.Translate(transform.forward * moveForce);

		if (Input.GetKey(KeyCode.W) && !tiltVertical) {
			cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, rotationX, 0.1f);
			tiltVertical = true;
		}

		if (Input.GetKey(KeyCode.S) && !tiltVertical) {
			cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, rotationY, 0.1f);	
			tiltVertical = true;
		}

		if (Input.GetKey(KeyCode.D) && !tiltHorizontal) {
			cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, rotationB, 0.1f);
			tiltHorizontal = true;
		}

		if (Input.GetKey(KeyCode.A) && !tiltHorizontal) {
			cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, rotationA, 0.1f);
			tiltHorizontal = true;
		}

		tiltHorizontal = false;
		tiltVertical = false;

		if (!tiltHorizontal) {
			cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, basicRotation, 0.1f);
		}

		if (!tiltVertical) {
			cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, basicRotation, 0.1f);
		}

        cam.transform.position = this.transform.position + new Vector3(0f, 1f, 3f);
    }

    public void OnDestroy()
    {
        
    }
}
