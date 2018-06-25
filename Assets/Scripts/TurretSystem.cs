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
        public ComponentDataArray<ComponentTypes.TurretState> TurretState;
    }

    [Inject]
    private Data m_data; 

    public static void SetupComponentData(EntityManager entityManager)
    {
        Debug.Log("SetupComponentData");
        var arch = entityManager.CreateArchetype(typeof(ComponentTypes.TurretState));

        var stateEntity = entityManager.CreateEntity(arch);
        var oldState = Random.state;

        entityManager.SetComponentData(stateEntity, new ComponentTypes.TurretState {AngleRadians = 0.0f });
    }

    protected override void OnUpdate()
    {
        Debug.Log("Test " + m_data.Length);

        for (int idx = 0; idx < m_data.Length; ++idx)
        {
            ComponentTypes.TurretState temp = m_data.TurretState[idx];
            temp.AngleRadians = 2.0f;

            m_data.TurretState[idx] = temp;
        }
    }
}
