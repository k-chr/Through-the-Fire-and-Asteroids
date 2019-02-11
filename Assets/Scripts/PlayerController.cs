using UnityEngine;
using System;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float tilt;              // tilt factor
    public Boundary boundary;       // movememnt boundary

    public Rigidbody myRigidbody;   // reference to rigitbody
    public float smoothing = 5;     // this value is used for smoothing movement
    private Vector3 smoothDirection;// used to smooth out mouse and touch control

    // Use this for initialization
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        transform.position += transform.forward * Time.deltaTime * gameObject.GetComponent<ShipStats>()._shipStats.speed;
        transform.Rotate(Input.GetAxis("Vertical"), 0f, -Input.GetAxis("Horizontal"));
    }

    public void Died()
    {
        this.enabled = false;
        GetComponent<ShipBehaviour>().enabled = false;
        GetComponent<PlayerShooting>().enabled = false;

    }
}
