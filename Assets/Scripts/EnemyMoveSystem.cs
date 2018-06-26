using ComponentTypes;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EnemyMoveSystem : ComponentSystem
{    
    struct State
    {
        public int Length;
        public ComponentDataArray<Enemy> Enemies;
        public ComponentDataArray<TransformMatrix> EnemyMatrices;
    }

    [Inject] State m_State;

    protected override void OnUpdate()
    {
        for (int i = 0; i < m_State.Length; ++i)
        {
            var data = m_State.EnemyMatrices[i].Value;
//            Debug.Log("Transform Matrix 0: " + m_State.EnemyMatrices[i].Value.c0);
//            Debug.Log("Transform Matrix 1: " + m_State.EnemyMatrices[i].Value.c1);
//            Debug.Log("Transform Matrix 2: " + m_State.EnemyMatrices[i].Value.c2);
//            Debug.Log("Transform Matrix 3: " + m_State.EnemyMatrices[i].Value.c3);

            var movePosition = new float3(m_State.EnemyMatrices[i].Value.c3.x - 0.01f
                , m_State.EnemyMatrices[i].Value.c3.y
                , m_State.EnemyMatrices[i].Value.c3.z);
            
            // TODO : find a better way to get scale
            var scale = new float3(m_State.EnemyMatrices[i].Value.c0.x
                            , m_State.EnemyMatrices[i].Value.c1.y
                            , m_State.EnemyMatrices[i].Value.c2.z);

            data = Matrix4x4.TRS(movePosition, Quaternion.identity, scale);
            m_State.EnemyMatrices[i] = new TransformMatrix {Value = data};
//            Debug.Log("Transform Matrix 3 After: " + m_State.EnemyMatrices[i].Value.c3);
//            Debug.Log("Transform Matrix 3 After data: " + data.c3);
        }
    }
    
}
