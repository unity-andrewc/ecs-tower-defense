using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace ComponentTypes
{
    public struct TurretBodyState : IComponentData
    {
    }

	public struct TurretHeadState : IComponentData
	{
		public float TargetAngle;
		public float Angle;
		public Vector3 Translation;
		public float TimeSinceLastFire;
		public int CanFire;
	}

	public struct TurretGun1State : IComponentData
	{
		public Vector3 Translation;
	}

	public struct TurretGun2State : IComponentData
	{
		public Vector3 Translation;
	}

	public struct Enemy : IComponentData
    {
        public float Speed;
		public int Health;
    }

    public struct EnemyState : ISystemStateComponentData
    {
        public int PathId;
    }

	public struct EnemySpawn : IComponentData
	{
		public float Cooldown;
	}

	public struct WaveSpawn : IComponentData
	{
		public float Cooldown;
		public float SpawnedEnemyCount;
		public Random.State RandomState;
	}

    // there should only be one of these in any given level
    public struct EnemySpawnPoint : IComponentData
    {
        public int2 GridIndex;
    }
    
    // there should only be one of these in any given level
    public struct EnemyGoalPoint : IComponentData
    {
        public int2 GridIndex;
    }
	
	public struct InputState : IComponentData
	{
	}

	public struct MissileState : IComponentData
	{
		public float BirthTime;
	}

	public struct PlayerSessionData : IComponentData
	{
		public int CurrencyAmount;
		public int Score;
		public int Health;
		public GameState gameState;
	}

	public enum GameState
	{
		START,
		PLAYING,
		END_GAME
	}
}
