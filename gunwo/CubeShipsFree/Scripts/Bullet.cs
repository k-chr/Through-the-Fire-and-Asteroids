using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


namespace CubeSpaceFree
{
    // A stub class that we use to check if an object is a Bullet (we could have used a Tag instead).
    public class Bullet : NetworkBehaviour
    {
        public void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
    }
}
