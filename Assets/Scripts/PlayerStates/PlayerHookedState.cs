using System.Collections;
using System.Collections.Generic;
using JBStateMachine;
using MoreMountains.NiceVibrations;
using UnityEngine;

public class PlayerHookedState : MonoBehaviour, IStateController
{ 
    public HookController HookController;
    public Animator _animator;
    private PlayerController _playerController;

    public void OnEntered(EnterDataBase data)
    {
        if (GameRoot.Instance.CanVibration())
        {
            MMVibrationManager.SetHapticsActive(true);
            MMVibrationManager.Haptic(HapticTypes.Warning);
        }
        
        GameRoot.Instance.ShakeCamera();
        
        if (_playerController == null)
            _playerController = GetComponent<PlayerController>();

        _playerController.IsHooked = true;
        
        HookController.CanHook = false;
        HookController.enabled = false;
        
        _animator.SetTrigger("TakeHook");
        _animator.Play("Impact");
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
