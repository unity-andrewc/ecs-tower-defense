using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;

namespace ComponentTypes
{
    public struct TurretState : IComponentData
    {
        public float AngleRadians;
    }
}
