using System;
using System.Collections;
using System.Collections.Generic;
using JBStateMachine;
using UnityEngine;

public class BotHookedState : MonoBehaviour, IStateController
{
    public HookController HookController;
    public Animator _animator;
    public GameObject Circle;

    private BotController _botController;
    private void Start()
    {
      
    }

    public void OnEntered(EnterDataBase data)
    {
        if (_botController == null)
            _botController = GetComponent<BotController>();
        
        _botController.IsHooked = true;
        
        HookController.CanHook = false;
        HookController.enabled = false;
        
        Circle.gameObject.SetActive(false);
        _animator.SetTrigger("TakeHook");
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
     
    }
}
