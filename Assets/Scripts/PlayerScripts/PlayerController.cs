using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Rigidbody myRigidbody;
    public float speed = 0;

    private GameObject[] SpawnPoints = null;
    public Camera playerCamera = null;
    public Vector3 playerCameraPosition = new Vector3(0f, 0.8f, -0.6f);

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        speed = gameObject.GetComponent<ShipStats>()._shipStats.speed;
        SpawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        playerCamera = GetComponentInChildren<Camera>();
        playerCameraPosition = new Vector3(0f, 0.8f, -0.6f);
    }

    void FixedUpdate()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
        playerCamera.transform.localPosition = playerCameraPosition;
        
        transform.Rotate(Input.GetAxis("Vertical"), 0f, -Input.GetAxis("Horizontal"));
    }

    public void Died()
    {
        this.enabled = false;
        GetComponent<PlayerNetworkActions>().enabled = false;
        Invoke("Respawn", 2f);
    }

    public void Respawn()
    {
        transform.position = SpawnPoints[Random.Range(0, SpawnPoints.Length)].transform.position;
        this.enabled = true;
        this.GetComponent<PlayerNetworkActions>().enabled = true;
        this.GetComponent<PlayerNetworkActions>().curHealth = GetComponent<ShipStats>()._shipStats.maxHealth;
    }
}
