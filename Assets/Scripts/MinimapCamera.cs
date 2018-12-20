using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour {

	public Transform player = null;

	// Use this for initialization
	void Start () {
		//player = this.transform.parent;
		this.transform.position = new Vector3(player.position.x, player.position.y + 15f, player.position.z);
		//this.transform.rotation = Quaternion.Euler(-90f, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = new Vector3(player.position.x,this.transform.position.y, player.position.z);
		
	}
}
