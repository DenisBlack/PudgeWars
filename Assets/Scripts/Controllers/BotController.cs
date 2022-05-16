using System;
using System.Collections;
using System.Collections.Generic;
using JBStateMachine;
using UnityEngine;
using UnityEngine.AI;

public class BotController : MonoBehaviour
{
    [Header("States")] 
    public BotMoveState BotMoveState;
    public BotAttackState BotAttackState;
    public BotHookedState BotHookedState;
    public BotInitState BotInitState;
    public BotRespawnState BotRespawnState;
    
    public StateMachine<BotStates, BotTriggers> botStateMachine;

    public bool IsAlliace = false;

    public bool IsHooked;

    public Vector3 SpawnPoint;
    public CharacterData CurrentCharacterData;
    public NickNameController NickNameController;

    public GameObject RootGO;

    public bool IsSniper = false;
    
    void Start()
    {
        GameRoot.Instance.GameFinished.AddListener(() =>
        {
            Fire(BotTriggers.BotEndGameTrigger);
        });
        
        botStateMachine = new StateMachine<BotStates, BotTriggers>(BotStates.Default);

        botStateMachine.Configure(BotStates.Default, null)
            .Permit(BotTriggers.BotDefaultTrigger, BotStates.BotInit)
            .Permit(BotTriggers.BotEndGameTrigger, BotStates.BotEndGame);

        botStateMachine.Configure(BotStates.BotInit, BotInitState)
            .Permit(BotTriggers.BotInitializedTrigger, BotStates.BotMove)
            .Permit(BotTriggers.BotEndGameTrigger, BotStates.BotEndGame);

        botStateMachine.Configure(BotStates.BotMove, BotMoveState)
            .Permit(BotTriggers.BotAttackTrigger, BotStates.BotAttack)
            .Permit(BotTriggers.BotIsHooked, BotStates.BotHooked)
            .Permit(BotTriggers.BotEndGameTrigger, BotStates.BotEndGame)
            .Permit(BotTriggers.BotMoveTrigger, BotStates.BotMove);

        botStateMachine.Configure(BotStates.BotAttack, BotAttackState)
            .Permit(BotTriggers.BotMoveTrigger, BotStates.BotMove)
            .Permit(BotTriggers.BotIsHooked, BotStates.BotHooked)
            .Permit(BotTriggers.BotEndGameTrigger, BotStates.BotEndGame);

        botStateMachine.Configure(BotStates.BotHooked, BotHookedState)
            .Permit(BotTriggers.BotDieTrigger, BotStates.BotRespawn)
            .Permit(BotTriggers.BotEndGameTrigger, BotStates.BotEndGame);

        botStateMachine.Configure(BotStates.BotRespawn, BotRespawnState)
            .Permit(BotTriggers.BotRespawnedTrigger, BotStates.BotMove)
            .Permit(BotTriggers.BotEndGameTrigger, BotStates.BotEndGame);

        botStateMachine.Configure(BotStates.BotEndGame, null);
        
       botStateMachine.Fire(BotTriggers.BotDefaultTrigger);
    }

    public void Fire(BotTriggers trigger)
    {
       botStateMachine.Fire(trigger);
    }

    public void SetData(CharacterData data)
    {
        CurrentCharacterData = data;
        NickNameController.NickNameLabel.text = data.NickName;
    }
    
    public enum BotTriggers
    {
        BotDefaultTrigger,
        BotInitializedTrigger,
        BotMoveTrigger,
        BotAttackTrigger,
        BotIsHooked,
        BotDieTrigger,
        BotRespawnedTrigger,
        BotEndGameTrigger
    }
    
    public enum BotStates
    {
        Default,
        BotInit,
        BotMove,
        BotAttack,
        BotHooked,
        BotRespawn,
        BotEndGame
    }
}
