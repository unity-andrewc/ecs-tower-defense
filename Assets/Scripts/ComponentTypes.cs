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
		public Vector3 Translation;
		
		public float TargetAngleRadians;
		public float AngleRadians;
	}

	public struct TurretGun1State : IComponentData
	{
	}

	public struct TurretGun2State : IComponentData
	{
	}
}
