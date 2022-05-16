using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JBStateMachine;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;

public class EndGameState : MonoBehaviour, IStateController
{
    [Header("UI Side")] 
    public GameObject IngamePanel;
    public GameObject ResultPanel;

    public GameObject WinPanel;
    public GameObject LosePanel;
    
    public TMP_Text WinPlaceLabel;

    public GameObject DescriptionLabel;
    public TMP_Text LosePlaceLabel;

    private bool IsWin;
    public GameObject BlockPanel;
    
    public void OnEntered(EnterDataBase data)
    {
        GameRoot.Instance.Joystick.enabled = false;
        GameRoot.Instance.Joystick.gameObject.SetActive(false);
        
        BlockPanel.SetActive(true);

        var hooks = FindObjectsOfType<HookController>();
        foreach (var item in hooks)
        {
            item.ResetPositions();
            item.enabled = false;
        }

        GameRoot.Instance.IsGameFinished = true;
        GameRoot.Instance.GameFinished.Invoke();
        GameRoot.Instance.CanFinishPos = true;

        var leaderBoardController = GameRoot.Instance.LeaderBoardController;
        var playerPlace = leaderBoardController.GetPlayerPlace();

        var playerData = leaderBoardController.CurrentData.FirstOrDefault(x => x.IsPlayer);
        
        if (playerPlace == 0 && playerData.Score != 0)
        {
            IsWin = true;
            if (GameRoot.Instance.CanVibration())
            {
                MMVibrationManager.SetHapticsActive(true);
                MMVibrationManager.Haptic(HapticTypes.Success);
            }
        }
        else
        {
            IsWin = false;
            if (GameRoot.Instance.CanVibration())
            {
                MMVibrationManager.SetHapticsActive(true);
                MMVibrationManager.Haptic(HapticTypes.Warning);
            }
        }
        
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        var character = FindObjectOfType<PlayerController>();
        character.PlayerAttackState.ResetHook();

        if (character.HookController.Target != null)
        {
            character.HookController.Target.SetActive(false);
        }
        
        while (!character.RootGO.gameObject.activeInHierarchy)
            yield return null;

        character.transform.rotation = Quaternion.Euler(0,180f,0);
     
        if (IsWin)
        {
            character.PlayerInputState.playerAnimator.SetTrigger("Win");
            character.PlayerInputState.playerAnimator.Play("Win");
        }
        else
        {
            character.PlayerInputState.playerAnimator.SetTrigger("Lose");
            character.PlayerInputState.playerAnimator.Play("Lose");
        }
        
        yield return new WaitForSeconds(GameRoot.Instance.GameSettings.EndPauseDelay);
        
        var leaderBoardController = GameRoot.Instance.LeaderBoardController;
        var playerPlace = leaderBoardController.GetPlayerPlace();

        var playerData = leaderBoardController.CurrentData.FirstOrDefault(x => x.IsPlayer);
        
        IngamePanel.SetActive(false);
        ResultPanel.SetActive(true);
        
        if (IsWin)
        {
            GameRoot.Instance.SendLevelFinished(true,1);
            WinPanel.SetActive(true);
        }
        else
        {
            GameRoot.Instance.SendLevelFinished(false,playerPlace + 1);
            DescriptionLabel.SetActive(playerData.Score != 0);
            LosePlaceLabel.text = playerData.Score != 0 ? "Place " + (playerPlace + 1).ToString() + "!" : "0 Kills!";
            LosePanel.SetActive(true);
        }
    }
    
    public ExitDataBase OnExited()
    {
        return new ExitData()
        {
           
        };
    }
    

    public class ExitData : ExitDataBase
    {
  
    }

    public class EnterData : EnterDataBase
    {
        public bool IsWin;
        public int Score;
    }
}
