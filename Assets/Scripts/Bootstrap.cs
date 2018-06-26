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

    public static MeshInstanceRenderer TurretBodyLook;
    public static MeshInstanceRenderer TurretHeadLook;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        TurretBodyArchetype = entityManager.CreateArchetype(typeof(TransformMatrix), typeof(ComponentTypes.TurretBodyState));
        TurretHeadArchetype = entityManager.CreateArchetype(typeof(TransformMatrix), typeof(ComponentTypes.TurretHeadState));
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoadRuntimeMethod()
    {
        TurretBodyLook = GetLookFromPrototype("bodyproto"); 
        TurretHeadLook = GetLookFromPrototype("headproto"); 
        NewGame();
    }

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
    }

    public static void NewGame()
    {
        for (int idx = 0; idx < 100; ++idx)
        {
            InstanceTurret();
        }

        TurretSystem.SetupComponentData(World.Active.GetOrCreateManager<EntityManager>());
    }

    private static void InstanceTurret()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        Vector3 position = new Vector3(Mathf.Floor(Random.Range(-10.0f, 10.0f)), 0.0f, Mathf.Floor(Random.Range(-10.0f, 10.0f)));
        position += new Vector3(0.5f, 0.0f, 0.5f);
        Matrix4x4 trans = Matrix4x4.Translate(position);
        Matrix4x4 scale = Matrix4x4.Scale(new Vector3(1.0f, 1.0f, 1.0f));

        Entity turretBody = entityManager.CreateEntity(TurretBodyArchetype);
        Entity turretHead = entityManager.CreateEntity(TurretHeadArchetype);

        Matrix4x4 world = trans * scale;

        entityManager.SetComponentData(turretBody, new TransformMatrix { Value = world });
        entityManager.SetComponentData(turretBody, new ComponentTypes.TurretBodyState());
        entityManager.AddSharedComponentData(turretBody, TurretBodyLook);

        entityManager.SetComponentData(turretHead, new TransformMatrix { Value = world });
        entityManager.SetComponentData(turretHead, new ComponentTypes.TurretHeadState());
        entityManager.AddSharedComponentData(turretHead, TurretHeadLook);
    }

    private static MeshInstanceRenderer GetLookFromPrototype(string protoName)
    {
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<MeshInstanceRendererComponent>().Value;
        Object.Destroy(proto);
        return result;
    }
}
