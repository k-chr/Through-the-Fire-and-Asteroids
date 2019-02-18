using UnityEngine;
using UnityEngine.Networking;
using System.Linq;



//random area
public enum Area2{
    Box,
    Sphere, 
}

public class EnemyRandomSpawner : NetworkBehaviour
{
    //objects to spawn
    public float radius = 0f;
    public Transform[] prefabs = {null,
                                  null};
    //choose Area                                  
    public Area2 spawnArea = Area2.Sphere;
    //multiply area, change its shape
    public Vector3 shapeModifiers = Vector3.one;
    //how many enemies do you want?
   	public int maxUnicornCount = 1;
	public int maxEnemiesCount = 20;
    //max distance from the center of spawner 
    public float spawnRange = 20.0f;
    //random scale of would-spawned objects

    // Use this for initialization
    void Start()
    {
        if(prefabs.ToList().Any(item => item != null)){
            for (int i = 0; i < maxEnemiesCount; i++)
               SpawnEnemy();
        }        
    }
    void Update(){
        if(transform.childCount < maxEnemiesCount){
            SpawnEnemy();
        }
    }
    private void SpawnEnemy()
    {
        Vector3 spawnPos = Vector3.zero;
        
        // Create random position based on choosen shape and range.
        if (spawnArea == Area2.Box)
        {
            spawnPos.x = Random.Range(-spawnRange, spawnRange) * shapeModifiers.x;
            spawnPos.y = Random.Range(-spawnRange, spawnRange) * shapeModifiers.y;
            spawnPos.z = Random.Range(-spawnRange, spawnRange) * shapeModifiers.z;
        }
        else if (spawnArea == Area2.Sphere)
        {
            spawnPos = Random.insideUnitSphere * spawnRange;
            spawnPos.x *= shapeModifiers.x;
            spawnPos.y *= shapeModifiers.y;
            spawnPos.z *= shapeModifiers.z;
        }
        //randomly choose one of the type of asteroid to spawn 
        int i = Random.Range(0, 1000);
       	if(i == 666){
       		i = 1;
       	} 
       	else{
       		i = 0;
       	}
        // Offset position to match position of the parent gameobject.
        if(ifAny(spawnPos))
                spawnPos += transform.position + Vector3.one*radius;
        else
            spawnPos -= transform.position +Vector3.one*radius;

        // Apply a random rotation if necessary.
        Quaternion spawnRot = Random.rotation;

        // Create the object and set the parent to this gameobject for scene organization.
        Transform t = Instantiate(prefabs[i], spawnPos, spawnRot) as Transform;

        t.SetParent(transform);

        if(NetworkServer.active){
            NetworkServer.Spawn(t.gameObject);
        }
    }
    bool ifAny(Vector3 vec){
        return vec[0] > 0 || vec[1] > 0 || vec[2] > 0;
    }
}
