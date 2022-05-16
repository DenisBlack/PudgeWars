using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
public class LevelEditor : EditorWindow
{
    private int currentLevel;
    
    [MenuItem("Game/Level Editor")]
    static void Init()
    {
        var window = (LevelEditor) EditorWindow.GetWindow(typeof(LevelEditor));
        window.maxSize = new Vector2(250f, 200f);
        window.minSize = window.maxSize;
        window.Show();
    }

    void OnGUI()
    {
        var levelSettings = GetLevelsSettings();
        
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Level: ", GUILayout.Width(50));
            var nextLevel = EditorGUILayout.IntField(currentLevel, GUILayout.Width(50));
            if (nextLevel < 0 || nextLevel >= levelSettings.Count)
            {
                nextLevel = currentLevel;
            }
            if (GUILayout.Button("LOAD", GUILayout.Width(100)))
            {
                GUI.FocusControl(null);
                LoadLevel(nextLevel);
            }
        
            currentLevel = nextLevel;
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("<<", GUILayout.Width(50)))
            {
                GUI.FocusControl(null);
                if (currentLevel > 0)
                {
                    currentLevel--;
                    LoadLevel(currentLevel);
                }
            }
            if (GUILayout.Button(">>", GUILayout.Width(50)))
            {
                GUI.FocusControl(null);
                if (currentLevel < levelSettings.Count - 1)
                {
                    currentLevel++;
                    LoadLevel(currentLevel);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button("SAVE", GUILayout.Width(100)))
        {
            SaveCurrentToLevel(currentLevel);
        }
    }

    private void OnDestroy()
    {
        ClearPreviousSpawn();
    }

    private void LoadLevel(int level)
    {
         ClearPreviousSpawn();
        
        
         var levelData = GetLevelData(level);
         
         //SpawnEnemy
         var enemiesData = levelData.Enemies.ToList();
         foreach (var enemyData in enemiesData)
         {
             var prefab = GameSettings.Instance.EnemyPrefab;
             var container = Instantiate(prefab);
             container.transform.position = enemyData.Position;
             container.eulerAngles = new Vector3(0, enemyData.Rotation.y, 0);
         }
         
         //SpawnEnemy SNipers
         var enemiesSnipersData = levelData.EnemiesSniper.ToList();
         foreach (var enemyData in enemiesSnipersData)
         {
             var prefab = GameSettings.Instance.SniperPrefab;
             var container = Instantiate(prefab);
             container.transform.position = enemyData.Position;
             container.eulerAngles = new Vector3(0, enemyData.Rotation.y, 0);
         }
         
         //SpawnAllies
         var alliesData = levelData.Allies.ToList();
         foreach (var allyData in alliesData)
         {
             var prefab = GameSettings.Instance.AlliesPrefab;
             var container = Instantiate(prefab);
             container.transform.position = allyData.Position;
             container.eulerAngles = new Vector3(0, allyData.Rotation.y, 0);
         }
         
         //SpawnPlayer
         var playerData =  levelData.Player;
         var playerClone = Instantiate(GameSettings.Instance.PlayerPrefab);
         playerClone.transform.position = playerData.Position;
         playerClone.transform.rotation = Quaternion.Euler(0, playerData.Rotation.y,0);
    }
    
    private void SaveCurrentToLevel(int level)
    {
        var enemyGo = GameObject.FindObjectsOfType<BotController>().Where(x=> !x.IsAlliace && !x.IsSniper).ToArray();
        var enemySnipersGo = GameObject.FindObjectsOfType<BotController>().Where(x=> !x.IsAlliace && x.IsSniper).ToArray();
        var alliesGo = GameObject.FindGameObjectsWithTag("Allies");
        var playerGo = GameObject.FindWithTag("Player");
        var levelData = GetLevelData(level);

        levelData.Enemies = enemyGo.Select(x => new CharacterInfo()
        {
            Character = GameSettings.Instance.EnemyPrefab,
            Position = x.transform.position,
            Rotation = new Vector3(0,x.transform.eulerAngles.y,0)
        }).ToArray();
        
        levelData.EnemiesSniper = enemySnipersGo.Select(x => new CharacterInfo()
        {
            Character = GameSettings.Instance.SniperPrefab,
            Position = x.transform.position,
            Rotation = new Vector3(0,x.transform.eulerAngles.y,0)
        }).ToArray();

        levelData.Allies = alliesGo.Select(x => new CharacterInfo()
        {
            Character = GameSettings.Instance.AlliesPrefab,
            Position = x.transform.position,
            Rotation = new Vector3(0,x.transform.eulerAngles.y,0)
        }).ToArray();

        levelData.Player = new CharacterInfo()
        {
            Character = GameSettings.Instance.PlayerPrefab,
            Position = playerGo.transform.position,
            Rotation = new Vector3(0, playerGo.transform.eulerAngles.y, 0)
        };
        
        EditorUtility.SetDirty(levelData);
        AssetDatabase.SaveAssets();
    }

    private void ClearPreviousSpawn()
    {
        var spawnedEnemies = FindObjectsOfType<BotController>();
        foreach (var spawnedEnemy in spawnedEnemies)
            DestroyImmediate(spawnedEnemy.gameObject);

        var spawnPlayer = GameObject.FindWithTag("Player");
        DestroyImmediate(spawnPlayer);
    }
    
    private LevelData GetLevelData(int level)
    {
        return GetLevelsSettings()[ level ];
    }
    
    private LevelsSetting GetLevelsSettings()
    {
        return GameSettings.Instance.LevelSettings;
    }
}
