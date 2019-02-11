using UnityEngine;
using UnityEngine.Networking;

public class PlayerColor : NetworkBehaviour {

    //when i think of a way to randomize colors for players or letting them choose in lobby without assets :V
    [SyncVar]
    public Color playColor = Color.white;
    MeshRenderer[] unitRends = null;

	void Start ()
    {
        unitRends = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < unitRends.Length; ++i)
            unitRends[i].material.color = playColor;
	}
}
