using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public Base playerBase;
    public List<Tower> towers;
    public List<Minion> minions;
    new private PlayerCamera camera;
    public int index;

    [TargetRpc]
    private void TargetSetupPlayer(NetworkConnection target, int playerIndex)
    {
        camera = GameObject.Find("PlayerCamera").GetComponent<PlayerCamera>();

        camera.transform.position = camera.startPositions[playerIndex];
    }

    // Use this for initialization
    void Start()
    {
        if (isServer)
        {
            // Setup local player.
            TargetSetupPlayer(connectionToClient, index);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnGameStart(Warden warden)
    {
        
    }
}
