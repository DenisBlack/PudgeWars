using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LeaderBoardController : MonoBehaviour
{
    public List<CharacterData> CurrentData = new List<CharacterData>();
    
    public PlayerScoreChangedEvent PlayerScoreChangedEvent;

    public List<LeaderBoardPlaceController> PlaceControllers = new List<LeaderBoardPlaceController>();
    
    void Awake()
    {
        PlayerScoreChangedEvent.AddListener(OnPointEvent);

        var currentLevel = GameSettings.Instance.LevelSettings.GetLevel(GameRoot.Instance.CurrentLevel);
        var charactersCount = currentLevel.Allies.Length + currentLevel.Enemies.Length + 1;
         
    }

    public int GetPlayerPlace()
    {
        var sort = CurrentData.OrderBy(x => x.Score).Reverse().ToList();
        var playerData = sort.FirstOrDefault(x => x.IsPlayer);
        var place = sort.IndexOf(playerData);
        return place;
    }

    public bool IsFirstPlace(CharacterData data)
    {
        var sort = CurrentData.OrderBy(x => x.Score).Reverse().ToList();
        var place = sort.IndexOf(data);
        return place == 0 ? true : false;
    }
    
    private void OnPointEvent(CharacterData data)
    {
        Debug.Log(data);
        
        if(!CurrentData.Contains(data))
            CurrentData.Add(data);

        // if (CurrentData.Count > 0)
        // {
        //    
        //     var sort = CurrentData.OrderBy(x => x.Score).Reverse();
        //     for (int i = 0; i < 3; i++)
        //     {
        //         PlaceControllers[i].SetData(CurrentData[i]);
        //     } 
        // }
        // else PlaceControllers[0].SetData(CurrentData[i]);

        var sort = CurrentData.OrderBy(x => x.Score).Reverse().ToList();
        for (int i = 0; i < 3; i++)
        {
            PlaceControllers[i].SetData(sort[i]);
        }
    }

    public void UpdatePlaces()
    {
        var sort = CurrentData.OrderBy(x => x.Score).Reverse().ToList();
        for (int i = 0; i < 3; i++)
        {
            PlaceControllers[i].SetData(sort[i]);
        }
    }
}

[System.Serializable]
public class PlayerScoreChangedEvent: UnityEvent<CharacterData>
{
}


[Serializable]
public class PlaceInfo
{
    public Vector3 PanelPosition;
}
