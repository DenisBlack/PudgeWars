using System.Collections;
using System.Collections.Generic;
using JBStateMachine;
using UnityEngine;

public class PlayerRespawnState : MonoBehaviour, IStateController
{
    private PlayerController _playerController;
    public GameObject SpawnEffect;
    public void OnEntered(EnterDataBase data)
    {
        _playerController = transform.GetComponent<PlayerController>();
        StartCoroutine(DoRespawn());
    }

    IEnumerator DoRespawn()
    {
        yield return new WaitForSeconds(GameRoot.Instance.GameSettings.RespawnDelay);
        
        _playerController.GetComponent<CharacterController>().enabled = true;
        
        _playerController.transform.SetParent(null);
        transform.position = _playerController.SpawnPoint;

        SpawnEffect.SetActive(true);
        
        yield return new WaitForSeconds(0.2f);
        
        _playerController.RootGO.SetActive(true);
        _playerController.IsHooked = false;
        _playerController.HookController.enabled = true;
        
        GameRoot.Instance.Joystick.ResetJoystickPos();
        
        _playerController.Fire(PlayerController.PlayerTriggers.PlayerRespawnedTrigger);
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