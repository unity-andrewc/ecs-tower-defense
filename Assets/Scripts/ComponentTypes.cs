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
	
	public struct Enemy : IComponentData { }
	
	public struct EnemySpawnCooldown : IComponentData
	{
		public float Cooldown;
	}
}
