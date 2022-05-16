using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JBStateMachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BotAttackState : MonoBehaviour, IStateController
{
    public Animator HookAnimator;
    public HookController HookController;
    public GameObject Hook;
    public Vector3 HookStartPos;
    public float HookDistance;

    private BotController _botController;
    public UnityEvent SomethingHooked;
    private Sequence hookStartSequence;
    private Animator _animator;
    
    private Coroutine _doHookCoroutine;
    
    public GameObject HookFlyEffect;
    
    public GameObject CrownGO;
    
    private void Start()
    {
        SomethingHooked.AddListener(OnSomethingHooked);
    }
    
    void OnSomethingHooked()
    {
        if (hookStartSequence != null)
            hookStartSequence.Kill();
        
        _animator.SetTrigger("HookEnd");
        
        HookAnimator.enabled = false;
        HookController.transform.localRotation = Quaternion.Euler(0,0,0);
        
        Hook.transform.DOLocalMove(HookStartPos, GameRoot.Instance.GameSettings.EndHookTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            _animator.SetTrigger("Walk");
            
            if (HookController.Target != null)
            {
                _botController.CurrentCharacterData.Score++;
                GameRoot.Instance.LeaderBoardController.PlayerScoreChangedEvent.Invoke(_botController.CurrentCharacterData);
                
                var spawnRagdoll = Instantiate(GameRoot.Instance.GameSettings.Ragdoll);
                var ragdollController = spawnRagdoll.GetComponent<RagdollController>();
                ragdollController.SetData(HookController.Target.transform);
                ragdollController.IsActive = true;
                Vector3 force = _botController.IsAlliace == true ? new Vector3(0, 1, -1) : new Vector3(0, 1, 1);
                ragdollController.AddForceAtPoint(force);
                
                var botController = HookController.Target.GetComponent<BotController>();
                if (botController != null)
                {
                    var currentEffect = botController.IsAlliace;
                    
                    var deathEffect = Instantiate(currentEffect == true ? GameRoot.Instance.GameSettings.DeathEffectAlly : GameRoot.Instance.GameSettings.DeathEffectEnemy);
                    deathEffect.transform.position = botController.transform.position;
                    deathEffect.SetActive(true);
                    
                    botController.Fire(BotController.BotTriggers.BotDieTrigger);
                    botController.RootGO.SetActive(false);
                    botController.transform.SetParent(null);
                }
                
                var playerController = HookController.Target.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    var deathEffect = Instantiate(GameRoot.Instance.GameSettings.DeathEffectAlly);
                    deathEffect.transform.position = playerController.transform.position;
                    deathEffect.SetActive(true);
                    
                    playerController.Fire(PlayerController.PlayerTriggers.PlayerDieTrigger);
                    playerController.RootGO.SetActive(false);
                    playerController.transform.SetParent(null);
                }
                
                //Destroy(HookController.Target);
                HookController.Target = null;
            }
                 
            if ( GameRoot.Instance.LeaderBoardController.IsFirstPlace(_botController.CurrentCharacterData))
            {
                var anotherCrown = GameObject.FindGameObjectsWithTag("Crown");
                foreach (var item in anotherCrown)
                    item.SetActive(false);
                
                CrownGO.SetActive(true);
            }
            else 
                CrownGO.SetActive(false);
            
            HookFlyEffect.SetActive(false);
            
            HookController.CanHook = false;
                
            _botController.Fire(BotController.BotTriggers.BotMoveTrigger);
        });
    }
    
    public void OnEntered(EnterDataBase data)
    {
        if(_animator == null)
            _animator = GetComponent<Animator>();
        
        if(_botController == null)
            _botController = GetComponent<BotController>();

        var agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 0f;
        
        var needClearChilds = HookController.GetComponentsInChildren<BotController>();
        if (needClearChilds.Length > 0)
        {
            foreach (var bot in needClearChilds)
            {
                bot.transform.SetParent(null);
                //bot.Fire(BotController.BotTriggers.BotRespawnedTrigger);
            }
        }
        
        var needClearChildsPlayer = HookController.GetComponentsInChildren<PlayerController>();
        if (needClearChildsPlayer.Length > 0)
        {
            foreach (var bot in needClearChildsPlayer)
            {
                bot.transform.SetParent(null);
                //bot.Fire(BotController.BotTriggers.BotRespawnedTrigger);
            }
        }
        
        HookController.enabled = true;
        HookStartPos = Hook.transform.localPosition;

        // var randRotateValue =  !_botController.IsAlliace ? Random.Range(140f, 200f) : Random.Range(-22f,40f);
        // transform.DORotate(new Vector3(0, randRotateValue, 0), GameRoot.Instance.GameSettings.BotRotationSpeedToHook).OnComplete(() =>
        // {
        //     DoHook();
        // });
        
        DoHook();
    }
    
    private void DoHook()
    {
        _animator.SetTrigger("Hook");

        HookController.CanHook = true;
        Hook.transform.localPosition = HookStartPos;

        //_animator.SetTrigger("HookIdle");
        
        HookAnimator.enabled = true;
        
        HookFlyEffect.SetActive(true);
        
        hookStartSequence = DOTween.Sequence();
        hookStartSequence.Insert(0, Hook.transform.DOLocalMoveZ(Hook.transform.localPosition.z +GameRoot.Instance.GameSettings.HookDistance, GameRoot.Instance.GameSettings.StartHookTime)).OnComplete(
            () =>
            {
                if (HookController.Target == null)
                {
                    _animator.SetTrigger("HookEnd");
                    _animator.SetTrigger("HookIdle");
                    
                    Hook.transform.DOLocalMove(HookStartPos, GameRoot.Instance.GameSettings.EndHookTime).OnComplete(() =>
                    {
                        _animator.SetTrigger("Walk");
                        
                        HookFlyEffect.SetActive(false);
                        
                        HookAnimator.enabled = false;
                        HookController.transform.localRotation = Quaternion.Euler(0,0,0);

                        HookController.CanHook = false;
                        _botController.Fire(BotController.BotTriggers.BotMoveTrigger);
                    });
                }
            });
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
        public bool IsLookAtPlayer;
    }
}
