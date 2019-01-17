using UnityEngine;

public class WhatIsLife : MonoBehaviour {

    float lifetime = 0;
	void Start () {
        lifetime = 2f;
	}
	
	void Update () {
        if ((lifetime -= Time.deltaTime) <= 0) Destroy(this.gameObject);
	}
}
