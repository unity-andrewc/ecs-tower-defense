using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;

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
	
	public struct Input : IComponentData
	{
	}
}
