using System.Collections;
using System.Collections.Generic;
using ComponentTypes;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EnemySpawnSystem : ComponentSystem
{    
    struct State
    {
        public int Length;
        public ComponentDataArray<EnemySpawnCooldown> Cooldown;
    }

    [Inject] State m_State;

    public static void SetupComponentData(EntityManager entityManager)
    {
        var arch = entityManager.CreateArchetype(typeof(EnemySpawnCooldown));
        var stateEntity = entityManager.CreateEntity(arch);
        var oldState = Random.state;
        Random.InitState(0xaf77);
        entityManager.SetComponentData(stateEntity, new EnemySpawnCooldown { Cooldown = 0.0f });
        Random.state = oldState;
    }


    protected override void OnUpdate()
    {
        var cooldown = Mathf.Max(0.0f, m_State.Cooldown[0].Cooldown - Time.deltaTime);
        bool spawn = cooldown <= 0.0f;

        if (spawn)
        {
            cooldown = GetCooldown();
        }

        m_State.Cooldown[0] = new EnemySpawnCooldown { Cooldown = cooldown };

        if (spawn)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Debug.Log("Spawn Enemy");
        float3 spawnPosition = GetSpawnLocation();
        var trs = Matrix4x4.TRS(spawnPosition, Quaternion.identity, new float3(0.1f, 0.02f, 0.1f));
        
        PostUpdateCommands.CreateEntity(Bootstrap.Enemy1Archetype);
        PostUpdateCommands.SetComponent(new TransformMatrix { Value = trs });
        PostUpdateCommands.SetComponent(default(Enemy));
        PostUpdateCommands.AddSharedComponent(Bootstrap.Enemy1BodyLook);
        
        // TODO : We may have one mesh enemy
        // TODO : We may adjust the position of the head of the enemy

        trs = Matrix4x4.TRS(spawnPosition, Quaternion.identity, new float3(0.06f, 0.04f, 0.06f));
        PostUpdateCommands.CreateEntity(Bootstrap.Enemy1Archetype);
        PostUpdateCommands.SetComponent(new TransformMatrix { Value = trs });
        PostUpdateCommands.SetComponent(default(Enemy));
        PostUpdateCommands.AddSharedComponent(Bootstrap.Enemy1HeadLook);
        
    }

    // TODO : We may use the components to store wave's cooldown value
    float GetCooldown()
    {
        // will be changed, (WIP)
        return 1000.15f;
    }

    float3 GetSpawnLocation()
    {
        float3 position = new float3(10.0f, 0.0f, 0.0f);
        position += new float3(0.5f, 0.0f, 0.5f);

        return position;
    }
    
}
