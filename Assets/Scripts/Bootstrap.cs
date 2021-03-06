﻿using ComponentTypes;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Transforms2D;

public sealed class Bootstrap
{
    public static EntityArchetype TurretBodyArchetype;
    public static EntityArchetype TurretHeadArchetype;
    public static EntityArchetype TurretGun1Archetype;
    public static EntityArchetype TurretGun2Archetype;
    public static EntityArchetype Enemy1Archetype;
    public static EntityArchetype SpawnPointArchetype;
    public static EntityArchetype GoalPointArchetype;
    public static EntityArchetype MissileArchetype;
    public static EntityArchetype PlayerDataArchetype; // TODO : add score in here

    public static MeshInstanceRenderer TurretBodyLook;
    public static MeshInstanceRenderer TurretHeadLook;
    public static MeshInstanceRenderer TurretGun1Look;
    public static MeshInstanceRenderer TurretGun2Look;
    public static MeshInstanceRenderer Enemy1BodyLook;
    public static MeshInstanceRenderer MissileLook;

    private static Entity playerDataEntity;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        TurretBodyArchetype = entityManager.CreateArchetype(typeof(TransformMatrix), typeof(Position), typeof(Rotation), typeof(ComponentTypes.TurretBodyState));
        TurretHeadArchetype = entityManager.CreateArchetype(typeof(TransformMatrix), typeof(LocalPosition), typeof(LocalRotation), typeof(TransformParent), typeof(ComponentTypes.TurretHeadState));
        TurretGun1Archetype = entityManager.CreateArchetype(typeof(TransformMatrix), typeof(LocalPosition), typeof(LocalRotation), typeof(TransformParent), typeof(ComponentTypes.TurretGun1State));
        TurretGun2Archetype = entityManager.CreateArchetype(typeof(TransformMatrix), typeof(LocalPosition), typeof(LocalRotation), typeof(TransformParent), typeof(ComponentTypes.TurretGun2State));

        Enemy1Archetype = entityManager.CreateArchetype(typeof(TransformMatrix), typeof(Position), typeof(Rotation), typeof(Enemy));

        SpawnPointArchetype = entityManager.CreateArchetype(typeof(EnemySpawnPoint));
        GoalPointArchetype = entityManager.CreateArchetype(typeof(EnemyGoalPoint));

        MissileArchetype = entityManager.CreateArchetype(typeof(TransformMatrix), typeof(Position), typeof(Rotation), typeof(MoveSpeed), typeof(MoveForward), typeof(MissileState));
        PlayerDataArchetype = entityManager.CreateArchetype(typeof(PlayerSessionData));
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoadRuntimeMethod()
    {
        TurretBodyLook = GetLookFromPrototype("bodyproto"); 
        TurretHeadLook = GetLookFromPrototype("headproto"); 
        TurretGun1Look = GetLookFromPrototype("gun1proto"); 
        TurretGun2Look = GetLookFromPrototype("gun2proto");

        Enemy1BodyLook = GetLookFromPrototype("EnemyBodyProto");
                
        MissileLook = GetLookFromPrototype("missileproto");
        
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        Entity spawnPoint = entityManager.CreateEntity(SpawnPointArchetype);
        entityManager.SetComponentData(spawnPoint, new EnemySpawnPoint { GridIndex = new int2(23, 10) });
        Entity goalPoint = entityManager.CreateEntity(GoalPointArchetype);
        entityManager.SetComponentData(goalPoint, new EnemyGoalPoint { GridIndex = new int2(0, 10) });
 
        playerDataEntity = entityManager.CreateEntity(PlayerDataArchetype);
        entityManager.SetComponentData(playerDataEntity, new PlayerSessionData {CurrencyAmount = 10, Score = 0, Health = 10, gameState = GameState.START});
        
        UpdateHUDSystem.SetupGameObjects(World.Active.GetOrCreateManager<EntityManager>());
    }

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
    }
    
    public struct PlayerData
    {
        public int Length;
        public ComponentDataArray<PlayerSessionData> Player;
    }
    
    [Inject] static PlayerData m_PlayerData;

    public static void NewGame()
    {
        EnemySpawnSystem.SetupComponentData(World.Active.GetOrCreateManager<EntityManager>());
        
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        var data = entityManager.GetComponentData<PlayerSessionData>(playerDataEntity);

        data.gameState = GameState.PLAYING;
        
        entityManager.SetComponentData(playerDataEntity, data);

//        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

//        Entity curencyEntity = entityManager.CreateEntity(PlayerDataArchetype);
//        entityManager.SetComponentData(curencyEntity, new PlayerSessionData {CurrencyAmount = 10, Score = 0, Health = 10});
    }

