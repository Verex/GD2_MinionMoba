using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class Warden : NetworkBehaviour
{
	public enum GameState
	{
		WAITING,
		STARTED
	}
    [System.Serializable] public class GameStartEvent : UnityEvent<Warden> { }
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] public int maxPlayerCount = 2;

	public GameStartEvent OnGameStart;
	public GameState currentState;

    private List<TowerSpawnPoint>[] towerSpawnLocations;
    private NetManager netManager;

    private IEnumerator PlayerWait()
    {
        // Wait until all players connected.
        yield return new WaitUntil(() => netManager.players.Count == maxPlayerCount);

		Debug.Log("All players joined. Spawning towers...");

        foreach (Player player in netManager.players)
        {
			// Add player to game start event listener.
			OnGameStart.AddListener(player.OnGameStart);

            // Spawn player's towers.
            foreach (TowerSpawnPoint point in towerSpawnLocations[player.index])
            {
                GameObject tower = Instantiate(towerPrefab, point.transform.position, point.transform.rotation);
                NetworkServer.Spawn(tower);
            }
        }

		// Set new game state.
		currentState = GameState.STARTED;

		// Invoke game start event.
		OnGameStart.Invoke(this);
    }

    private void Start()
    {
        if (!isServer) return;

		// Assign current game state.
		currentState = GameState.WAITING;

        // Get current net manager.
        netManager = (NetManager)NetworkManager.singleton;

        // Initialize player tower spawn location lists.
        towerSpawnLocations = new List<TowerSpawnPoint>[maxPlayerCount];

        for (int i = 0; i < maxPlayerCount; i++)
        {
            towerSpawnLocations[i] = new List<TowerSpawnPoint>();
        }

        // Find all player tower spawn locations.
        Object[] spawns = FindObjectsOfType(typeof(TowerSpawnPoint));

        // Add each to spawn list.
        foreach (Object spawn in spawns)
        {
            TowerSpawnPoint tsp = (TowerSpawnPoint)spawn;

            if (tsp.playerID >= 0 && tsp.playerID < maxPlayerCount)
            {
                towerSpawnLocations[tsp.playerID].Add(tsp);
            }
        }

        StartCoroutine(PlayerWait());
    }
}
