using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public bool Zombie = false;
    public Rigidbody myRigidbody;
    public float speed = 0;
    public float shotTimer = 0f;

    private GameObject[] SpawnPoints = null;
    public Camera[] playerCameras = null;
    public Vector3 playerCameraPosition = new Vector3(0f, 0.8f, -0.6f);

    private void Awake()
    {
        shotTimer = -1f;
        myRigidbody = GetComponent<Rigidbody>();
        speed = gameObject.GetComponent<ShipStats>()._shipStats.speed;
        SpawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        playerCameras = GetComponentsInChildren<Camera>();
        playerCameraPosition = new Vector3(0f, 0.8f, -0.6f);
    }

    void FixedUpdate()
    {
        if (shotTimer >= 0f) shotTimer -= Time.deltaTime;
        if (Zombie == true) return;
        
        playerCameras[0].transform.localPosition = playerCameraPosition;
        playerCameras[1].transform.rotation = Quaternion.Euler(90f, transform.rotation.y, 0f);
        playerCameras[1].transform.position = new Vector3(0f, 50f, 0f) + transform.position;
        transform.Rotate(Input.GetAxis("Vertical"), 0f, -Input.GetAxis("Horizontal"));
        if (Input.GetButton("Fire1") && shotTimer <= 0f)
        {
            gameObject.GetComponent<PlayerNetworkActions>().ShootKurla();
            //do testowania shotTimer musi byc na stale ustawiony, tak samo jak speed
            shotTimer = 0.3f;
                //gameObject.GetComponent<PlayerNetworkActions>().shipStats._shipStats.fireRate;
        }
    }

    public void Died()
    {
        Zombie = true;
        Invoke("Respawn", 2f);
    }

    public void Respawn()
    {
        int SpawnIndex = Random.Range(0, SpawnPoints.Length);
        transform.position = SpawnPoints[SpawnIndex].transform.position;
        transform.rotation = SpawnPoints[SpawnIndex].transform.rotation;
        Zombie = false;
        this.GetComponent<PlayerNetworkActions>().curHealth = GetComponent<ShipStats>()._shipStats.maxHealth;
    }
}
