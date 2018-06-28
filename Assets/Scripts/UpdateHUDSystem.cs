using ComponentTypes;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[AlwaysUpdateSystem]
public class UpdateHUDSystem : ComponentSystem
{
    public struct PlayerData
    {
        public int Length;
        [ReadOnly] public EntityArray Entity;
    }

    [Inject] PlayerData m_Players;

    public static void SetupGameObjects(EntityManager entityManager)
    {
        var buttonContainer = GameObject.Find("TurretButtons");
        for (int i = 0; i < buttonContainer.transform.childCount; i++)
        {
            var button = buttonContainer.transform.GetChild(i).GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    TurretButtonListener(entityManager, button.gameObject.transform.position);
                });
            }
        }
    }

    private static void TurretButtonListener(EntityManager entityManager, float3 position)
    {
        var InputTurretBodyArchetype = entityManager.CreateArchetype(typeof(TransformMatrix), typeof(Position), typeof(Rotation), typeof(ComponentTypes.TurretBodyState), typeof(ComponentTypes.InputState));
        var InputTurretHeadArchetype = entityManager.CreateArchetype(typeof(TransformMatrix), typeof(LocalPosition), typeof(LocalRotation), typeof(TransformParent), typeof(ComponentTypes.InputState));
//        Vector3 position = new Vector3(Mathf.Floor(Random.Range(-10.0f, 10.0f)), 0.0f, Mathf.Floor(Random.Range(-10.0f, 10.0f)));
//        //position = new Vector3(0.0f, 0.0f, -2.0f);
        position = new float3(-9.5f, 0.0f, 0.5f);
        Matrix4x4 trans = Matrix4x4.Translate(position);
        Entity turretBody = entityManager.CreateEntity(InputTurretBodyArchetype);
        Entity turretHead = entityManager.CreateEntity(InputTurretHeadArchetype);
        
        entityManager.SetComponentData(turretBody, new TransformMatrix {Value = Matrix4x4.identity});
        entityManager.SetComponentData(turretBody, new Position {Value = position});
        entityManager.SetComponentData(turretBody, new Rotation {Value = quaternion.identity});
        entityManager.SetComponentData(turretBody, new InputState());
        entityManager.AddSharedComponentData(turretBody, Bootstrap.TurretBodyLook);

        entityManager.SetComponentData(turretHead, new TransformMatrix {Value = Matrix4x4.identity});
        entityManager.SetComponentData(turretHead, new LocalPosition {Value = new Vector3(0.0f, 0.6128496f, 0.0f)});
        entityManager.SetComponentData(turretHead, new LocalRotation {Value = quaternion.identity});
        entityManager.SetComponentData(turretHead, new TransformParent {Value = turretBody});
//        entityManager.SetComponentData(turretHead, new Input());
        entityManager.AddSharedComponentData(turretHead, Bootstrap.TurretHeadLook);
    }

    protected override void OnUpdate()
    {
        // TODO : implement UI updates here
        // 
    }

}
