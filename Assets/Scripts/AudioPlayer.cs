using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : NetworkBehaviour {

    public AudioSource audioSource;

    public AudioClip[] clips;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(int id)
    {
        if(id > 0 && id < clips.Length)
        {
            CmdPlayOnServer(id);
        }
    }

    public void PlayLocaly(int id)
    {
        audioSource.PlayOneShot(clips[id], 1);
    }

    [Command]
    public void CmdPlayOnServer(int id)
    {
        RpcSendSoundToClients(id);
    }

    [ClientRpc]
    public void RpcSendSoundToClients(int id)
    {
        audioSource.PlayOneShot(clips[id], 1);
    }

}
