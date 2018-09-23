using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class NetManager : NetworkManager
{
    public List<Player> players;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        Player p = player.GetComponent<Player>();
        p.index = PlayerState.Instance.playerIndex++;
        players.Add(p);
    }

    public override void OnStartServer()
    {
        // Setup player list.
        players = new List<Player>();
    }
}