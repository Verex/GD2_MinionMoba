using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject uiPrefab;
    public PlayerUI userInterface;
    public Base playerBase;
    public List<Tower> towers;
    public List<Minion> minions;
    new private PlayerCamera camera;
    public int index;

    [SyncVar] public float baseCurrentHealth;
    [SyncVar] public float baseMaxHealth;

    [TargetRpc]
    private void TargetSetupPlayer(NetworkConnection target, int playerIndex)
    {
        // Set up player's camera.
        camera = GameObject.Find("PlayerCamera").GetComponent<PlayerCamera>();
        camera.transform.position = camera.startPositions[playerIndex];

        // Set up player's UI.
        GameObject ui = Instantiate(uiPrefab);
        userInterface = ui.GetComponent<PlayerUI>();
        userInterface.player = this;

        PlayerState.Instance.localIndex = playerIndex;
    }

    private IEnumerator UpdateHealth()
    {
        yield return new WaitUntil(() => playerBase != null);
        yield return new WaitUntil(() => playerBase.damageable != null);
        while (true)
        {
            if (baseCurrentHealth != playerBase.damageable.currentHealth)
            {
                baseCurrentHealth = playerBase.damageable.currentHealth;
            }

            if (baseMaxHealth != playerBase.damageable.maxHealth)
            {
                baseMaxHealth = playerBase.damageable.maxHealth;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    // Use this for initialization
    void Start()
    {
        if (isServer)
        {
            // Setup local player.
            TargetSetupPlayer(connectionToClient, index);

            StartCoroutine(UpdateHealth());
        }
    }

    public void OnGameStart(Warden warden)
    {

    }
}
