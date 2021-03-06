﻿using ComponentTypes;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EnemySpawnSystem : ComponentSystem
{    
    struct EnemySpawnState
    {
        public int Length;
        public EntityArray InputEntities;
        public ComponentDataArray<EnemySpawn> Enemy;
    }
    
    struct WaveSpawnState
    {
        public int Length;
        public EntityArray InputEntities;
        public ComponentDataArray<WaveSpawn> Wave;
    }
    
    struct SpawnPointState
    {
        public int Length;
        public ComponentDataArray<EnemySpawnPoint> SpawnPoint;
    }
    public struct PlayerData
    {
        public int Length;
        public ComponentDataArray<PlayerSessionData> Player;
    }
    
    [Inject] PlayerData m_PlayerData;
    
    [Inject] EnemySpawnState m_EnemySpawnState;
    [Inject] WaveSpawnState m_WaveSpawnState;
    [Inject] SpawnPointState m_SpawnPointState;


    public static void SetupComponentData(EntityManager entityManager)
    {
        var arch = entityManager.CreateArchetype(typeof(WaveSpawn), typeof(EnemySpawn));
        var stateEntity = entityManager.CreateEntity(arch);
        
        var oldState = Random.state;
        Random.InitState(0xaf77);
        entityManager.SetComponentData(stateEntity, new WaveSpawn { Cooldown = Constants.Enemy.WAVE_COOLDOWN , SpawnedEnemyCount = 3, SpawnedWaveCount = 0});
        entityManager.SetComponentData(stateEntity, new EnemySpawn { Cooldown = 0.0f });
        Random.state = oldState;
    }

    
    protected override void OnUpdate()
    {
        if (m_PlayerData.Length < 0 || !m_PlayerData.Player[0].gameState.Equals(GameState.PLAYING))
        {
            return;
        }
        
        var wave = m_WaveSpawnState.Wave[0];
        if (wave.SpawnedEnemyCount < wave.SpawnedWaveCount + 2)
        {
            var cooldown = Mathf.Max(0.0f, m_EnemySpawnState.Enemy[0].Cooldown - Time.deltaTime);
            bool spawn = cooldown <= 0.0f;

            if (spawn)
            {
                cooldown = Constants.Enemy.COOLDOWN;
            }

            m_EnemySpawnState.Enemy[0] = new EnemySpawn { Cooldown = cooldown };

            if (spawn)
            {
                SpawnEnemy();
            }
        }
        else
        {
            m_EnemySpawnState.Enemy[0] = new EnemySpawn { Cooldown = 0f };
            
            var cooldown = Mathf.Max(0.0f, wave.Cooldown - Time.deltaTime);
            bool spawn = cooldown <= 0.0f;

            wave.Cooldown = cooldown;
            m_WaveSpawnState.Wave[0] = wave;

            if (spawn)
            {
                SpawnWave();
            }
        }
    }

    void SpawnWave()
    {
        var wave = m_WaveSpawnState.Wave[0];

        wave.SpawnedEnemyCount = 0;
        wave.SpawnedWaveCount++;
        wave.Cooldown = Constants.Enemy.WAVE_COOLDOWN;
        
        m_WaveSpawnState.Wave[0] = wave;
    }

    void SpawnEnemy()
    {
        var wave = m_WaveSpawnState.Wave[0];
        var oldState = Random.state;
        Random.state = wave.RandomState;
        wave.SpawnedEnemyCount++;
        
        float3 spawnPosition = GetSpawnLocation();
        spawnPosition += new float3(0.0f, 1.0f, 0.0f);
        
        var trs = Matrix4x4.TRS(spawnPosition, Quaternion.identity, Vector3.one);
        
        PostUpdateCommands.CreateEntity(Bootstrap.Enemy1Archetype);
        PostUpdateCommands.SetComponent(new TransformMatrix { Value = trs });
        PostUpdateCommands.SetComponent(new Position { Value = spawnPosition });
        PostUpdateCommands.SetComponent(new Rotation { Value = quaternion.identity });
        PostUpdateCommands.SetComponent(new Enemy { Speed = 3.7f, Health = wave.SpawnedWaveCount * (wave.SpawnedWaveCount / 2) + 1 });
        PostUpdateCommands.AddSharedComponent(Bootstrap.Enemy1BodyLook);  
        wave.RandomState = Random.state;

        m_WaveSpawnState.Wave[0] = wave;
        Random.state = oldState;
        
    }

    float3 GetSpawnLocation()
    {
        return Grid.ConvertToWorldPosition(m_SpawnPointState.SpawnPoint[0].GridIndex);
    }
}
