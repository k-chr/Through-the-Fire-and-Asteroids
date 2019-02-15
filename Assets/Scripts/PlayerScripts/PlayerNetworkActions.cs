using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ShipStats))]
public class PlayerNetworkActions : NetworkBehaviour
{
    public ShipStats shipStats;
    [SyncVar]
    public float curHealth;
    [SyncVar]
    private bool gameEnded = false;
    [SerializeField]
    private int gameTimer = 0;

    //UI values
    public Text PlayerUITimer;
    public GameObject PlayerUIElement;
    public GameObject PlayerListContainer;
    public List<GameObject> PlayerList = new List<GameObject>();
    [SerializeField]
    private MenuUI menu = null;

    public GameObject explosion;
    public string[] keys = null;

    ParticleSystem[] particleEffects;
    //particle List:
    //0: thruster
    //1: star particle

    [SerializeField]
    List<Behaviour> components = new List<Behaviour>();

    Camera sceneCamera;

    //Audio
    public AudioSource audioSource;
    public AudioClip[] clips;

    //Shooting
    public GameObject shot;
    public Transform shotSpawn;
    
    public int difference = 1;

    public void Start()
    {
        SetScoreboard(true);
        PlayerListContainer.SetActive(false);

        gameTimer = 60;
        InvokeRepeating("UpdateUITimer", 0f, 1f);

        audioSource = GetComponent<AudioSource>();
        shipStats = gameObject.GetComponent<ShipStats>();
        curHealth = shipStats._shipStats.maxHealth;

        if (!isLocalPlayer)
        {
            //setup
            components.Add(GetComponentInChildren<Camera>());
            components.Add(GetComponent<ShipBehaviour>());
            components.Add(GetComponent<ShipStats>());
            components.Add(GetComponentInChildren<AudioListener>());
            GetComponent<PlayerController>().enabled = false;
            DisableComponents();
            particleEffects = GetComponentsInChildren<ParticleSystem>();

            //turning off star particle
            particleEffects[1].Stop();
            particleEffects[1].Clear();

            return;
        }
        else
        {
            SetScoreboard(true);
            PlayerListContainer.SetActive(false);

            menu = FindObjectOfType<MenuUI>();

            menu.ingame = true;
            menu.title.enabled = false;
            menu.menuStates[2].GetComponentInChildren<InputField>().interactable = false;
            menu.menuStates[2].GetComponentInChildren<Dropdown>().interactable = false;
            menu.menuStates[0].SetActive(false);
            menu.menuStates[1].SetActive(false);
            menu.menuStates[2].SetActive(false);

            audioSource = GetComponent<AudioSource>();
            shipStats = gameObject.GetComponent<ShipStats>();
            curHealth = shipStats._shipStats.maxHealth;

            sceneCamera = Camera.main;
            if (sceneCamera != null)
                sceneCamera.gameObject.SetActive(false);
            gameTimer = 60;
            InvokeRepeating("UpdateUITimer", 0f, 1f);
        }
    }

    public void Update()
    {
        if (gameTimer <= 0f && isServer) { gameEnded = true; }
        if (!isLocalPlayer) return;

        //respawn key
        if (Input.GetKeyDown(KeyCode.R))
        {
            CmdPlayerDied(transform.name, transform.name);
            gameObject.GetComponent<PlayerController>().Died();
        }

        //tablist
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            SetScoreboard(keys.Length != CustomGameManager.GetNumberOfPlayers());
            PlayerListContainer.SetActive(true);
        }

