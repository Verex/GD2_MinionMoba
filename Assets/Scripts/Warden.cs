using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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


    public int winnerIndex = -1;

    public GameStartEvent OnGameStart;
    public GameState currentState;

    private List<TowerSpawnPoint>[] towerSpawnPoints;
    private List<MinionSpawnPoint>[] minionSpawnPoints;
    private List<List<Vector3>>[] minionPaths;
    private BaseSpawnPoint[] baseSpawnPoints;
    private NetManager netManager;

    [ClientRpc]
    private void RpcGameOver(int winnerIndex)
    {
        PlayerState.Instance.winnerIndex = winnerIndex;

        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
    }

    private IEnumerator MinionSpawn()
    {
        while (currentState == GameState.IN_GAME)
        {
            foreach (Player player in netManager.players)
            {
                int pathIndex = 0;

                foreach (MinionSpawnPoint point in minionSpawnPoints[player.index])
                {
                    // Create minion game object.
                    GameObject mGameObject = Instantiate(minionPrefab, point.transform.position, point.transform.rotation);

                    // Push game object to server.
                    NetworkServer.Spawn(mGameObject);

                    // Get the minion's main component.
                    Minion minion = mGameObject.GetComponent<Minion>();

                    // Assign team material.
                    minion.RpcSetTeamMaterial(player.index);

                    minion.ownerIndex = player.index;

                    // Add minion to player's list.
                    player.minions.Add(minion);

                    // Add path to queue.
                    minion.path = new Queue<Vector3>(minionPaths[player.index][pathIndex]);

                    // Set next path index.
                    pathIndex = (pathIndex + 1) % minionPaths[player.index].Count;
                }
            }

            yield return new WaitForSeconds(8.0f);
        }
    }

    public void OnPlayerBaseDestroyed(Damageable damageable, Damager damager)
    {
        Base b = damageable.GetComponent<Base>();

        winnerIndex = (b.ownerIndex == 1 ? 0 : 1);
        RpcGameOver(winnerIndex);
    }

    private IEnumerator GameUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
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

                playerTower.ownerIndex = player.index;

                // Add tower to list.
                player.towers.Add(playerTower);
            }

            if (baseSpawnPoints[player.index] != null)
            {
                GameObject playerBase = Instantiate(basePrefab,
                    baseSpawnPoints[player.index].transform.position,
                    baseSpawnPoints[player.index].transform.rotation);

                NetworkServer.Spawn(playerBase);

                Base pb = playerBase.GetComponent<Base>();
                pb.RpcSetTeamMaterial(player.index);
                pb.ownerIndex = player.index;

                Damageable dmg = playerBase.GetComponent<Damageable>();
                dmg.OnDie.AddListener(OnPlayerBaseDestroyed);

                // Assign players base.
                player.playerBase = pb;
            }
        }

        // Set new game state.
        currentState = GameState.IN_GAME;

        // Invoke game start event.
        OnGameStart.Invoke(this);

        // Start minion spawning coroutine.
        StartCoroutine(MinionSpawn());

        // Start game control coroutine.
        StartCoroutine(GameUpdate());

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

        // Find all minion spawn points.
        Object[] minionSpawns = FindObjectsOfType(typeof(MinionSpawnPoint));

        // Sort and store minion spawn points.
        foreach (Object spawn in minionSpawns)
        {
            MinionSpawnPoint msp = (MinionSpawnPoint)spawn;

            if (msp.playerID >= 0 && msp.playerID < maxPlayerCount)
            {
                minionSpawnPoints[msp.playerID].Add(msp);
            }
        }

        // Setup minion path lists.
        minionPaths = new List<List<Vector3>>[maxPlayerCount];

        // Setup array of lists.
        for (int i = 0; i < maxPlayerCount; i++)
        {
            minionPaths[i] = new List<List<Vector3>>();
        }

        // Find all path starting nodes.
        Object[] pathStartNodes = FindObjectsOfType(typeof(PathStartNode));

        foreach (Object startNode in pathStartNodes)
        {
            PathStartNode sNode = (PathStartNode)startNode;

            if (sNode.playerIndex >= 0 && sNode.playerIndex < maxPlayerCount)
            {
                List<Vector3> path = new List<Vector3>();

                // Add starting node position.
                path.Add(sNode.transform.position);

                // Get path node component.
                PathNode node = sNode.gameObject.GetComponent<PathNode>();

                do
                {
                    // Move to next node.
                    node = node.next;

                    // Add node to list.
                    path.Add(node.transform.position);
                }
                while (node.next != null);

                // Add path to list.
                minionPaths[sNode.playerIndex].Add(path);
            }
        }

        StartCoroutine(PlayerWait());
    }
}
