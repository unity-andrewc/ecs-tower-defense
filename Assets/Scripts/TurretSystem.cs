using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;

public class TurretSystem : ComponentSystem
{
    public struct Data
    {
        public int Length;
        public ComponentDataArray<TransformMatrix> Transforms;
        public ComponentDataArray<ComponentTypes.TurretHeadState> TurretHeadState;
    }

    [Inject]
    private Data m_data; 

    protected override void OnUpdate()
    {
        for (int idx = 0; idx < m_data.Length; ++idx)
        {
            ComponentTypes.TurretHeadState temp = m_data.TurretHeadState[idx];

            if (temp.AngleRadians > 360.0f)
            {
                temp.AngleRadians -= 360.0f;
            }

            if (temp.AngleRadians < 0.0f)
            {
                temp.AngleRadians += 360.0f;
            }

            float delta = temp.TargetAngleRadians - temp.AngleRadians;
            float absDelta = Mathf.Abs(delta);

            if (absDelta < 1.0f)
            {
                temp.TargetAngleRadians = Random.Range(0, 360.0f);
            }

            temp.AngleRadians += Mathf.Sign(delta) * (45.0f * Time.deltaTime);

            m_data.TurretHeadState[idx] = temp;


            TransformMatrix tempMatrix = m_data.Transforms[idx];

            Matrix4x4 rot = Matrix4x4.Rotate(Quaternion.Euler(0.0f, temp.AngleRadians, 0.0f));
            Matrix4x4 trans = Matrix4x4.Translate(temp.Translation);

            rot = trans * rot;

            m_data.Transforms[idx] = new TransformMatrix {Value = rot};
        }
    }
}
