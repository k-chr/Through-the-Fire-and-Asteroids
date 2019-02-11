using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbyUIEL : MonoBehaviour
{
    [SerializeField]
    private Text thisTextName;
    [SerializeField]
    private Text thisTextState;

    public void Setup(string _name, bool _isReady)
    {
        thisTextName.text = _name;
        if (_isReady) thisTextState.color = Color.green;
        else thisTextState.color = Color.red;
    }
}