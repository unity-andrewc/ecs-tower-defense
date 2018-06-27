using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

[AlwaysUpdateSystem]
public class UpdateHUDSystem : ComponentSystem
{
    public struct PlayerData
    {
        public int Length;
        [ReadOnly] public EntityArray Entity;
    }

    [Inject] PlayerData m_Players;

    public static void SetupGameObjects()
    {
        var buttonContainer = GameObject.Find("TurretButtons");
        for (int i = 0; i < buttonContainer.transform.childCount; i++)
        {
            buttonContainer.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(Bootstrap.InstanceTurret);
        }
    }

    protected override void OnUpdate()
    {
        // implement UI updates here
        // 
    }

}
