using System;
using System.Collections;
using System.Collections.Generic;
using JBStateMachine;
using UnityEngine;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEditor;
using UnityEngine.Events;

public class PlayerAttackState : MonoBehaviour, IStateController
{
    public FloatingJoystick _joystick;
    
    public Animator HookAnimator;
    public HookController HookController;
    public GameObject Hook;
    public Vector3 HookStartPos;
    public float HookDistance;
    
    private PlayerController _playerController;

    public UnityEvent SomethingHooked;
    
    private Coroutine _doHookCoroutine;
    private Sequence hookStartSequence;
    private Sequence hookEndSequence;
    
    public Animator playerAnimator;

    public GameObject HookFlyEffect;

    private LeaderBoardController leaderBoardController;

    public GameObject CrownGO;
    
    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        HookStartPos = Hook.transform.localPosition;
        
        SomethingHooked.AddListener(OnSomethingHooked);
    }

    public void ResetHook()
    {
        hookStartSequence.Kill(true);
        hookEndSequence.Kill(true);
        Hook.transform.DOLocalMove(HookStartPos, 0);
    }

    void OnSomethingHooked()
    {
        if(GameRoot.Instance.IsGameFinished)
            return;

        if(_doHookCoroutine != null)
            StopCoroutine(_doHookCoroutine);
        
        if (hookStartSequence != null)
            hookStartSequence.Kill();
        
        
        if (GameRoot.Instance.CanVibration())
        {
            MMVibrationManager.SetHapticsActive(true);
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
        }
        
        GameRoot.Instance.ShakeCamera();
        
        HookAnimator.enabled = false;
        HookController.transform.localRotation = Quaternion.Euler(0,0,0);

        playerAnimator.SetTrigger("HookEnd");
        
        hookEndSequence = DOTween.Sequence();
        hookEndSequence.AppendInterval(GameRoot.Instance.GameSettings.HookImpactDelay).Append( 
        Hook.transform.DOLocalMove(HookStartPos, GameRoot.Instance.GameSettings.EndHookTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            if(GameRoot.Instance.IsGameFinished)
                return;

            if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
                playerAnimator.SetTrigger("Walk");
            else
                playerAnimator.SetTrigger("Idle");
            
            if (HookController.Target != null)
            {
                var spawnRagdoll = Instantiate(GameRoot.Instance.GameSettings.Ragdoll);
                var ragdollController = spawnRagdoll.GetComponent<RagdollController>();
                ragdollController.SetData(HookController.Target.transform);
                ragdollController.ActiveWithDelay(new Vector3(0,1,-1));
                
                _playerController.CharacterData.Score++;
                leaderBoardController.PlayerScoreChangedEvent.Invoke(_playerController.CharacterData);

                var botController = HookController.Target.GetComponent<BotController>();
                if (botController != null)
                {
                    var deathEffect = Instantiate(GameRoot.Instance.GameSettings.DeathEffectEnemy);
                    deathEffect.transform.position = botController.transform.position;
                    deathEffect.SetActive(true);

                    botController.Fire(BotController.BotTriggers.BotDieTrigger);
                    botController.RootGO.SetActive(false);
                }
                
                //Destroy(HookController.Target);
                HookController.Target = null;
            }

            if (leaderBoardController.IsFirstPlace(_playerController.CharacterData))
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
                
            _playerController.Fire(PlayerController.PlayerTriggers.PlayerInputTrigger);
        }));
    }

    public void OnEntered(EnterDataBase data)
    {
        if (leaderBoardController == null)
            leaderBoardController = GameRoot.Instance.LeaderBoardController;


        var needClearChilds = HookController.GetComponentsInChildren<BotController>();
        if (needClearChilds.Length > 0)
        {
            foreach (var bot in needClearChilds)
            {
                bot.transform.SetParent(null);
                //bot.Fire(BotController.BotTriggers.BotRespawnedTrigger);
            }
        }
        
        
        _joystick = GameRoot.Instance.Joystick;
        
        if(_doHookCoroutine != null)
            StopCoroutine(_doHookCoroutine);
        
        _doHookCoroutine = StartCoroutine(DoHook());
    }

    IEnumerator DoHook()
    {
        if(GameRoot.Instance.IsGameFinished)
            yield break;
        
        playerAnimator.SetTrigger("Hook");
           
        HookController.CanHook = true;
        Hook.transform.localPosition = HookStartPos;
        
        yield return new WaitForSeconds(0.25f);

        HookAnimator.enabled = true;

        HookFlyEffect.SetActive(true);
        
        hookStartSequence = DOTween.Sequence();
        hookStartSequence.Insert(0, Hook.transform.DOLocalMoveZ(Hook.transform.localPosition.z + GameRoot.Instance.GameSettings.HookDistance, GameRoot.Instance.GameSettings.StartHookTime)).OnComplete(
            () =>
            {
                if (HookController.Target == null && !GameRoot.Instance.IsGameFinished)
                {
                    playerAnimator.SetTrigger("HookEnd");
                    playerAnimator.SetTrigger("HookIdle");
            
                    Hook.transform.DOLocalMove(HookStartPos, GameRoot.Instance.GameSettings.EndHookTime).OnComplete(() =>
                    {
                        if (!GameRoot.Instance.IsGameFinished)
                        {
                            if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
                                playerAnimator.SetTrigger("Walk");
                            else
                                playerAnimator.SetTrigger("Idle");
                        }
                        
                        HookFlyEffect.SetActive(false);
                        
                        HookAnimator.enabled = false;
                        HookController.transform.localRotation = Quaternion.Euler(0,0,0);
                        
                        HookController.CanHook = false;
                        _playerController.Fire(PlayerController.PlayerTriggers.PlayerInputTrigger);
                    });
                }
            });
    }
    
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;
    //     float theta = 0;
    //     float x = GameSettings.Instance.HookDistance * Mathf.Cos(theta);
    //     float y = GameSettings.Instance.HookDistance * Mathf.Sin(theta);
    //     Vector3 pos = transform.position + new Vector3(x, 0, y);
    //     Vector3 newPos = pos;
    //     Vector3 lastPos = pos;
    //     for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
    //     {
    //         x = GameSettings.Instance.HookDistance * Mathf.Cos(theta);
    //         y = GameSettings.Instance.HookDistance * Mathf.Sin(theta);
    //         newPos = transform.position + new Vector3(x, 0, y);
    //         Gizmos.DrawLine(pos, newPos);
    //         pos = newPos;
    //     }
    //     Gizmos.DrawLine(pos, lastPos);
    // }

    
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
