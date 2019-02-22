using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    private int roomSize = 4;
    private string roomName = "Default";
    private string playerName = "Player";
    [SerializeField]
    private Text searchingStat;

    public CustomNetworkManager networkManager;
    public GameObject[] menuStates;
    /*Menu states:
     * 0: Host/joined screen
     * 1: Search screen
     * 2: Settings window
     * 3: In game
     */
    public Text title;
    
    public bool ingame = false;
    public bool isReady = false;
    List<GameObject> roomList = new List<GameObject>();
    [SerializeField]
    private GameObject matchUIel;
    [SerializeField]
    private Transform content;

    List<GameObject> playerList = new List<GameObject>();
    [SerializeField]
    private GameObject playerUIEl;
    [SerializeField]
    private Transform playersListTrans;

    //for settings widnow
    private int prevState = 1;

    void Start()
    {
        networkManager = FindObjectOfType<CustomNetworkManager>();
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
        RefreshGamesList();
    }

    public void SetRoomName(string _name)
    {
        roomName = _name;
        Debug.Log("room name: " + roomName + ", given name:" + _name);
    }

    public void SetRoomSize(int _size)
    {
        roomSize = _size+4;
        Debug.Log("room size: " + roomSize.ToString() + ", given size:" + _size.ToString());
    }

    public void SetPlayerName(string _name)
    {
        playerName = _name;
    }
    public string GetPlayerName()
    {
        return playerName;
    }

    public void CreateRoom()
    {
        if (roomName != "" && roomName != null)
        {
            Debug.Log("Creating room with name: " + roomName + ", and size of: " + roomSize.ToString());
            if (networkManager.matchMaker == null) networkManager.StartMatchMaker();
            networkManager.matchMaker.CreateMatch(roomName, (uint)roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
            InvokeRepeating("OnPlayerList", 0f, 0.5f);
            foreach (Button butt in menuStates[0].GetComponentsInChildren<Button>())
            {
                if (butt.name == "ReadyButton") butt.interactable = true;
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void RefreshGamesList()
    {
        ClearMatchList();
        if (networkManager.matchMaker == null) networkManager.StartMatchMaker();
        networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
        searchingStat.text = "Loading...";
    }

    public void OnMatchList(bool succes, string extendedInfo, List<MatchInfoSnapshot> _matches)
    {
        if (!succes)
        {
            searchingStat.text = "Error!";
            return;
        }
        if(_matches.Count == 0)
        {
            searchingStat.text = "No match found";
            return;
        }
        searchingStat.text = "";

        foreach(MatchInfoSnapshot _match in _matches)
        {
            GameObject _matchListEl = Instantiate(matchUIel);
            _matchListEl.transform.SetParent(content, false);

            MatchListItem matchItem = _matchListEl.GetComponent<MatchListItem>();
            if(matchItem != null)
            {
                matchItem.Setup(_match, JoinRoom);
            }

            roomList.Add(_matchListEl);
        }
    }

    private void ClearMatchList()
    {
        for(int i=0; i < roomList.Count; ++i)
        {
            Destroy(roomList[i]);
        }
        roomList.Clear();
    }

    public void OnPlayerList()
    {
        ClearPlayersList();
        foreach(NetworkLobbyPlayer lobbyPlayer in networkManager.lobbySlots)
        {
            if (lobbyPlayer != null)
            {
                GameObject lobbyPlayerEl = Instantiate(playerUIEl, playersListTrans);
                if (lobbyPlayer.isLocalPlayer)
                {
                    if (isReady) lobbyPlayer.SendReadyToBeginMessage();
                    else lobbyPlayer.SendNotReadyToBeginMessage();
                }
                lobbyPlayerEl.GetComponent<PlayerLobbyUIEL>().Setup(lobbyPlayer.GetComponent<CustomNetworkLobbyPlayer>().name + "_" + lobbyPlayer.netId.ToString(), lobbyPlayer.readyToBegin);
                playerList.Add(lobbyPlayerEl);
            }
        }
    }

    private void ClearPlayersList()
    {
        for (int i = 0; i < playerList.Count; ++i)
        {
            Destroy(playerList[i]);
        }
        playerList.Clear();
    }

    public void TogglePlayerReady()
    {
        isReady = !isReady;
    }

    public void ChangeClientHostMode()
    {
        isReady = false;
        if(menuStates[0].activeSelf)
        {

            prevState = 1;
            menuStates[0].SetActive(false);
            LeaveRoom();
            Invoke("RefreshGamesList", 1f);
            menuStates[1].SetActive(true);
        }
        else
        {
            prevState = 0;
            menuStates[1].SetActive(false);
            ClearMatchList();
            foreach(Button butt in menuStates[0].GetComponentsInChildren<Button>())
            {
                if (butt.name == "ReadyButton") butt.interactable = false;
            }
            menuStates[0].SetActive(true);
        }
    }

    public void SettingsState()
    {
        Debug.Log("clicked");
        if(ingame)
        {
            menuStates[2].SetActive(!menuStates[2].activeSelf);//toggle 2
            menuStates[3].SetActive(!menuStates[2].activeSelf);//set 3
        }
        else if(menuStates[2].activeSelf)
        {
            menuStates[prevState].SetActive(true);
            menuStates[2].SetActive(false);
        }
        else
        {
            menuStates[prevState].SetActive(false);
            LeaveRoom();
            menuStates[2].SetActive(true);
        }
    }

    public void JoinRoom(MatchInfoSnapshot _match)
    {
        searchingStat.text = "Joining...";
        ChangeClientHostMode();
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        foreach (Button butt in menuStates[0].GetComponentsInChildren<Button>())
        {
            if (butt.name == "ReadyButton") butt.interactable = true;
        }
        InvokeRepeating("OnPlayerList", 0f, 0.5f);
    }

    public void LeaveRoom()
    {
        MatchInfo match = networkManager.matchInfo;
        if (match != null)
        {
            networkManager.matchMaker.DropConnection(match.networkId, match.nodeId, 0, networkManager.OnDropConnection);
            networkManager.StopHost();
            TogglePlayerReady();
            menuStates[2].GetComponentInChildren<InputField>().interactable = true;
            menuStates[2].GetComponentInChildren<Dropdown>().interactable = true;
        }
        CancelInvoke();
        ClearPlayersList();
    }
}
