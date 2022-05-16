using System;
using System.Collections;
using System.Collections.Generic;
using JBStateMachine;
using UnityEngine;
using DG.Tweening;
public class PlayerInputState : MonoBehaviour, IStateController
{
    public FloatingJoystick _joystick;
    
    public float MoveSpeed;
    public bool CanMove = false;

    private PlayerController _playerController;

    private CharacterController _characterController;

    public Animator playerAnimator;

    private bool CanUpdate = false;

    public Transform TargetTransform;

    public float TouchTime;
    
    private void Update()
    {
        if(!CanUpdate)
            return;

        if (_playerController.playerStateMachine.currentState == PlayerController.PlayerStates.PlayerInput && !GameRoot.Instance.IsGameFinished)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                _joystick.Test();
                _joystick.Vertical = -1f;
            }

            var tapCount = Input.touchCount;
            GameRoot.Instance.TouchCount.text = tapCount.ToString();
            
            if (Input.GetMouseButton(0) && !GameRoot.Instance.IsGameFinished)
            {
                TouchTime += Time.deltaTime;
            }
            
            if (Input.GetMouseButtonUp(0) && !GameRoot.Instance.IsGameFinished)
            {
                if (TouchTime <= 0.2f)
                {
                    int layerMask = (1 << 8);
                    var worldPosition = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(worldPosition, out var hit, Mathf.Infinity, layerMask))
                    {
                        if (hit.transform.tag == "Ground")
                        {
                            TargetTransform.position = hit.point;
                        
                            transform.LookAt(TargetTransform);
                            transform.rotation =
                                Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);
                            
                            CanMove = false;
                            
                            playerAnimator.ResetTrigger("Walk");
                            playerAnimator.SetTrigger("Idle");
                            
                            //Debug.LogError("Short Touch: " + TouchTime);
                            
                            TouchTime = 0f;
                            
                            _playerController.Fire( PlayerController.PlayerTriggers.PlayerAttackTrigger );
                        
                            return;
                        }
                    }
                }else TouchTime = 0f;
                
                
                
               // else Debug.LogError("Long Touch: "  + TouchTime);
            }
            
            if (_joystick.IsPressed && !CanMove)
            {
                CanMove = true;
            }
            else if (!_joystick.IsPressed && CanMove)
            {
                //_joystick.enabled = false;
                CanMove = false;
                
                playerAnimator.ResetTrigger("Walk");
                playerAnimator.SetTrigger("Idle");
            
                //_playerController.Fire( PlayerController.PlayerTriggers.PlayerAttackTrigger );
            }
        
            if (CanMove)
            {
                if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
                {
                    playerAnimator.ResetTrigger("Idle");
                    playerAnimator.SetTrigger("Walk");
                
                    //transform.eulerAngles = new Vector3( 0, Mathf.Atan2( _joystick.Horizontal, _joystick.Vertical) * 180  / Mathf.PI, 0 );
               
                    Quaternion eulerRot = Quaternion.Euler(0f, Mathf.Atan2(_joystick.Horizontal, _joystick.Vertical) * Mathf.Rad2Deg, 0f);
                    transform.rotation  = Quaternion.Slerp(transform.rotation, eulerRot, Time.deltaTime * GameRoot.Instance.GameSettings.PlayerRotationSpeed);
                    
                    var direction = new Vector3( _joystick.Horizontal, -1f, _joystick.Vertical);
                
                    _characterController.Move(direction *  GameRoot.Instance.GameSettings.PlayerMoveSpeed * Time.deltaTime);


                    if (tapCount >= 2)
                    {
                        _joystick.IsPressed = false;
                        CanMove = false;
                        
                        int layerMask = (1 << 8);
                        var worldPosition = Camera.main.ScreenPointToRay(Input.touches[1].position);
                        if (Physics.Raycast(worldPosition, out var hit, Mathf.Infinity, layerMask))
                        {
                            if (hit.transform.tag == "Ground")
                            {
                                TargetTransform.position = hit.point;
                            
                                transform.LookAt(TargetTransform);
                                transform.rotation =
                                    Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);
                                
                                playerAnimator.ResetTrigger("Walk");
                                playerAnimator.SetTrigger("Idle");
                                
                                //Debug.LogError("Short Touch " + TouchTime);
                                
                                TouchTime = 0f;
                                
                                _playerController.Fire( PlayerController.PlayerTriggers.PlayerAttackTrigger );
                            }
                        }
                    }
                }
            }
        }
    }

    public void OnEntered(EnterDataBase data)
    {
        TouchTime = 0;
        
        TargetTransform = GameObject.FindGameObjectWithTag("Hook Target").transform;
        
        playerAnimator.Play("IdleState");
        playerAnimator.SetTrigger("Idle");
        
        if(_playerController == null)
            _playerController = transform.GetComponent<PlayerController>();
    
        if(_characterController == null)
            _characterController = transform.GetComponent<CharacterController>();
        
        if(_joystick == null)
            _joystick = GameRoot.Instance.Joystick;

        
        
        _joystick.gameObject.SetActive(true);
        
        CanUpdate = true;
        
        _joystick.enabled = true;
    }
    
    public ExitDataBase OnExited()
    {
        CanUpdate = false;
        
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
