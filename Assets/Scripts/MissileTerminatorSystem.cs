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

public class MissileTerminatorSystem : ComponentSystem
{
    public struct MissileData
    {
        public int Length;
        public EntityArray InputEntities;
        public ComponentDataArray<Position> Positions;
        public ComponentDataArray<ComponentTypes.MissileState> State;
    }

    [Inject]
    private MissileData m_missileData;


    protected override void OnUpdate()
    {
        EntityManager manager = World.Active.GetOrCreateManager<EntityManager>();

        for (int idx = 0; idx < m_missileData.Length; ++idx)
        {
            if (Time.time - m_missileData.State[idx].BirthTime > 5.0f)
            {
                PostUpdateCommands.DestroyEntity(m_missileData.InputEntities[idx]);
            }
        }
    }
}