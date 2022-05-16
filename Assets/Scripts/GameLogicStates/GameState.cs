using System;
using System.Collections;
using System.Collections.Generic;
using JBStateMachine;
using TMPro;
using UnityEngine;

public class GameState : MonoBehaviour, IStateController
{
    private int startGameValue = 4;
    public TMP_Text LevelTimer;
    public TMP_Text StartGameLabel;
    
    private int levelTimerCount;
    private bool CanStart = false;
    public void OnEntered(EnterDataBase data)
    {
         var enterData = data as GameState.EnterData;
         var level = enterData.Level;
         var currentLevel = GameRoot.Instance.CurrentLevelData;

         levelTimerCount = currentLevel.LevelTimer;
         UpdateLevelTimer(levelTimerCount);

         StartCoroutine(DoLevelTimer());
    }

    IEnumerator DoLevelTimer()
    {
        StartGameLabel.gameObject.SetActive(true);
        
        while (startGameValue != 1)
        {
            startGameValue -= 1;
            StartGameLabel.text = startGameValue.ToString();
            yield return new WaitForSeconds(1f);
        }

        StartGameLabel.text = "Fight!";
        
        yield return new WaitForSeconds(1f);

        GameRoot.Instance.GameCanStarted = true;
        
        StartGameLabel.gameObject.SetActive(false);

        while (levelTimerCount != 0)
        {
            levelTimerCount -= 1;
            UpdateLevelTimer(levelTimerCount);
            yield return new WaitForSeconds(1f);
        }
        
        GameRoot.Instance.Fire(GameRoot.Triggers.CanFinishLevel);
        
        yield return null;
    }

    private void UpdateLevelTimer(int value)
    {
        TimeSpan time = TimeSpan.FromSeconds(value);
        LevelTimer.text = time.ToString(@"mm\:ss");
    }
    
    public ExitDataBase OnExited()
    {
        return new GameState.ExitData()
        {
           
        };
    }

    public class ExitData : ExitDataBase
    {
  
    }

    public class EnterData : EnterDataBase
    {
        public int Level;
    }
}