using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using JBStateMachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("States")] 
    public PlayerInputState PlayerInputState;
    public PlayerAttackState PlayerAttackState;
    public PlayerHookedState PlayerHookedState;
    public PlayerInitState PlayerInitState;
    public PlayerRespawnState PlayerRespawnState;
    
    public StateMachine<PlayerStates, PlayerTriggers> playerStateMachine;

    public int PlayerHealth;
    public bool IsHooked;

    public Vector3 SpawnPoint;
    public CharacterData CharacterData;
    public GameObject RootGO;
    public HookController HookController;
    
    void Start()
    {
        var camera = FindObjectOfType<CinemachineVirtualCamera>();
        if (camera != null)
            camera.Follow = transform;
        
        GameRoot.Instance.GameFinished.AddListener(() =>
        {
            Fire(PlayerTriggers.ExitTrigger);
        });
        
        playerStateMachine = new StateMachine<PlayerStates, PlayerTriggers>(PlayerStates.Default);

        playerStateMachine.Configure(PlayerStates.Default, null)
            .Permit(PlayerTriggers.PlayerDefaultTrigger, PlayerStates.PlayerInit);

        playerStateMachine.Configure(PlayerStates.PlayerInit, PlayerInitState)
            .Permit(PlayerTriggers.PlayerInitializedTrigger, PlayerStates.PlayerInput);
        
        playerStateMachine.Configure(PlayerStates.PlayerInput, PlayerInputState)
            .Permit(PlayerTriggers.PlayerAttackTrigger, PlayerStates.PlayerAttack)
            .Permit(PlayerTriggers.PlayerIsHooked, PlayerStates.PlayerHooked);

        playerStateMachine.Configure(PlayerStates.PlayerAttack, PlayerAttackState)
            .Permit(PlayerTriggers.PlayerInputTrigger, PlayerStates.PlayerInput)
            .Permit(PlayerTriggers.PlayerIsHooked, PlayerStates.PlayerHooked);
        
        playerStateMachine.Configure(PlayerStates.PlayerHooked, PlayerHookedState)
            .Permit(PlayerTriggers.PlayerDieTrigger, PlayerStates.PlayerRespawn);

        playerStateMachine.Configure(PlayerStates.PlayerRespawn, PlayerRespawnState)
            .Permit(PlayerTriggers.PlayerRespawnedTrigger, PlayerStates.PlayerInput);
        
        playerStateMachine.Configure(PlayerStates.ExitState, null);
        
        playerStateMachine.Fire(PlayerTriggers.PlayerDefaultTrigger);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            PlayerInputState.playerAnimator.SetTrigger("Win");
        
        
        if(Input.GetKeyDown(KeyCode.Alpha2))
            PlayerInputState.playerAnimator.SetTrigger("Lose");
    }

    public void Fire(PlayerTriggers trigger)
    {
        playerStateMachine.Fire(trigger);
    }
    
    public enum PlayerTriggers
    {
        PlayerDefaultTrigger,
        PlayerInitializedTrigger,
        PlayerInputTrigger,
        PlayerAttackTrigger,
        PlayerIsHooked,
        ExitTrigger,
        PlayerDieTrigger,
        PlayerRespawnedTrigger,
    }
    
    public enum PlayerStates
    {
        Default,
        PlayerInit,
        PlayerInput,
        PlayerAttack,
        DieState,
        PlayerHooked,
        PlayerRespawn,
        ExitState,
    }
}
