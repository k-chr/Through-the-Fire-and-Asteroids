using UnityEngine;
using UnityEngine.Networking;

public class PlayerShooting : NetworkBehaviour {

    public ShipStats shipStats;
    public GameObject shot;
    public Transform shotSpawn;

    public float fireRate;
    public float shotTimer = 0f;

    private AudioPlayer Cygan;

    public void Start()
    {
        shipStats = gameObject.GetComponent<ShipStats>();

        shot.GetComponent<Bullet>().InitBullet(shipStats._shipStats.damage, shipStats._shipStats.shotSpeed, this.GetComponent<NetworkIdentity>().netId);

        fireRate = 4f;
        Cygan = this.GetComponent<AudioPlayer>();
    }

    public void Update()
    {
        if (shotTimer >= 0f) shotTimer -= Time.deltaTime;
        if (Input.GetButton("Fire1") && shotTimer <= 0f)
        {
            CmdShoot();
            shotTimer = 1f / fireRate;

            Cygan.CmdPlayOnServer(0);
        }
    }

    [Command]
    public void CmdShoot()
    {
        NetworkServer.Spawn(Instantiate(shot, shotSpawn.position, shotSpawn.rotation) as GameObject);
    }
}
