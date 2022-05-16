using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using DG.Tweening;
using JBStateMachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class HookController : MonoBehaviour
{
    public LineRenderer LineRenderer;
    
    private bool canCheck;

    public float maxDistanceRay;

    private Vector3 startLiner;

    public bool CanHook;
    public GameObject Target;

    public PlayerAttackState _playerAttackState;
    public BotAttackState _botAttackState;
    
    public bool IsBot;

    public string ParentTag;

    private Vector3 origin;
    private Vector3 direction;
    private void Start()
    {
        startLiner = transform.localPosition;
        LineRenderer.SetPosition(0, startLiner);
        ParentTag = transform.root.tag;
    }

    private void Update()
    {
        LineRenderer.SetPosition(1, transform.localPosition);
    }

    public void RestLineRenderer()
    {
        LineRenderer.SetPosition(1,startLiner);
    }

    
    private void FixedUpdate()
    {
        RaycastHit hit;

        if (CanHook && Target == null)
        {
            Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            if (Physics.SphereCast(transform.position, 0.45f, transform.TransformDirection(Vector3.forward), out hit, 1.5f))
            {
                string tag = hit.collider.transform.tag;
                if(tag == ParentTag || tag == "Untagged")
                    return;
                
                switch (tag)
                {
                    case "Player":
                        
                        if(ParentTag == "Player" || ParentTag == "Allies")
                            return;
                        
                        var playerController = hit.transform.GetComponent<PlayerController>();
                        if(playerController.IsHooked)
                            return;
                        
                        var impactEffect1 = Instantiate(GameRoot.Instance.GameSettings.ImpactEffect);
                        impactEffect1.transform.position = playerController.transform.position;
                        impactEffect1.SetActive(true);
              
                        _botAttackState.SomethingHooked.Invoke();
                        Target = hit.collider.transform.gameObject;
                        hit.collider.transform.SetParent(transform, true);
                        
                        var joystick = FindObjectOfType<FloatingJoystick>();
                        if (joystick != null)
                        {
                            joystick.enabled = false;
                            joystick.gameObject.SetActive(false);
                        }
                    
                        hit.transform.GetComponent<Collider>().enabled = false;
                    
                     
                        if(playerController != null)
                            playerController.Fire(PlayerController.PlayerTriggers.PlayerIsHooked);

                        var playerAttackState = hit.transform.GetComponent<PlayerAttackState>();
                        if (playerAttackState != null)
                        {
                            playerAttackState.HookController.RestLineRenderer();
                            playerAttackState.ResetHook();
                            playerAttackState.HookController.enabled = false;
                        }
                        break;
                        
                    case "Allies":
                        
                        if(ParentTag == "Player" || ParentTag == "Allies")
                            return;
                        
                        var botAlliesController = hit.transform.GetComponent<BotController>();
                        if(botAlliesController.IsHooked)
                            return;

                        var impactEffect2 = Instantiate(GameRoot.Instance.GameSettings.ImpactEffect);
                        impactEffect2.transform.position = botAlliesController.transform.position;
                        impactEffect2.SetActive(true);
                        
                        _botAttackState.SomethingHooked.Invoke();
                        Target = hit.collider.transform.gameObject;
                        hit.collider.transform.SetParent(transform, true);
                        
                        hit.transform.localPosition = new Vector3(0, 0.25f, 0);
                        
                        if(botAlliesController != null)
                            botAlliesController.Fire(BotController.BotTriggers.BotIsHooked);

                        var botAlliesMove = hit.transform.GetComponent<BotMoveState>();
                        if(botAlliesMove != null)
                            botAlliesMove.StopAllCoroutines();
                    
                        var botAlliesAttackState = hit.transform.GetComponent<BotAttackState>();
                        if (botAlliesAttackState)
                        {
                            botAlliesAttackState.HookController.RestLineRenderer();
                            // botAlliesAttackState.ResetHook();
                            botAlliesAttackState.HookController.enabled = false;
                        }
                    
                        if (hit.transform.GetComponent<NavMeshAgent>() != null)
                            hit.transform.GetComponent<NavMeshAgent>().enabled = false;
                        
                        break;

                    case "Enemy":
                        
                        if(ParentTag == "Enemy")
                            return;

                        var botController = hit.transform.GetComponent<BotController>();
                        if(botController.IsHooked)
                            return;

                        var impactEffect = Instantiate(GameRoot.Instance.GameSettings.ImpactEffect);
                        impactEffect.transform.position = botController.transform.position;
                        impactEffect.SetActive(true);
                        
                        if (ParentTag == "Player")
                            _playerAttackState.SomethingHooked.Invoke();
                        else
                            _botAttackState.SomethingHooked.Invoke();
                        
                        //botController.ImpactEffect.SetActive(true);
                        
                        Target = hit.collider.transform.gameObject;
                        hit.collider.transform.SetParent(transform, true);
                        
                        hit.transform.localPosition = new Vector3(0, 0.25f, 0);
                        
                        if(botController != null)
                            botController.Fire(BotController.BotTriggers.BotIsHooked);

                        var botMove = hit.transform.GetComponent<BotMoveState>();
                        if(botMove != null)
                            botMove.StopAllCoroutines();
                    
                        var botAttackState = hit.transform.GetComponent<BotAttackState>();
                        if (botAttackState)
                        {
                            botAttackState.HookController.RestLineRenderer();
                            // botAlliesAttackState.ResetHook();
                            botAttackState.HookController.enabled = false;
                        }
                        
                        if (hit.transform.GetComponent<NavMeshAgent>() != null)
                            hit.transform.GetComponent<NavMeshAgent>().enabled = false;
                
                        break;
                }
            }
        }
    }
    
    public void ResetPositions()
    {
        LineRenderer.SetPosition(1,startLiner);
        transform.localPosition = Vector3.zero;
    }
    
}
