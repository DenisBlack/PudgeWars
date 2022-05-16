using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/GameSettings")]
public class GameSettings : SingletonScriptableObject<GameSettings>
{
    [Header("Дебаг")] 
    public DebugSettings DebugSettings;
    
    [Header("Настройки Персонажа")] 
    public float PlayerMoveSpeed;
    public float PlayerRotationSpeed;
    
    [Header("Настройки Ботов")] 
    public float BotMoveSpeed;
    public float BotRotationSpeedToHook;
   
    [Header("Настройки Хука")] 
    public float HookDistance;
    public float StartHookTime;
    public float EndHookTime;
    [Tooltip("Задержка хука перед тем как мы тянем на себя")]
    public float HookImpactDelay;

    [Header("Левела")] 
    public LevelsSetting LevelSettings;

    [Header("Префабы")] 
    public Transform PlayerPrefab;
    public Transform EnemyPrefab;
    public Transform SniperPrefab;
    public Transform AlliesPrefab;

    [Header("Ники")] 
    public TextAsset NickNames;

    [Header("Инфо персонажа")] 
    public CharacterData CharacterData;

    [Header("Респавн Делей")] 
    public float RespawnDelay;

    [Header("Єффектики")] 
    public GameObject ImpactEffect;
    public GameObject DeathEffectAlly;
    public GameObject DeathEffectEnemy;
    public GameObject ArrowPrefab;
    public Transform Ragdoll;
    [Header("Шейк камеры")] 
    public float IntensityShake;
    public float FrequencyShake;
    public float TimeShake;

    [Header("Камера в конце")] 
    public float EndCameraSpeed = 7f;

    [Header("Пауза на финальных анимациях - перед юайкой")]
    public float EndPauseDelay = 3f;

    [Header("Ragdoll Настройки")] 
    public float TimeToSpawnRagdoll = 0.5f;
    public float RagdollTimerToEnd = 2f;
    public float RagdollTimerToDespawn = 2f;
    public float RagdollSpeedHide = 5f;

}

[System.Serializable]
public class DebugSettings
{
    public bool Enabled = false;
    public int Level;
    public bool Loop = true;
}
