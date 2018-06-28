using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms2D;

public class MissileImpactSystem : ComponentSystem
{
    public struct MissileData
    {
        public int Length;
        public EntityArray InputEntities;
        public ComponentDataArray<Position> Positions;
        public ComponentDataArray<ComponentTypes.MissileState> State;
    }

    public struct EnemyData
    {
        public int Length;
        public EntityArray InputEntities;
        public ComponentDataArray<Position> Positions;
        public ComponentDataArray<ComponentTypes.Enemy> State;
    }

    [Inject]
    private MissileData m_missileData;
    [Inject]
    private EnemyData m_enemyData;

    protected override void OnUpdate()
    {
        EntityManager manager = World.Active.GetOrCreateManager<EntityManager>();

        for (int mIdx = 0; mIdx < m_missileData.Length; ++mIdx)
        {
            float3 missilePos = m_missileData.Positions[mIdx].Value;

            for (int eIdx = 0; eIdx < m_enemyData.Length; ++eIdx)
            {
                float3 enemyPos = m_enemyData.Positions[eIdx].Value;

                if (math.length(enemyPos - missilePos) < 0.5f)
                {
                    PostUpdateCommands.DestroyEntity(m_missileData.InputEntities[mIdx]);
                    PostUpdateCommands.DestroyEntity(m_enemyData.InputEntities[eIdx]);
                }
            }
        }
    }
}