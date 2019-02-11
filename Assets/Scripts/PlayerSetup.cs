using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PlayerSetup : NetworkBehaviour {

    //add components of player prefab to this in inspector
    
    [SerializeField]
    List<Behaviour> components = new List<Behaviour>();

    Camera playercam;
    Camera sceneCamera;

    //Okay here is the thing, we can do what we want to do in few ways.
    //First of all is this one, disabling all parts of the game that we don't want to be controlled by the other players.
    //However we can also just use   if(!isLocalPlayer) Destroy(this);  should work the same.
    //Use with caution, may destroy something important.
    public void Start()
    {
        playercam = GetComponentInChildren<Camera>();
        components.Add(playercam.gameObject.GetComponent<AudioListener>());
        if(!isLocalPlayer)
        {
            DisableComponents();
        }
        else
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null)
                sceneCamera.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        sceneCamera = Camera.main;
        if(sceneCamera != null)
            sceneCamera.gameObject.SetActive(true);
    }

    void DisableComponents()
    {
        for (int i = 0; i < components.Count; ++i)
        {
            components[i].enabled = false;
        }
    }
}
