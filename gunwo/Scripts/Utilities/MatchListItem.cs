using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class MatchListItem : MonoBehaviour
{
    private MatchInfoSnapshot match;
    [SerializeField]
    private Text roomname;
    [SerializeField]
    private Text roomFill;
    [SerializeField]
    private Text state;

    public delegate void JoinGameDelegate(MatchInfoSnapshot _match);
    private JoinGameDelegate joinGameCallback;

    public void Setup(MatchInfoSnapshot _match, JoinGameDelegate _joinGameCallback)
    {
        match = _match;
        roomname.text = match.name;
        joinGameCallback = _joinGameCallback;
        roomFill.text = match.currentSize + "/" + match.maxSize;
        if (match.currentSize >= match.maxSize) state.text = "FULL";
        else state.text = "";
    }

    public void JoinGame()
    {
        joinGameCallback.Invoke(match);
    }
}
