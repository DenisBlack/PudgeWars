using System.Collections;
using System.Collections.Generic;
using JBStateMachine;
using UnityEngine;

public class BotRespawnState : MonoBehaviour, IStateController
{
    private BotController _botController;

    public GameObject SpawnEffect;
    
    public void OnEntered(EnterDataBase data)
    {
        _botController = transform.GetComponent<BotController>();
        StartCoroutine(DoRespawn());
    }

    IEnumerator DoRespawn()
    {
        yield return new WaitForSeconds(GameRoot.Instance.GameSettings.RespawnDelay);
        
        SpawnEffect.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        
        transform.SetParent(null);
        _botController.RootGO.SetActive(true);
        _botController.IsHooked = false;
        transform.position = _botController.SpawnPoint;
        
        _botController.Fire(BotController.BotTriggers.BotRespawnedTrigger);
    }
    
    public ExitDataBase OnExited()
    {
        return new BotRespawnState.ExitData()
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