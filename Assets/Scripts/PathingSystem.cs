using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using ComponentTypes;

public class PathingSystem : ComponentSystem
{
    public PathingSystem()
    {
        Grid.Configure(new int2(20, 20), new float2(1.0f, 1.0f), new float3(0.0f, 0.0f, 0.0f));

        m_PathManager = new PathManager();

        m_NeighborOffsetsCardinal = new int2[4];
        m_NeighborOffsetsCardinal[0] = new int2(1, 0);
        m_NeighborOffsetsCardinal[1] = new int2(0, 1);
        m_NeighborOffsetsCardinal[2] = new int2(-1, 0);
        m_NeighborOffsetsCardinal[3] = new int2(0, -1);

        m_NeighborOffsetsIntercardinal = new int2[4];
        m_NeighborOffsetsIntercardinal[0] = new int2(1, 1);
        m_NeighborOffsetsIntercardinal[1] = new int2(-1, 1);
        m_NeighborOffsetsIntercardinal[2] = new int2(-1, -1);
        m_NeighborOffsetsIntercardinal[3] = new int2(1, -1);
    }

    struct TrackedEnemyData
    {
        public ComponentDataArray<Enemy>      Enemies;
        public ComponentDataArray<EnemyState> EnemyStates;
        public ComponentDataArray<Position>   Positions;
        public EntityArray                    Entities;

        public int Length;
    }
    [Inject] private TrackedEnemyData m_TrackedEnemyData;

    struct NewEnemyData
    {
        public ComponentDataArray<Enemy>        Enemies;
        public EntityArray                      Entities;
        public SubtractiveComponent<EnemyState> NoEnemyState;
        public ComponentDataArray<Position>     Positions;

        public int Length;
    }
    [Inject] private NewEnemyData m_NewEnemyData;

    struct DanglingEnemyData
    {
        public ComponentDataArray<EnemyState> EnemyStates;
        public EntityArray                    Entities;
        public SubtractiveComponent<Enemy>    NoEnemy;

        public int Length;
    }
    [Inject] private DanglingEnemyData m_DanglingEnemyData;

    struct TurretData
    {
        public ComponentDataArray<TurretBodyState> TurretBodies;
        public ComponentDataArray<Position>        Positions;

        public int Length;
    }
    [Inject] private TurretData m_TurretData;

    struct SpawnPointData
    {
        public ComponentDataArray<EnemySpawnPoint> SpawnPoint;

        public int Length;
    }
    [Inject] private SpawnPointData m_SpawnPointData;

    struct GoalPointData
    {
        public ComponentDataArray<EnemyGoalPoint> GoalPoint;

        public int Length;
    }
    [Inject] private GoalPointData m_GoalPointData;

    private int2 m_Goal;

    private int2[] m_NeighborOffsetsCardinal;
    private int2[] m_NeighborOffsetsIntercardinal;
    
    private GridBitfield m_PreviousBlockedTerrain;

    private float CalcHeurisitic(int2 lhs, int2 rhs)
    {
        return math.length(rhs - lhs);
    }

    private static int CmpNodes(Dictionary<int2, float> scorePassThrough, int2 lhs, int2 rhs)
    {
        float lhsScore = float.MaxValue;
        if (scorePassThrough.ContainsKey(lhs))
            lhsScore = scorePassThrough[lhs];

        float rhsScore = float.MaxValue;
        if (scorePassThrough.ContainsKey(rhs))
            rhsScore = scorePassThrough[rhs];

        if (lhsScore == rhsScore)
            return 0;

        if (lhsScore < rhsScore)
            return -1;

        return 1;
    }

    private bool IsNavigableGridIndex(GridBitfield blockedTerrain, int2 coords)
    {
        return
            coords.x >= 0 &&
            coords.y >= 0 &&
            coords.x < Grid.NumCells.x &&
            coords.y < Grid.NumCells.y &&
            !blockedTerrain[coords.x, coords.y];
    }

