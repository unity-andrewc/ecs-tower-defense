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
        public float AngleRadians;
    }

	public struct TurretHeadState : IComponentData
	{
	}

	public struct TurretGun1State : IComponentData
	{
	}

	public struct TurretGun2State : IComponentData
	{
	}
}
