using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Data/Levels Setting")]
public class LevelsSetting : ScriptableObject
{
    public LevelData[] _levels;
    
    public int Count => _levels.Length;
    public LevelData this[int level] => _levels[ level ];
    public LevelData GetLevel(int level)
    {
        return _levels[level < _levels.Length ? level : Random.Range(0, _levels.Length)];
    }
}
