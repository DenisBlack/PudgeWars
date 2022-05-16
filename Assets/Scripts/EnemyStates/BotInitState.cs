using System.Collections;
using System.Collections.Generic;
using JBStateMachine;
using UnityEngine;

public class BotInitState : MonoBehaviour, IStateController
{
    private BotController _botController;
    public void OnEntered(EnterDataBase data)
    {
        _botController = transform.GetComponent<BotController>();
        StartCoroutine(DoWaitForStartGame());
    }

    IEnumerator DoWaitForStartGame()
    {
        while (!GameRoot.Instance.GameCanStarted)
            yield return null;
        
        _botController.Fire(BotController.BotTriggers.BotInitializedTrigger);
    }
    
    public ExitDataBase OnExited()
    {
        return new BotInitState.ExitData()
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
