using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using ComponentTypes;
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
    
    public struct PlayerData
    {
        public int Length;
        public ComponentDataArray<PlayerSessionData> Player;
    }

    [Inject]
    private MissileData m_missileData;
    [Inject]
    private EnemyData m_enemyData;
    
    [Inject] PlayerData m_PlayerData;

    protected override void OnUpdate()
    {
        EntityManager manager = World.Active.GetOrCreateManager<EntityManager>();

        for (int mIdx = 0; mIdx < m_missileData.Length; ++mIdx)
        {
            float3 missilePos = m_missileData.Positions[mIdx].Value;
            missilePos.y = 0.0f;

            for (int eIdx = 0; eIdx < m_enemyData.Length; ++eIdx)
            {
                float3 enemyPos = m_enemyData.Positions[eIdx].Value;
                enemyPos.y = 0.0f;


                if (math.length(enemyPos - missilePos) < 1.0f)
                {
                    ComponentTypes.Enemy enemyState = m_enemyData.State[eIdx];
                    enemyState.Health -= 1;

                    if ( enemyState.Health <= 0)
                    {
                        for (int i = 0; i < m_PlayerData.Player.Length; i++)
                        {
                            var playerData = m_PlayerData.Player[i];
                            playerData.CurrencyAmount++;
                            playerData.Score += 10 ;
                            m_PlayerData.Player[i] = playerData;
                        }
                        PostUpdateCommands.DestroyEntity(m_enemyData.InputEntities[eIdx]);
                    }

                    PostUpdateCommands.DestroyEntity(m_missileData.InputEntities[mIdx]);
                    m_enemyData.State[eIdx] = enemyState;
                }
            }
        }
    }
}