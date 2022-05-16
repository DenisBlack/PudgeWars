using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Data/Character Data")]
public class CharacterData : ScriptableObject
{
    public string NickName;
    public int Score;
    public bool IsPlayer;
}
