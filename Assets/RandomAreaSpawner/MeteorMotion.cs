using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class MeteorMotion : NetworkBehaviour
{
	public Transform expObj = null;
	public float velocity;
	public float angularVelocity;
	public Rigidbody myRigidbody;
	public float health;
	[SyncVar]
	public float currHealth;
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myRigidbody.AddRelativeForce(Random.insideUnitSphere * velocity, ForceMode.Force);
        myRigidbody.AddRelativeTorque(Random.insideUnitSphere * angularVelocity * Mathf.Deg2Rad, ForceMode.Force);
    }

    // Update is called once per frame
    void Update()
    {
      // transform.position += transform.forward * velocity * Time.deltaTime;
       if(ifAny(transform.position, transform.parent.position)){
       		DestroyAsteroid();
       }
    }
    bool ifAny(Vector3 vec, Vector3 src){
       return vec[0] > src[0] + 2000f || vec[1] > src[1] + 2000f || vec[2] > src[2] + 2000f || vec[0] + 2000f < src[0] || vec[1] + 2000f < src[1] || vec[2] + 2000f < src[2];
    }
    public void DestroyAsteroid(){
       Explosion();
   	   NetworkServer.Destroy(this.gameObject);
    }
    public void Explosion(){
    	for(uint i = 0; i < 40; ++i){
    		int choose = Random.Range(0,1);
    		float x = choose == 1 ? Random.Range(-1f, -0.01f) : Random.Range(0.01f, 1f);
    		choose = Random.Range(0,1);
    		float y = choose == 1 ? Random.Range(-1f, -0.01f) : Random.Range(0.01f, 1f);
    		choose = Random.Range(0,1);
    		float z = choose == 1 ? Random.Range(-1f, -0.01f) : Random.Range(0.01f, 1f);
    		Vector3 vec = new Vector3(x,y,z).normalized * 60f;
    		Quaternion rot = Quaternion.LookRotation(vec);
    		Transform t = Instantiate(expObj, vec + transform.position, rot) as Transform;
    		t.GetComponent<Rigidbody>().mass = myRigidbody.mass/20f;
    		t.GetComponent<Rigidbody>().AddRelativeForce(Vector3.one * 2f, ForceMode.VelocityChange);
    		if(NetworkServer.active){
           		NetworkServer.Spawn(t.gameObject);
      		}
    	}
    }
    public void TakeDamage(float amount){
        currHealth -= amount;
        Debug.Log("Asteroid hit");
        if (currHealth <= 0f){
  
       		DestroyAsteroid();
       	}
    }
}
