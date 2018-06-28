using ComponentTypes;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

[AlwaysUpdateSystem]
[UpdateAfter(typeof(EndGameSystem))]
public class UpdateHUDSystem : ComponentSystem
{
    struct EnemySpawnState
    {
        public int Length;
        public ComponentDataArray<EnemySpawn> Enemy;
    }
    
    struct WaveSpawnState
    {
        public int Length;
        public ComponentDataArray<WaveSpawn> Wave;
    }
    
    public struct PlayerData
    {
        public int Length;
        public ComponentDataArray<ComponentTypes.PlayerSessionData> Player;
    }

    [Inject] EnemySpawnState m_EnemySpawnState;
    [Inject] WaveSpawnState m_WaveSpawnState;
    [Inject] PlayerData m_PlayerData;

    private static Image waveFillImage;
    private static TextMeshProUGUI currencyAmount;
    private static TextMeshProUGUI playerScore;
    private static TextMeshProUGUI playerHealth;
    private static GameObject ingameMenu;
    private static GameObject mainMenu;

    public static void SetupGameObjects(EntityManager entityManager)
    {
        var buttonContainer = GameObject.Find("TurretButtons");
        for (int i = 0; i < buttonContainer.transform.childCount; i++)
        {
            var button = buttonContainer.transform.GetChild(i).GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    TurretButtonListener(entityManager);
                });
            }
        }
        
        var waveInfoContainer = GameObject.Find("WaveInfo");
        for (int i = 0; i < waveInfoContainer.transform.childCount; i++)
        {
            var image = waveInfoContainer.transform.GetChild(i).GetComponent<Image>();
            if (image != null)
            {
                waveFillImage = image.transform.GetChild(i).GetComponent<Image>();
                if (waveFillImage != null)
                {
                    waveFillImage.fillAmount = 1.0f;
                }
            }
        }
        
        var currencyInfoContainer = GameObject.Find("CurrencyInfo");
        for (int i = 0; i < currencyInfoContainer.transform.childCount; i++)
        {
            var text = currencyInfoContainer.transform.GetChild(i).GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                currencyAmount = text;
            }
        }
        
        var scoreInfoContainer = GameObject.Find("ScoreInfo");
        for (int i = 0; i < scoreInfoContainer.transform.childCount; i++)
        {
            var text = scoreInfoContainer.transform.GetChild(i).GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                playerScore = text;
            }
        }
        
        var healthInfoContainer = GameObject.Find("HealthInfo");
        for (int i = 0; i < healthInfoContainer.transform.childCount; i++)
        {
            var text = healthInfoContainer.transform.GetChild(i).GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                playerHealth = text;
            }
        }
        
        var playButton = GameObject.Find("PlayButton");
        ingameMenu = GameObject.Find("InGameMenu");
        mainMenu = GameObject.Find("MainMenu");
        if (playButton != null)
        {
            playButton.GetComponent<Button>().onClick.AddListener(()=>
            {
                Bootstrap.NewGame();
            });
        }
    }

    private static void TurretButtonListener(EntityManager entityManager)
    {
        var InputTurretBodyArchetype = entityManager.CreateArchetype(typeof(TransformMatrix), typeof(Position), typeof(Rotation), typeof(ComponentTypes.TurretBodyState), typeof(ComponentTypes.InputState));
        var InputTurretHeadArchetype = entityManager.CreateArchetype(typeof(TransformMatrix), typeof(LocalPosition), typeof(LocalRotation), typeof(TransformParent), typeof(ComponentTypes.InputState));
        var position = new float3(-9.5f, 0.0f, 0.5f);
        Matrix4x4 trans = Matrix4x4.Translate(position);
        Entity turretBody = entityManager.CreateEntity(InputTurretBodyArchetype);
        Entity turretHead = entityManager.CreateEntity(InputTurretHeadArchetype);
        
        entityManager.SetComponentData(turretBody, new TransformMatrix {Value = Matrix4x4.identity});
        entityManager.SetComponentData(turretBody, new Position {Value = position});
        entityManager.SetComponentData(turretBody, new Rotation {Value = quaternion.identity});
        entityManager.SetComponentData(turretBody, new InputState());
        entityManager.AddSharedComponentData(turretBody, Bootstrap.TurretBodyLook);

        entityManager.SetComponentData(turretHead, new TransformMatrix {Value = Matrix4x4.identity});
        entityManager.SetComponentData(turretHead, new LocalPosition {Value = new Vector3(0.0f, 0.6128496f, 0.0f)});
        entityManager.SetComponentData(turretHead, new LocalRotation {Value = quaternion.identity});
        entityManager.SetComponentData(turretHead, new TransformParent {Value = turretBody});
        entityManager.AddSharedComponentData(turretHead, Bootstrap.TurretHeadLook);
    }

    protected override void OnUpdate()
    {
        switch (m_PlayerData.Player[0].gameState)
        {
             case GameState.START:
                 mainMenu.gameObject.SetActive(true);
                 ingameMenu.gameObject.SetActive(false);
                 break;
             case GameState.PLAYING:
                 mainMenu.gameObject.SetActive(false);
                 ingameMenu.gameObject.SetActive(true);
                 break;
             case GameState.END_GAME:
                 mainMenu.gameObject.SetActive(true);
                 ingameMenu.gameObject.SetActive(false);
                 break;
        }
        
        if (m_WaveSpawnState.Length <= 0 || m_PlayerData.Length <= 0)
        {
            return;
        }
        
        var wave = m_WaveSpawnState.Wave[0];
        if (wave.SpawnedEnemyCount >= 3)
        {
            waveFillImage.fillAmount = wave.Cooldown / Constants.Enemy.WAVE_COOLDOWN;
        }
        else
        {
            waveFillImage.fillAmount = 1.0f;
        }

        currencyAmount.text = m_PlayerData.Player[0].CurrencyAmount.ToString();
        playerScore.text = m_PlayerData.Player[0].Score.ToString();
        playerHealth.text = m_PlayerData.Player[0].Health.ToString();
    }

}
