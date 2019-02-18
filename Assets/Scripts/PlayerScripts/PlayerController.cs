using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
     /*
        In my opinion, we have to determine, in rigidbody component, a physical mass of ship, drag of ship and angular drag of ship. 
        Provisionally I set up this values: mass = 200, drag = 2, angular drag = 5. It works :)
    */
    //input coefficients    
    [Range(-1, 1)]
    public float movX;
    [Range(-1, 1)]
    public float movZ;
    [Range(-1, 1)]
    public float rotZ;
    [Range(-1, 1)]
    public float rotY;
    [Range(-1, 1)]
    public float rotX;

    public Vector3 linearForce = new Vector3(1.0f, 1.0f, 4.0f); //basic coefficients' vectors of movement and rotation
    public Vector3 angularForce = new Vector3(0.6f, 0.6f, 0.2f);

    private Vector3 determinedLinearForce = Vector3.zero; //indicates, how much transform.position coordinates depend on single move 
    private Vector3 determinedAngularForce = Vector3.zero; //indicates, how much transform.rotation coordinates depend on single move
    public float force = 100f; //force coefficient, that indicates the rate of movement change - but not to be confused with speed, that refers only to linear movement 

    //camera emergency patch :V
    public Vector3 startOffset;
    public Transform target;
    public float rotateSpeed = 3f;

    public bool Zombie = false;
    public Rigidbody myRigidbody;
    public float speed = 0;
    public float shotTimer = 0f;

    private GameObject[] SpawnPoints = null;
    public Camera[] playerCameras = null;
    public Vector3 playerCameraPosition = new Vector3(0f, 0.8f, -0.6f);

    private void Start()
    {
        shotTimer = -1f;

        myRigidbody = GetComponent<Rigidbody>();
        myRigidbody.drag = 39f;
        myRigidbody.mass = 50f;
        myRigidbody.angularDrag = 37f;

        //speed = gameObject.GetComponent<ShipStats>()._shipStats.speed;
        speed = 2f;
        SpawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        playerCameras = GetComponentsInChildren<Camera>();
        target = playerCameras[0].transform.parent; //get parent of main camera of player

        playerCameraPosition = new Vector3(0f, 0.8f, -0.6f);
        playerCameras[0].transform.SetParent(null); //ingenious way to make camera follow you with some angular delay :V
        startOffset = new Vector3(0f, 0.7f, -1.7f); // I've set a initial constant position :V
        movX = 0f;
        movZ = 0.5f;
    }

    void FixedUpdate()
    {
        if (shotTimer >= 0f) shotTimer -= Time.deltaTime;
        if (Zombie == true) return;

        rotX = Input.GetAxis("Vertical");
        rotY = Input.GetAxis("Horizontal");
        rotZ = -Input.GetAxis("Horizontal") * 0.5f;

        UpdateForces(new Vector3(movX, 0f, movZ), new Vector3(rotX, rotY, rotZ));
        if (myRigidbody != null){
            myRigidbody.AddRelativeForce(determinedLinearForce * force * speed * speed, ForceMode.Force); //apply linear force to ship
            myRigidbody.AddRelativeTorque(determinedAngularForce * force, ForceMode.Force); //apply angular force to ship
        }
       // playerCameras[0].transform.localPosition = playerCameraPosition;
        if(target != null){
            playerCameras[0].transform.position = target.TransformPoint(startOffset);
            playerCameras[0].transform.rotation = Quaternion.Slerp(playerCameras[0].transform.rotation, target.rotation, 900f * Time.deltaTime);
        }
        playerCameras[1].transform.rotation = Quaternion.Euler(90f, transform.rotation.y, 0f);
        playerCameras[1].transform.position = new Vector3(0f, 50f, 0f) + transform.position;
        //Debug.Log("velocity vector of object: x = " + myRigidbody.velocity.x.ToString() + ", y = " + myRigidbody.velocity.y.ToString() + ", z =  " + myRigidbody.velocity.z.ToString());
        if (Input.GetButton("Fire1") && shotTimer <= 0f)
        {
            gameObject.GetComponent<PlayerNetworkActions>().ShootKurla();
            //do testowania shotTimer musi byc na stale ustawiony, tak samo jak speed
            shotTimer = 0.6f;
                //gameObject.GetComponent<PlayerNetworkActions>().shipStats._shipStats.fireRate;
        }

    }
    public void UpdateForces(Vector3 linear, Vector3 angular){ //multiply input changes by basic force
        determinedLinearForce.x = linear.x * linearForce.x;
        determinedLinearForce.y = linear.y * linearForce.y;
        determinedLinearForce.z = linear.z * linearForce.z;
        determinedAngularForce.x = angular.x * angularForce.x;
        determinedAngularForce.y = angular.y * angularForce.y;
        determinedAngularForce.z = angular.z * angularForce.z;
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
