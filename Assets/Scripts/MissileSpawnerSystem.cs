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

public class MissileSpawnerSystem : ComponentSystem
{
    public struct TurretData
    {
        public int Length;
        public ComponentDataArray<TransformParent> Parent;

        public ComponentDataArray<LocalPosition> Positions;
        public ComponentDataArray<ComponentTypes.TurretHeadState> State;        
    }

    [Inject]
    private TurretData m_turretData;

    protected override void OnUpdate()
    {
        EntityManager manager = World.Active.GetOrCreateManager<EntityManager>();

        for (int turretIdx = 0; turretIdx < m_turretData.Length; ++turretIdx)
        {
            if (m_turretData.State[turretIdx].TimeSinceLastFire >= 1.0f && m_turretData.State[turretIdx].CanFire == 1)
            {
                ComponentTypes.TurretHeadState stateCopy = m_turretData.State[turretIdx];

                Entity body = m_turretData.Parent[turretIdx].Value;
                Position bodyPosition = manager.GetComponentData<Position>(body);

                PostUpdateCommands.CreateEntity(Bootstrap.MissileArchetype);
                PostUpdateCommands.SetComponent(new Position {Value = new Vector3(bodyPosition.Value.x, bodyPosition.Value.y, bodyPosition.Value.z) + new Vector3(m_turretData.Positions[turretIdx].Value.x, m_turretData.Positions[turretIdx].Value.y, m_turretData.Positions[turretIdx].Value.z)});
                PostUpdateCommands.SetComponent(new MoveSpeed {speed = 10.0f});
                PostUpdateCommands.SetComponent(new Rotation {Value = Quaternion.Euler(0.0f, stateCopy.Angle, 0.0f)});
                PostUpdateCommands.SetComponent(new ComponentTypes.MissileState {BirthTime = Time.time});

                PostUpdateCommands.AddSharedComponent(Bootstrap.MissileLook);

                stateCopy.TimeSinceLastFire = 0.0f;
                m_turretData.State[turretIdx] = stateCopy;
            }
        }
    }
}