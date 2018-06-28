using ComponentTypes;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EndGameSystem : ComponentSystem
{    
    public struct PlayerData
    {
        public int Length;
        public ComponentDataArray<ComponentTypes.PlayerSessionData> Player;
    }
    [Inject] PlayerData m_PlayerData;
    
    struct PositionState
    {
        public int Length;
        public EntityArray Entities;
        public ComponentDataArray<Position> PositionEntities;
    }

    [Inject] PositionState m_PosEntities;
    
    protected override void OnUpdate()
    {
        if (m_PlayerData.Length <= 0)
        {
            return;
        }

        if (m_PlayerData.Player[0].Health <= 0)
        {
//            for (int i = 0; i < m_PosEntities.Length; i++)
//            {
//                var entity = m_PosEntities.Entities[i];
//                PostUpdateCommands.DestroyEntity(entity);
//            }
//            
            var playerData = m_PlayerData.Player[0];
            playerData.gameState = GameState.END_GAME;
            playerData.Health = 10;
            playerData.Score = 0;
            playerData.CurrencyAmount = 10;
            m_PlayerData.Player[0] = playerData;
        }

        
    }

}
