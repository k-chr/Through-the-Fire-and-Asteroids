using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyAI : MonoBehaviour/*NetworkBehaviour*/ {

	public float directionChanging = 0f;
	public float minBoundary = 0f;
	public float maxBoundary = 0f;
	public float speed = 5f;
	public float lerpVal = 0.7f;

	public Quaternion targetRotation = default(Quaternion);
	public Object player = null;

	private bool directionChanged = false;
	

	void Start () {
		directionChanging = Random.Range(minBoundary, maxBoundary);
		int signFlag = Random.Range(0, 2);
		float val = Random.Range(45f, 70f);
		targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + (signFlag == 1 ? -val : val), transform.eulerAngles.z);	
	}
	
	void Update () {
		if (directionChanging > 0f) {
			directionChanging -= Time.deltaTime;
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpVal);
		} else {
			int signFlag = Random.Range(0, 2);
			float val = Random.Range(45f, 70f);
			targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + (signFlag == 1 ? -val : val), transform.eulerAngles.z);
			directionChanging = Random.Range(minBoundary, maxBoundary);
		}

		transform.Translate(transform.forward * speed * Time.deltaTime);
	}
}
