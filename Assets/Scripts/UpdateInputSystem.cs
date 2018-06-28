using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysUpdateSystem]
public class UpdateInputSystem : ComponentSystem
{
    public struct InputData
    {
        public int Length;
        public EntityArray InputEntities;
        public ComponentDataArray<ComponentTypes.Input> Input;
        public ComponentDataArray<Position> TurretPosition;
    }
    
    public struct InputDataChildren
    {
        public int Length;
        public EntityArray InputEntities;
        public ComponentDataArray<ComponentTypes.Input> Input;
        public ComponentDataArray<LocalPosition> TurretHeadPosition;
    }

    [Inject] InputData m_Inputs;
    [Inject] InputDataChildren m_InputChildren;

    protected override void OnUpdate()
    {
        if (m_Inputs.Length > 0)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
//                Debug.Log("Raycast point: " + hit.point);
                var gridIndex = Grid.ConvertToGridIndex(hit.point);
                var gridWorldPos = Grid.ConvertToWorldPosition(gridIndex);
                if (Input.GetMouseButtonDown(0))
                {
                    var em = PostUpdateCommands;
                    if (hit.collider.tag.Equals("Floor"))
                    {
                        InstanceTurret(gridWorldPos);
//                        Debug.Log("Grid Index: " + gridIndex);
//                        Debug.Log("Grid World Pos: " + gridWorldPos);
                    } 
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    for (int i = 0; i < m_Inputs.InputEntities.Length; i++)
                    {
                        var data = m_Inputs.InputEntities[i];
//                        Debug.Log("Input entity: " + data);
                        PostUpdateCommands.DestroyEntity(data);
                    }
                    for (int i = 0; i < m_InputChildren.InputEntities.Length; i++)
                    {
                        var data = m_InputChildren.InputEntities[i];
//                        Debug.Log("Input entity: " + data);
                        PostUpdateCommands.DestroyEntity(data);
                    }
                }
                else
                {
                    for (int i = 0; i < m_Inputs.TurretPosition.Length; i++)
                    {
                        var data = m_Inputs.TurretPosition[i];
                        data = new Position(gridWorldPos);
                        m_Inputs.TurretPosition[i] = data;
                    }
                }
            }
        }
    }

    public static void InstanceTurret(float3 position)
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        Matrix4x4 trans = Matrix4x4.Translate(position);
//        Vector3 headPosition = position + new float3(0.0f, 0.6128496f, 0.0f);
//        Vector3 gun1Position = new Vector3(0.08563034f, 0.08383693f, 0.327976f);
//        Vector3 gun2Position = new Vector3(-0.08563034f, 0.08383693f, 0.327976f);
//        float rotateAngle = Random.Range(0.0f, 360.0f);

        Entity turretBody = entityManager.CreateEntity(Bootstrap.TurretBodyArchetype);
        Entity turretHead = entityManager.CreateEntity(Bootstrap.TurretHeadArchetype);
        Entity turretGun1 = entityManager.CreateEntity(Bootstrap.TurretGun1Archetype);
        Entity turretGun2 = entityManager.CreateEntity(Bootstrap.TurretGun2Archetype);

        Matrix4x4 world = trans;

        entityManager.SetComponentData(turretBody, new TransformMatrix {Value = world});
        entityManager.SetComponentData(turretBody, new Position {Value = position});
        entityManager.SetComponentData(turretBody, new Rotation {Value = quaternion.identity});
        entityManager.AddSharedComponentData(turretBody, Bootstrap.TurretBodyLook);

        entityManager.SetComponentData(turretHead, new TransformMatrix {Value = world});
        entityManager.SetComponentData(turretHead, new LocalPosition {Value = new Vector3(0.0f, 0.6128496f, 0.0f)});
        entityManager.SetComponentData(turretHead, new LocalRotation {Value = quaternion.identity});
        entityManager.SetComponentData(turretHead, new TransformParent {Value = turretBody});
        entityManager.AddSharedComponentData(turretHead, Bootstrap.TurretHeadLook);

        entityManager.SetComponentData(turretGun1, new TransformMatrix {Value = Matrix4x4.identity});
        entityManager.SetComponentData(turretGun1, new TransformParent {Value = turretHead});
        entityManager.SetComponentData(turretGun1, new LocalPosition {Value = new Vector3(0.08563034f, 0.08383693f, 0.327976f)});
        entityManager.SetComponentData(turretGun1, new LocalRotation {Value = quaternion.identity});
        entityManager.AddSharedComponentData(turretGun1, Bootstrap.TurretGun1Look);

        entityManager.SetComponentData(turretGun2, new TransformMatrix {Value = Matrix4x4.identity});
        entityManager.SetComponentData(turretGun2, new TransformParent {Value = turretHead});
        entityManager.SetComponentData(turretGun2, new LocalPosition {Value = new Vector3(-0.08563034f, 0.08383693f, 0.327976f)});
        entityManager.SetComponentData(turretGun2, new LocalRotation {Value = quaternion.identity});
        entityManager.AddSharedComponentData(turretGun2, Bootstrap.TurretGun2Look);
    }
}