        if(Input.GetKeyUp(KeyCode.Tab)) PlayerListContainer.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu.menuStates[2].activeSelf || menu.menuStates[3].activeSelf)
            {
                menu.menuStates[2].SetActive(false);
                menu.menuStates[3].SetActive(false);
                gameObject.GetComponent<PlayerController>().pause = false;
            }
            else
            {
                menu.menuStates[3].SetActive(true);
                gameObject.GetComponent<PlayerController>().pause = true;
            }
        }

        if (gameEnded) { CancelInvoke(); PlayerUITimer.text = "GameOver"; }
        else PlayerUITimer.text = (Mathf.RoundToInt(gameTimer)).ToString();
    }

    //Setup methods
    void OnDisable()
    {
        sceneCamera = Camera.main;
        if(sceneCamera != null)
            sceneCamera.gameObject.SetActive(true);
        menu.menuStates[1].SetActive(true);
        menu.menuStates[0].SetActive(false);
        menu.menuStates[2].SetActive(false);
        menu.menuStates[3].SetActive(false);
        menu.title.enabled = true;
    }

    void DisableComponents()
    {
        for (int i = 0; i < components.Count; ++i)
        {
            components[i].enabled = false;
        }
    }

    void SetScoreboard(bool reset)
    {
        if (!isLocalPlayer) return;
        if(reset)
        {
            foreach(GameObject PlayerElement in PlayerList)
            {
                Destroy(PlayerElement);
            }
            PlayerList.Clear();

            keys = CustomGameManager.ReturnKeys();

            foreach(string key in keys)
            {
                PlayerList.Add(Instantiate(PlayerUIElement, new Vector3(0, 0, 0), Quaternion.identity, PlayerListContainer.transform) as GameObject);
                foreach(Text el in PlayerList[PlayerList.Count-1].GetComponentsInChildren<Text>())
                {
                    switch (el.name)
                    {
                        case "PlayerName": el.text = PlayerList[PlayerList.Count-1].name = CustomGameManager.GetPlayerShip(key).name; break;
                        case "Kills": el.text = CustomGameManager.GetPlayerShip(key)._matchStats.kills.ToString(); break;
                        case "Deaths": el.text = CustomGameManager.GetPlayerShip(key)._matchStats.deaths.ToString(); break;
                        case "Points": el.text = CustomGameManager.GetPlayerShip(key)._matchStats.points.ToString(); break;
                    }
                }
            }
        }
        else
        {
            for(int i = 0; i < PlayerList.Count; ++i)
            {
                foreach(Text el in PlayerList[i].GetComponentsInChildren<Text>())
                {
                    switch (el.name)
                    {
                        case "PlayerName": el.text = CustomGameManager.GetPlayerShip(keys[i]).name; break;
                        case "Kills": el.text = CustomGameManager.GetPlayerShip(keys[i])._matchStats.kills.ToString(); break;
                        case "Deaths": el.text = CustomGameManager.GetPlayerShip(keys[i])._matchStats.deaths.ToString(); break;
                        case "Points": el.text = CustomGameManager.GetPlayerShip(keys[i])._matchStats.points.ToString(); break;
                    }
                }
            }
        }
    }

    private void UpdateUITimer()
    {
        gameTimer -= difference;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        ShipStats _playerShip = GetComponent<ShipStats>();
        _playerShip.transform.name = "Player_" + _netID;
        CustomGameManager.RegisterPlayer(_netID, _playerShip);

        Debug.Log("Amount of players in GameManager: " + CustomGameManager.GetNumberOfPlayers().ToString() + 
                  " and number of connections/players/lobbyPlayers on the Server: " + FindObjectOfType<NetworkLobbyManager>().numPlayers.ToString());
    }

    //Sound methods
    public void PlaySound(int id)
    {
        if (id > 0 && id < clips.Length)
        {
            CmdPlayOnServer(id);
        }
    }

    public void PlayLocaly(int id)
    {
        if (id > 0 && id < clips.Length)
            audioSource.PlayOneShot(clips[id], 1);
    }

    [Command]
    public void CmdPlayOnServer(int id)
    {
        if (id > 0 && id < clips.Length)
            RpcSendSoundToClients(id);
    }

    [ClientRpc]
    public void RpcSendSoundToClients(int id)
    {
        if (id > 0 && id < clips.Length)
            audioSource.PlayOneShot(clips[id], 1);
    }

    //Shooting mehtods
    public void ShootKurla() { CmdShoot(shotSpawn.position, shotSpawn.rotation, this.GetComponent<NetworkIdentity>().netId.ToString()); }
    [Command]
    public void CmdShoot(Vector3 _shotPos, Quaternion _shotRot, string _Owner)
    {
        GameObject newBullet = Instantiate(shot, _shotPos, _shotRot) as GameObject;
        //static values for testing on multiple instances on one computer
        newBullet.GetComponent<Bullet>().InitBullet(
                                                   //shipStats._shipStats.damage,
                                                   25,
                                                   //shipStats._shipStats.shotSpeed,
                                                   10f,
                                                    _Owner);
        NetworkServer.Spawn(newBullet);
        RpcSendSoundToClients(0);
    }

    [Command]
    public void CmdExplode(Vector3 _point)
    {
        GameObject expd = Instantiate(explosion, _point, new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
        NetworkServer.Spawn(expd);
    }

    //health management
    public void TakeDamage(float amount, string _attacker)
    {
        if (GetComponent<PlayerController>().Zombie) return;
        string thisPlayerID = "Player_" + gameObject.GetComponent<NetworkIdentity>().netId.ToString();
        curHealth -= amount;

        if (curHealth <= 0f)
        {
            curHealth = 0f;
            CmdPlayerDied(_attacker, thisPlayerID);
            gameObject.GetComponent<PlayerController>().Died();
        }
}

    [Command]
    public void CmdPlayerDied(string whoKilled, string whoDied)
    {
        Debug.Log("CmdPlayerDied :: PlayerID(" + whoKilled + ") died, killed by PlayerID(" + whoDied + ")");
        RpcPlayerDied(whoKilled, whoDied);
    }

    [ClientRpc]
    public void RpcPlayerDied(string whoKilled, string whoDied)
    {
        Debug.Log("RpcPlayerDied :: PlayerID(" + whoKilled + ") died, killed by PlayerID(" + whoDied + ")");
        CustomGameManager.UpdatePlayer(whoKilled, "kills");
        CustomGameManager.UpdatePlayer(whoDied, "deaths");
    }
}
