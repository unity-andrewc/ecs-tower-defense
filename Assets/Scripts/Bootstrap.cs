using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Transforms2D;

public sealed class Bootstrap
{
    public static EntityArchetype TurretArchetype;

    public static MeshInstanceRenderer TurretBodyLook;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        TurretArchetype = entityManager.CreateArchetype(typeof(TransformMatrix), typeof(ComponentTypes.TurretState));

        Debug.Log("OnBeforeSceneLoadRuntimeMethod()");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoadRuntimeMethod()
    {
        TurretBodyLook = GetLookFromPrototype("bodyproto"); 
        NewGame();
    }

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
    }

    public static void NewGame()
    {
        Debug.Log("NewGame()");
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        for (int idx = 0; idx < 100; ++idx)
        {
            Entity turret = entityManager.CreateEntity(TurretArchetype);

            Vector3 position = new Vector3(Mathf.Floor(Random.Range(-10.0f, 10.0f)), 0.0f, Mathf.Floor(Random.Range(-10.0f, 10.0f)));
            position += new Vector3(0.5f, 0.0f, 0.5f);
            Matrix4x4 trans = Matrix4x4.Translate(position);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(1.0f, 1.0f, 1.0f));

            Matrix4x4 world = trans * scale;

            entityManager.SetComponentData(turret, new TransformMatrix { Value = world });
            entityManager.SetComponentData(turret, new ComponentTypes.TurretState());
            entityManager.AddSharedComponentData(turret, TurretBodyLook);
        }

        TurretSystem.SetupComponentData(World.Active.GetOrCreateManager<EntityManager>());
    }

    private static MeshInstanceRenderer GetLookFromPrototype(string protoName)
    {
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<MeshInstanceRendererComponent>().Value;
        Object.Destroy(proto);
        return result;
    }
}
