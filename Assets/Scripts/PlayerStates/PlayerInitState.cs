using System.Collections;
using System.Collections.Generic;
using JBStateMachine;
using UnityEngine;

public class PlayerInitState : MonoBehaviour, IStateController
{
    private PlayerController _playerController;
    public void OnEntered(EnterDataBase data)
    {
        _playerController = transform.GetComponent<PlayerController>();
        StartCoroutine(DoWaitForStartGame());
    }

    IEnumerator DoWaitForStartGame()
    {
        while (!GameRoot.Instance.GameCanStarted)
            yield return null;
        
        _playerController.Fire(PlayerController.PlayerTriggers.PlayerInitializedTrigger);
    }
    
    public ExitDataBase OnExited()
    {
        return new PlayerInitState.ExitData()
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
