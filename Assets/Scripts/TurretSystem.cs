using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms2D;

public class TurretSystem : ComponentSystem
{
    public struct Data
    {
        public int Length;
        public ComponentDataArray<LocalRotation> LocalRotation;
        public ComponentDataArray<ComponentTypes.TurretHeadState> State;
    }

    [Inject]
    private Data m_data; 

    protected override void OnUpdate()
    {
        for (int idx = 0; idx < m_data.Length; ++idx)
        {
            ComponentTypes.TurretHeadState tempState = m_data.State[idx];

            // Wrap the rotation angle to keep between 0-360.
            if (tempState.Angle > 360.0f)
                tempState.Angle -= 360.0f;

            if (tempState.Angle < 0.0f)
                tempState.Angle += 360.0f;

            float delta = tempState.TargetAngle - tempState.Angle;

            // Determine if near target location.
            if (Mathf.Abs(delta) < 1.0f)
            {
                tempState.TargetAngle = Random.Range(0, 360.0f);
            }

            // Rotate towards target angle.
            tempState.Angle += Mathf.Sign(delta) * (45.0f * Time.deltaTime);

            m_data.State[idx] = tempState;

            LocalRotation tempRotation = m_data.LocalRotation[idx];
            tempRotation.Value = Quaternion.Euler(0.0f, tempState.Angle, 0.0f);
            m_data.LocalRotation[idx] = tempRotation;
        }
    }
}