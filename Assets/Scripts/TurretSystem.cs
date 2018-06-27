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
        public ComponentDataArray<TransformParent> Parent;
        public ComponentDataArray<LocalRotation> LocalRotation;
        public ComponentDataArray<ComponentTypes.TurretHeadState> State;
    }

    public struct Enemies
    {
        public int Length;
        public ComponentDataArray<Position> EnemyPositions;
        public ComponentDataArray<ComponentTypes.Enemy> EnemyState;
        
    }

    [Inject]
    private Data m_data; 

    [Inject]
    private Enemies m_enemies;

    protected override void OnUpdate()
    {
        EntityManager manager = World.Active.GetOrCreateManager<EntityManager>();

        for (int idx = 0; idx < m_data.Length; ++idx)
        {
            float nearestDistance = float.MaxValue;
            Vector3 nearestToEnemy = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 nearestPosition = new Vector3(0.0f, 0.0f, 0.0f);

            bool nearestFound = false;

            Entity turretBody = m_data.Parent[idx].Value;
            Position position = manager.GetComponentData<Position>(turretBody);

            for (int enemyIdx = 0; enemyIdx < m_enemies.Length; ++enemyIdx)
            {
                Vector3 toEnemy = m_enemies.EnemyPositions[enemyIdx].Value - position.Value;
                
                if (toEnemy.magnitude < nearestDistance && toEnemy.magnitude < 10.0f)
                {
                    nearestDistance = toEnemy.magnitude;
                    nearestPosition = m_enemies.EnemyPositions[enemyIdx].Value;
                    nearestToEnemy = toEnemy;
                    nearestFound = true;
                }
            }

            ComponentTypes.TurretHeadState tempState = m_data.State[idx];

            float delta = tempState.TargetAngle - tempState.Angle;

            float rotSpeed = 45.0f;
            if (nearestFound)
            {
                nearestToEnemy.Normalize();
                float dot = Vector3.Dot(new Vector3 (0.0f, 0.0f, 1.0f), nearestToEnemy);
                Vector3 cross = Vector3.Cross(new Vector3 (0.0f, 0.0f, 1.0f), nearestToEnemy);

                tempState.TargetAngle = Mathf.Rad2Deg * Mathf.Acos(dot);

                if (cross.y < 0.0f)
                {
                    tempState.TargetAngle = 360.0f - tempState.TargetAngle;           
                }

                rotSpeed = 180.0f;

                Vector3 basis = new Vector3(0.0f, 0.0f, 1.0f);
                Vector3 angleVector = Quaternion.Euler(0.0f, tempState.Angle, 0.0f) * basis;
                Vector3 targetVector = Quaternion.Euler(0.0f, tempState.TargetAngle, 0.0f) * basis;

                Vector3 angleTargetCross = Vector3.Cross(angleVector, targetVector);
                angleTargetCross.Normalize();

                if (angleTargetCross.y < 0.0f)
                {
                    tempState.Angle -= Time.deltaTime * rotSpeed;
                    if (tempState.Angle < tempState.TargetAngle)
                        tempState.Angle = tempState.TargetAngle;
                }
                else
                {
                    tempState.Angle += Time.deltaTime * rotSpeed;
                    if (tempState.Angle > tempState.TargetAngle)
                        tempState.Angle = tempState.TargetAngle;
                }
            }


            LocalRotation tempRotation = m_data.LocalRotation[idx];
            tempRotation.Value = Quaternion.Euler(0.0f, tempState.Angle, 0.0f);
            m_data.LocalRotation[idx] = tempRotation;
            m_data.State[idx] = tempState;
        }
    }
}