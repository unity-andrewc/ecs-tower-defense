using ComponentTypes;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
            
            var movePosition = new float3(data.x - 0.01f
                , data.y
                , data.z);

            data = movePosition;
            
            m_State.EnemyPositions[i] = new Position {Value = data};
        }
    }
    
}