//    public static void InstanceTurret()
//    {
//        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
//
//        Vector3 position = new Vector3(Mathf.Floor(Random.Range(-10.0f, 10.0f)), 0.0f, Mathf.Floor(Random.Range(-10.0f, 10.0f)));
//        position += new Vector3(0.5f, 0.0f, 0.5f);
//
//        Vector3 headPosition = position + new Vector3(0.0f, 0.6128496f, 0.0f);
//        Vector3 gun1Position = new Vector3(0.08563034f, 0.08383693f, 0.327976f);
//        Vector3 gun2Position = new Vector3(-0.08563034f, 0.08383693f, 0.327976f);
//
//        Entity turretBody = entityManager.CreateEntity(TurretBodyArchetype);
//        Entity turretHead = entityManager.CreateEntity(TurretHeadArchetype);
//        Entity turretGun1 = entityManager.CreateEntity(TurretGun1Archetype);
//        Entity turretGun2 = entityManager.CreateEntity(TurretGun2Archetype);
//
//        entityManager.SetComponentData(turretBody, new TransformMatrix {Value = Matrix4x4.identity});
//        entityManager.SetComponentData(turretBody, new Position {Value = position});
//        entityManager.SetComponentData(turretBody, new Rotation {Value = quaternion.identity});
//        entityManager.AddSharedComponentData(turretBody, TurretBodyLook);
//
//        entityManager.SetComponentData(turretHead, new TransformMatrix {Value = Matrix4x4.identity});
//        entityManager.SetComponentData(turretHead, new LocalPosition {Value = new Vector3(0.0f, 0.6128496f, 0.0f)});
//        entityManager.SetComponentData(turretHead, new LocalRotation {Value = quaternion.identity});
//        entityManager.SetComponentData(turretHead, new TransformParent {Value = turretBody});
//        entityManager.SetComponentData(turretHead, new TurretHeadState {TargetAngle = 0.0f, Angle = 0.0f, Translation = new Vector3(), TimeSinceLastFire = 1.0f, CanFire = 0});
//        entityManager.AddSharedComponentData(turretHead, TurretHeadLook);
//
//        entityManager.SetComponentData(turretGun1, new TransformMatrix {Value = Matrix4x4.identity});
//        entityManager.SetComponentData(turretGun1, new TransformParent {Value = turretHead});
//        entityManager.SetComponentData(turretGun1, new LocalPosition {Value = new Vector3(0.08563034f, 0.08383693f, 0.327976f)});
//        entityManager.SetComponentData(turretGun1, new LocalRotation {Value = quaternion.identity});
//        entityManager.AddSharedComponentData(turretGun1, TurretGun1Look);
//
//        entityManager.SetComponentData(turretGun2, new TransformMatrix {Value = Matrix4x4.identity});
//        entityManager.SetComponentData(turretGun2, new TransformParent {Value = turretHead});
//        entityManager.SetComponentData(turretGun2, new LocalPosition {Value = new Vector3(-0.08563034f, 0.08383693f, 0.327976f)});
//        entityManager.SetComponentData(turretGun2, new LocalRotation {Value = quaternion.identity});
//        entityManager.AddSharedComponentData(turretGun2, TurretGun2Look);
//    }

    private static MeshInstanceRenderer GetLookFromPrototype(string protoName)
    {
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<MeshInstanceRendererComponent>().Value;
        Object.Destroy(proto);
        return result;
    }
}
