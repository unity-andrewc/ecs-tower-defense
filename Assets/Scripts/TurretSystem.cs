using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;

public class TurretSystem : ComponentSystem
{
    public struct Data
    {
        public int Length;
        public ComponentDataArray<ComponentTypes.TurretBodyState> TurretBodyState;
        public ComponentDataArray<ComponentTypes.TurretHeadState> TurretHeadState;
    }

    [Inject]
    private Data m_data; 

    public static void SetupComponentData(EntityManager entityManager)
    {
        var arch = entityManager.CreateArchetype(typeof(ComponentTypes.TurretBodyState));
        var stateEntity = entityManager.CreateEntity(arch);
        var oldState = Random.state;

        entityManager.SetComponentData(stateEntity, new ComponentTypes.TurretBodyState {AngleRadians = 0.0f });
    }

    protected override void OnUpdate()
    {
        for (int idx = 0; idx < m_data.Length; ++idx)
        {
            ComponentTypes.TurretBodyState temp = m_data.TurretBodyState[idx];
            temp.AngleRadians = 2.0f;

            m_data.TurretBodyState[idx] = temp;
        }
    }
}
