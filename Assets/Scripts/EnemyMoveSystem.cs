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
        public ComponentDataArray<Position> EnemyPositions;
    }

    [Inject] State m_State;

    protected override void OnUpdate()
    {
        for (int i = 0; i < m_State.Length; ++i)
        {
            var data = m_State.EnemyPositions[i].Value;
//            var data = m_State.EnemyMatrices[i].Value;
            
//            var c0vec3 = new Vector3(data.c0.x, data.c0.y, data.c0.z);
//            var c1vec3 = new Vector3(data.c1.x, data.c1.y, data.c1.z);
//            var c2vec3 = new Vector3(data.c2.x, data.c2.y, data.c2.z);
//            var c3vec3 = new Vector3(data.c3.x, data.c3.y, data.c3.z);
            
            // it might not be the right way to decompose the Matrix
            // below is an applied version of this answer -> https://answers.unity.com/questions/402280/how-to-decompose-a-trs-matrix.html
            
//            var movePosition = new float3(c3vec3.x - 0.01f
//                            , c3vec3.y
//                            , c3vec3.z);
//
//            var scale = new float3(c0vec3.magnitude
//                            , c1vec3.magnitude
//                            , c2vec3.magnitude);
//            
//            var rotation = Quaternion.LookRotation(c2vec3
//                            , c1vec3);
            
//            data = Matrix4x4.TRS(movePosition, rotation, scale);
            
            var movePosition = new float3(data.x - 0.01f
                , data.y
                , data.z);

            data = movePosition;
            
            m_State.EnemyPositions[i] = new Position {Value = data};
        }
    }
    
}
