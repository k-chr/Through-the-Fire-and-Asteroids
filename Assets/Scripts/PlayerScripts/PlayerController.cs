using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public bool Zombie = false;
    public Rigidbody myRigidbody;
    public float speed = 0;

    private GameObject[] SpawnPoints = null;
    public Camera[] playerCameras = null;
    public Vector3 playerCameraPosition = new Vector3(0f, 0.8f, -0.6f);

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        speed = gameObject.GetComponent<ShipStats>()._shipStats.speed;
        SpawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        playerCameras = GetComponentsInChildren<Camera>();
        playerCameraPosition = new Vector3(0f, 0.8f, -0.6f);
    }

    void FixedUpdate()
    {
        if (Zombie) return;
        transform.position += transform.forward * Time.deltaTime * speed;
        playerCameras[0].transform.localPosition = playerCameraPosition;
        playerCameras[1].transform.rotation = Quaternion.Euler(90f, transform.rotation.y, 0f);
        playerCameras[1].transform.position = new Vector3(0f, 50f, 0f) + transform.position;
        transform.Rotate(Input.GetAxis("Vertical"), 0f, -Input.GetAxis("Horizontal"));
    }

    public void Died()
    {
        //this.enabled = false;
        Zombie = true;
        Invoke("Respawn", 2f);
    }

    public void Respawn()
    {
        int SpawnIndex = Random.Range(0, SpawnPoints.Length);
        transform.position = SpawnPoints[SpawnIndex].transform.position;
        transform.rotation = SpawnPoints[SpawnIndex].transform.rotation;
        //this.enabled = true
        //this.GetComponent<PlayerNetworkActions>().enabled = true;
        Zombie = false;
        this.GetComponent<PlayerNetworkActions>().curHealth = GetComponent<ShipStats>()._shipStats.maxHealth;
    }
}