    private float3 HandleLiveEnemy(float3 currentPosition, EnemyState enemyState, GridBitfield blockedTerrainForCardinal, GridBitfield blockedTerrainForIntercardinal)
    {
        {
            List<float3> path = m_PathManager.GetPath(enemyState.PathId);
            if (path.Count > 0)
            {
                float3 ret = path[path.Count - 1];
                return ret;
            }
        }

        int2 currentGridIdx = Grid.ConvertToGridIndex(currentPosition);
        GridBitfield closedSet = new GridBitfield();

        // TODO: clean up the terrible perf incurred from just using an array here
        List<int2> openSet = new List<int2>();
        openSet.Add(Grid.ConvertToGridIndex(currentPosition));

        Dictionary<int2, int2> cameFrom = new Dictionary<int2, int2>();

        // nodes have a default value of infinity
        Dictionary<int2, float> gScore = new Dictionary<int2, float>();
        gScore[currentGridIdx] = 0.0f;

        // nodes have a default value of infinity
        Dictionary<int2, float> fScore = new Dictionary<int2, float>();
        fScore[currentGridIdx] = CalcHeurisitic(currentGridIdx, m_Goal);

        bool pathFound = false;
        while (openSet.Count > 0)
        {
            int bestIndex = 0;
            for (int b = 1; b < openSet.Count; ++b)
            {
                if (CmpNodes(fScore, openSet[bestIndex], openSet[b]) > 0)
                    bestIndex = b;
            }

            int2 currentNode = openSet[bestIndex];
            if (m_Goal.Equals(currentNode))
            {
                pathFound = true;

                List<float3> path = m_PathManager.GetPath(enemyState.PathId);
                path.Add(Grid.ConvertToWorldPosition(currentNode));
                while (cameFrom.ContainsKey(currentNode))
                {
                    currentNode = cameFrom[currentNode];
                    path.Add(Grid.ConvertToWorldPosition(currentNode));
                }

                float3 ret = path[path.Count - 1];
                return ret;
            }

            openSet.RemoveAt(bestIndex);
            closedSet[currentNode] = true;
            int2[] neighbors = new int2[8];
            int numNeighbors = 0;

            int blockedCardinalFlags = 0;
            for (int neighborIndex = 0; neighborIndex < m_NeighborOffsetsCardinal.Length; ++neighborIndex)
            {
                int2 neighbor = currentNode + m_NeighborOffsetsCardinal[neighborIndex];
                if (!IsNavigableGridIndex(blockedTerrainForCardinal, neighbor))
                {
                    blockedCardinalFlags |= 1 << neighborIndex;
                    continue;
                }

                neighbors[numNeighbors] = neighbor;
                ++numNeighbors;
            }

            for (int neighborIndex = 0; neighborIndex < m_NeighborOffsetsIntercardinal.Length; ++neighborIndex)
            {
                int adjacentCardinalFlags = (1 << neighborIndex) | (1 << ((neighborIndex + 1) % 4));
                if (adjacentCardinalFlags == (adjacentCardinalFlags & blockedCardinalFlags))
                    continue;

                int2 neighbor = currentNode + m_NeighborOffsetsIntercardinal[neighborIndex];
                if (!IsNavigableGridIndex(blockedTerrainForIntercardinal, neighbor))
                    continue;

                neighbors[numNeighbors] = neighbor;
                ++numNeighbors;
            }

            for (int neighborIndex = 0; neighborIndex < numNeighbors; ++neighborIndex)
            {
                if (closedSet[neighbors[neighborIndex]])
                    continue;

                // why doesn't Contains do what it's supposed to...?
                bool exists = false;
                foreach (int2 iv in openSet)
                {
                    if (neighbors[neighborIndex].Equals(iv))
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                    openSet.Add(neighbors[neighborIndex]);

                float tentativeScoreFromStart = gScore[currentNode] + math.length(currentNode - neighbors[neighborIndex]);

                float scoreFromStartToNeighbor = float.MaxValue;
                if (gScore.ContainsKey(neighbors[neighborIndex]))
                    scoreFromStartToNeighbor = gScore[neighbors[neighborIndex]];
                if (tentativeScoreFromStart >= scoreFromStartToNeighbor)
                    continue;

                cameFrom[neighbors[neighborIndex]] = currentNode;
                gScore[neighbors[neighborIndex]] = tentativeScoreFromStart;
                fScore[neighbors[neighborIndex]] = tentativeScoreFromStart + CalcHeurisitic(neighbors[neighborIndex], m_Goal);
            }
        }

        Debug.LogError("FAILED TO FIND PATH");
        return -Vector3.one;
    }

    private static bool WouldDeltaJumpTarget(float3 old, float3 delta, float3 target)
    {
        float3 fullMove = old + delta;
        if (target.Equals(fullMove))
            return true;

        float3 toTarget = target - fullMove;
        return math.dot(delta, toTarget) < 0.0f;
    }

    //bool first = true;
    protected override unsafe void OnUpdate()
    {
        m_Goal = m_GoalPointData.GoalPoint[0].GridIndex;

        GridBitfield blockedTerrainForCardinal = new GridBitfield();
        GridBitfield blockedTerrainForIntercardinal = new GridBitfield();
        for (int turretIndex = 0; turretIndex < m_TurretData.Length; ++turretIndex)
        {
            int2 turretGridCoords = Grid.ConvertToGridIndex(m_TurretData.Positions[turretIndex].Value);
            blockedTerrainForCardinal[turretGridCoords] = true;
            blockedTerrainForIntercardinal[turretGridCoords] = true;
            if (turretGridCoords.x > 0)
                blockedTerrainForIntercardinal[turretGridCoords.x - 1, turretGridCoords.y] = true;
            if (turretGridCoords.x < Grid.NumCells.x - 1)
                blockedTerrainForIntercardinal[turretGridCoords.x + 1, turretGridCoords.y] = true;
            if (turretGridCoords.y > 0)
                blockedTerrainForIntercardinal[turretGridCoords.x, turretGridCoords.y - 1] = true;
            if (turretGridCoords.y < Grid.NumCells.y - 1)
                blockedTerrainForIntercardinal[turretGridCoords.x, turretGridCoords.y + 1] = true;
        }

        // clear out all previous paths to force path-recompute if the tower layour has changed
        if (m_PreviousBlockedTerrain != null && m_PreviousBlockedTerrain != blockedTerrainForCardinal)
            m_PathManager.ForcePathRecompute();
        m_PreviousBlockedTerrain = blockedTerrainForCardinal;

        for (int trackedEnemyIndex = 0; trackedEnemyIndex < m_TrackedEnemyData.Length; ++trackedEnemyIndex)
        {
            float3 currentPos = m_TrackedEnemyData.Positions[trackedEnemyIndex].Value;
            float3 target = HandleLiveEnemy(currentPos, m_TrackedEnemyData.EnemyStates[trackedEnemyIndex], blockedTerrainForCardinal, blockedTerrainForIntercardinal);

            if (currentPos.Equals(target))
            {
                List<float3> path = m_PathManager.GetPath(m_TrackedEnemyData.EnemyStates[trackedEnemyIndex].PathId);
                path.RemoveAt(path.Count - 1);
                target = HandleLiveEnemy(currentPos, m_TrackedEnemyData.EnemyStates[trackedEnemyIndex], blockedTerrainForCardinal, blockedTerrainForIntercardinal);
            }

            float3 delta = Time.deltaTime * m_TrackedEnemyData.Enemies[trackedEnemyIndex].Speed * math.normalize(target - currentPos);

            if (WouldDeltaJumpTarget(currentPos, delta, target))
            {
                float3 toTarget = target - currentPos;
                float magDelta = math.length(delta);
                float normalizedTimeUsed = math.length(toTarget) / magDelta;
                float distanceLeft = (1.0f - normalizedTimeUsed) * magDelta;

                List<float3> path = m_PathManager.GetPath(m_TrackedEnemyData.EnemyStates[trackedEnemyIndex].PathId);
                path.RemoveAt(path.Count - 1);

                if (path.Count == 0)
                {
                    PostUpdateCommands.DestroyEntity(m_TrackedEnemyData.Entities[trackedEnemyIndex]);
                    return;
                }

                currentPos = target;
                target = path[path.Count - 1];
                delta = distanceLeft * math.normalize(target - currentPos);
            }

            m_TrackedEnemyData.Positions[trackedEnemyIndex] = new Position { Value = currentPos + delta };
        }

        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        for (int newEnemyIndex = 0; newEnemyIndex < m_NewEnemyData.Length; ++newEnemyIndex)
        {
            EnemyState enemyState = new EnemyState{ PathId = m_PathManager.AddPath() };
            entityManager.AddComponentData<EnemyState>(m_NewEnemyData.Entities[newEnemyIndex], enemyState);
        }

        for (int danglingEnemyIndex = 0; danglingEnemyIndex < m_DanglingEnemyData.Length; ++danglingEnemyIndex)
        {
            m_PathManager.RemovePath(m_DanglingEnemyData.EnemyStates[danglingEnemyIndex].PathId);
            entityManager.RemoveComponent<EnemyState>(m_DanglingEnemyData.Entities[danglingEnemyIndex]);
        }
    }

    private class PathManager
    {
        public PathManager()
        {
            m_Paths = new List<List<float3>>();
            m_FreeList = new List<int>();
        }

        public List<float3> GetPath(int id)
        {
            return m_Paths[id - 1];
        }

        public int AddPath()
        {
            int idx = -1;

            if (m_FreeList.Count > 0)
            {
                idx = m_FreeList[m_FreeList.Count - 1];
                m_FreeList.RemoveAt(m_FreeList.Count - 1);
            }
            else
            {
                idx = m_Paths.Count;
                m_Paths.Add(null);
            }

            m_Paths[idx] = new List<float3>();
            return idx + 1;
        }

        public void RemovePath(int id)
        {
            m_Paths[id - 1] = null;
            m_FreeList.Add(id - 1);
        }

        public void ForcePathRecompute()
        {
            foreach (List<float3> path in m_Paths)
            {
                if (path != null)
                    path.Clear();
            }
        }

        private List<List<float3>> m_Paths;
        private List<int> m_FreeList;
    }

    private PathManager m_PathManager;
}
