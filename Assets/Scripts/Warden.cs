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
        IN_GAME,
        ENDED
    }
    [System.Serializable] public class GameStartEvent : UnityEvent<Warden> { }
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private GameObject basePrefab;
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] public Material[] playerColor;
    [SerializeField] public Material[] playerMinionMaterial;
    [SerializeField] public int maxPlayerCount = 2;

    public GameStartEvent OnGameStart;
    public GameState currentState;

    private List<TowerSpawnPoint>[] towerSpawnPoints;
    private List<MinionSpawnPoint>[] minionSpawnPoints;
    private BaseSpawnPoint[] baseSpawnPoints;
    private NetManager netManager;

    private IEnumerator MinionSpawn()
    {
        while (currentState == GameState.IN_GAME)
        {
            foreach (Player player in netManager.players)
            {
                foreach (MinionSpawnPoint point in minionSpawnPoints[player.index])
                {
                    GameObject minion = Instantiate(minionPrefab, point.transform.position, point.transform.rotation);

                    NetworkServer.Spawn(minion);

                    Minion m = minion.GetComponent<Minion>();
                    m.RpcSetTeamMaterial(player.index);
                }
            }

            yield return new WaitForSeconds(10.0f);
        }
    }

    private IEnumerator PlayerWait()
    {
        // Wait until all players connected.
        yield return new WaitUntil(() => netManager.players.Count == maxPlayerCount);

        Debug.Log("All players joined. Spawning player objects.");

        foreach (Player player in netManager.players)
        {
            // Add player to game start event listener.
            OnGameStart.AddListener(player.OnGameStart);

            // Spawn player's towers.
            foreach (TowerSpawnPoint point in towerSpawnPoints[player.index])
            {
                GameObject tower = Instantiate(towerPrefab, point.transform.position, point.transform.rotation);

                NetworkServer.Spawn(tower);

                Tower playerTower = tower.GetComponent<Tower>();
                playerTower.RpcSetTeamMaterial(player.index);
            }

            if (baseSpawnPoints[player.index] != null)
            {
                GameObject playerBase = Instantiate(basePrefab,
                    baseSpawnPoints[player.index].transform.position,
                    baseSpawnPoints[player.index].transform.rotation);

                NetworkServer.Spawn(playerBase);

                Base pb = playerBase.GetComponent<Base>();
                pb.RpcSetTeamMaterial(player.index);
            }
        }

        // Set new game state.
        currentState = GameState.IN_GAME;

        // Invoke game start event.
        OnGameStart.Invoke(this);

        StartCoroutine(MinionSpawn());

        yield break;
    }

    private void Start()
    {
        if (!isServer) return;

        // Assign current game state.
        currentState = GameState.WAITING;

        // Get current net manager.
        netManager = (NetManager)NetworkManager.singleton;

        // Initialize player tower spawn location lists.
        towerSpawnPoints = new List<TowerSpawnPoint>[maxPlayerCount];

        for (int i = 0; i < maxPlayerCount; i++)
        {
            towerSpawnPoints[i] = new List<TowerSpawnPoint>();
        }

        // Find all player tower spawn locations.
        Object[] spawns = FindObjectsOfType(typeof(TowerSpawnPoint));

        // Add each to spawn list.
        foreach (Object spawn in spawns)
        {
            TowerSpawnPoint tsp = (TowerSpawnPoint)spawn;

            if (tsp.playerID >= 0 && tsp.playerID < maxPlayerCount)
            {
                towerSpawnPoints[tsp.playerID].Add(tsp);
            }
        }

        // Initialize player base spawn points.
        baseSpawnPoints = new BaseSpawnPoint[maxPlayerCount];

        // Find player base spawn points.
        Object[] baseSpawns = FindObjectsOfType(typeof(BaseSpawnPoint));

        foreach (Object spawn in baseSpawns)
        {
            BaseSpawnPoint bsp = (BaseSpawnPoint)spawn;

            if (bsp.playerID >= 0 && bsp.playerID < maxPlayerCount)
            {
                baseSpawnPoints[bsp.playerID] = bsp;
            }
        }

        // Initialize player minion spawn points.
        minionSpawnPoints = new List<MinionSpawnPoint>[maxPlayerCount];

        for (int i = 0; i < maxPlayerCount; i++)
        {
            minionSpawnPoints[i] = new List<MinionSpawnPoint>();
        }

        Object[] minionSpawns = FindObjectsOfType(typeof(MinionSpawnPoint));

        foreach (Object spawn in minionSpawns)
        {
            MinionSpawnPoint msp = (MinionSpawnPoint)spawn;

            if (msp.playerID >= 0 && msp.playerID < maxPlayerCount)
            {
                minionSpawnPoints[msp.playerID].Add(msp);
            }
        }

        StartCoroutine(PlayerWait());
    }
}
