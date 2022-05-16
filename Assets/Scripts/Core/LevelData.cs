using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Враги")]
    public CharacterInfo[] Enemies;

    [Header("Враг снайпер")]
    public CharacterInfo[] EnemiesSniper;
    
    [Header("Союзники")] 
    public CharacterInfo[] Allies;

    [Header("Игрок")]
    public CharacterInfo Player;

    [Header("Таймер")]
    public int LevelTimer;
}

[Serializable]
public class CharacterInfo
{
    public Transform Character;
    public Vector3 Position;
    public Vector3 Rotation;
} 
