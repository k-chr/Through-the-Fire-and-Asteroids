using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
//random area
public enum Area
{
    Box,
    Sphere, 
}

public class GameAreaSpawner : NetworkBehaviour
{
    //specify, if objects should not collide with center of your area
    public GameObject Star = null;
    //objects to spawn
    public float radius = 0f;
    public Transform[] prefabs = {null,
                                  null,
                                  null,
                                  null,
                                  null};
    //choose Area                                  
    public Area spawnArea = Area.Sphere;
    //multiply area, change its shape
    public Vector3 shapeModifiers = Vector3.one;
    //how many asteroids do you want?
    public int objectsCount = 1000;
    //max distance from the center of spawner 
    public float spawnRange = 2000.0f;
    //random scale of would-spawned objects
    public Vector2 scaleRange = new Vector2(.001f, 0.005f);

    //rigidbody stuff
    public float velocity = 0.0f;
    public float angularVelocity = 0.0f;
    //public int actualObjectsCount = 0;
    // Use this for initialization
    void Start()
    {
        if(Star != null) radius = 10f;
        if(prefabs.ToList().Any(item => item != null)){
            for (int i = 0; i < objectsCount; i++)
               SpawnAsteroid();
        }        
    }
    void Update(){
        if(Star.transform.childCount < objectsCount){
            SpawnAsteroid();
        }
    }
    private void SpawnAsteroid()
    {
        Vector3 spawnPos = Vector3.zero;
        int choose = Random.Range(0,1);
        if(choose == 1)
            velocity = Random.Range(-50f, -10f);
        else
            velocity = Random.Range(10f, 50f);
        choose = Random.Range(0,1);
        if(choose == 1)
            angularVelocity = Random.Range(50f, 10f);
        else
            angularVelocity = Random.Range(-10f, -50f);
        // Create random position based on choosen shape and range.
        if (spawnArea == Area.Box)
        {
            spawnPos.x = Random.Range(-spawnRange, spawnRange) * shapeModifiers.x;
            spawnPos.y = Random.Range(-spawnRange, spawnRange) * shapeModifiers.y;
            spawnPos.z = Random.Range(-spawnRange, spawnRange) * shapeModifiers.z;
        }
        else if (spawnArea == Area.Sphere)
        {
            spawnPos = Random.insideUnitSphere * spawnRange;
            spawnPos.x *= shapeModifiers.x;
            spawnPos.y *= shapeModifiers.y;
            spawnPos.z *= shapeModifiers.z;
        }
        //randomly choose one of the type of asteroid to spawn 
        //int i = Random.Range(0,4);
        int i = 0;
        while(prefabs[i] == null){
            i = Random.Range(0,4);
        }
        // Offset position to match position of the parent gameobject.
        if(ifAny(spawnPos))
                spawnPos += Star.transform.position + Vector3.one*radius;
        else
            spawnPos -= Star.transform.position +Vector3.one*radius;

        // Apply a random rotation if necessary.
        Quaternion spawnRot = Random.rotation;

        // Create the object and set the parent to this gameobject for scene organization.
        Transform t = Instantiate(prefabs[i], spawnPos, spawnRot) as Transform;

        t.SetParent(Star.transform);

        // Apply scaling.
        float scale = Random.Range(scaleRange.x, scaleRange.y);
        t.localScale = Vector3.one * scale;

        // Apply rigidbody values.
        Rigidbody r = t.GetComponent<Rigidbody>();
        float hp =  (float)(int)(Random.Range(50f,300f));
        t.GetComponent<MeteorMotion>().health = hp;
        t.GetComponent<MeteorMotion>().currHealth = hp;
        t.GetComponent<MeteorMotion>().velocity = velocity;
        t.GetComponent<MeteorMotion>().angularVelocity = angularVelocity;
        if (r)
        {
            r.drag = 2f;
            r.mass *= scale * scale * scale * 100000000f;

            r.AddRelativeForce(Random.insideUnitSphere * velocity, ForceMode.Force);
            r.AddRelativeTorque(Random.insideUnitSphere * angularVelocity * Mathf.Deg2Rad, ForceMode.Force);
        }
        if(NetworkServer.active){
            NetworkServer.Spawn(t.gameObject);
    //        actualObjectsCount++;
        }
    }
    bool ifAny(Vector3 vec){
        return vec[0] > 0 || vec[1] > 0 || vec[2] > 0;
    }
}
