using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JBStateMachine;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using DG.Tweening;
public class BotMoveState : MonoBehaviour, IStateController
{
    public NavMeshAgent _navMeshAgent;
    private BotController _botController;
    
    [Header("Short Move Settings")]
    public float ShortMoveRadius;
    public float ShortMinDistance;
    [Header("Long Move Settings")]
    public float LongMoveRadius;
    public float LongMinDistance;
    
    public BoxCollider areaCollider;
    private Coroutine _moveCoroutine;
    public EnemyMovePointsController _enemyMovePointsController;
    public Animator _animator;
    
    [Header("Moving Type")]
    public List<MovingType> MovingTypesList;

    private GameObject kostul;
    
    public enum MovingType
    {
        ShotMove,
        LongMove,
        LookAtPlayer
    }
    
    public void OnEntered(EnterDataBase data)
    {
        StopCoroutines();
        _navMeshAgent.enabled = true;
        _animator.Play("IdleState");
        _animator.SetTrigger("Walk");

        if(_botController == null)
            _botController = GetComponent<BotController>();
        
        if(_enemyMovePointsController == null)
            _enemyMovePointsController = GameRoot.Instance.EnemyMovePointsController;

        if (!_botController.IsAlliace)
            areaCollider = GameRoot.Instance.EnemyBoxCollider;
        else
            areaCollider = GameRoot.Instance.AlliesBoxCollider;
        
        var agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 120f;
        
         InitMove();
    }

    private void InitMove()
    {
        
        //OLD
        List<IEnumerator> coroutineList = new List<IEnumerator>();
        var randCount = Random.Range(2, 5);
        if (_botController.IsSniper)
        {
            coroutineList.Add(DoShortMove());
        }
        else
        {
            for (int i = 0; i < randCount; i++)
            {
                var rand = Random.Range(0, 3);
                switch (rand)
                {
                    case 0:
                        coroutineList.Add(DoShortMove());
                        break;
                    case 1:
                        coroutineList.Add(DoShortMove());
                        break;
                    case 2:
                        coroutineList.Add(DoLongMove());
                        break;
                }
            }
        }
  
        //New
        // List<IEnumerator> coroutineList = new List<IEnumerator>();
        // for (int i = 0; i < MovingTypesList.Count; i++)
        // {
        //     switch (MovingTypesList[i])
        //     {
        //         case MovingType.ShotMove:
        //             coroutineList.Add(DoShortMove());
        //             break;
        //         case MovingType.LookAtPlayer:
        //             coroutineList.Add(DoLookAtPlayer());
        //             break;
        //         case MovingType.LongMove:
        //             coroutineList.Add(DoLongMove());
        //             break;
        //     }
        // }
        
        _moveCoroutine = StartCoroutine(DoMove(coroutineList));
    }
    
    IEnumerator DoMove(List<IEnumerator> coroutineList)
    {
        for (int i = 0; i <  coroutineList.Count; i++)
        {
            yield return coroutineList[i];
        }
        
        _animator.ResetTrigger("Walk");
        _animator.SetTrigger("Idle");

        yield return new WaitForSeconds(0.5f);
        
        float randRotateValue = 0f;

        if (_botController.IsSniper)
        {
            var player = FindObjectOfType<PlayerController>();
            if (player != null && !player.IsHooked)
            {

                if(kostul == null)
                    kostul = new GameObject();
                
                kostul.transform.position = transform.position;
                kostul.transform.LookAt(player.transform);
                kostul.transform.rotation =
                    Quaternion.Euler(0, kostul.transform.eulerAngles.y, kostul.transform.eulerAngles.z);
                
                transform.DORotate(new Vector3(kostul.transform.eulerAngles.x, kostul.transform.eulerAngles.y, kostul.transform.eulerAngles.z), 0.5f);
            }
            else
            {
                randRotateValue =  !_botController.IsAlliace ? Random.Range(140f, 200f) : Random.Range(-22f,40f);
                transform.DORotate(new Vector3(0, randRotateValue, 0), GameRoot.Instance.GameSettings.BotRotationSpeedToHook);
            }
        }
        else
        {
            randRotateValue =  !_botController.IsAlliace ? Random.Range(140f, 200f) : Random.Range(-22f,40f);
            transform.DORotate(new Vector3(0, randRotateValue, 0), GameRoot.Instance.GameSettings.BotRotationSpeedToHook);
        }
        
        yield return new WaitForSeconds(GameRoot.Instance.GameSettings.BotRotationSpeedToHook);
        
        _botController.Fire(BotController.BotTriggers.BotAttackTrigger);
        
        yield return null;
    }

    private void ShortMove()
    {
        StartCoroutine(DoShortMove());
    }
    
    private IEnumerator DoShortMove()
    {
        Vector3 randomDirection = GetRandomDirection(ShortMinDistance);
        while (Vector3.Distance(transform.position, randomDirection) < ShortMinDistance)
        { 
            randomDirection = GetRandomDirection(ShortMinDistance);
            yield return null;
        }

        if (_enemyMovePointsController.CurrentPoins.Count != 0)
        {
            while (_enemyMovePointsController.CurrentPoins.Any(x => Vector3.Distance(randomDirection, x) < 4f))
            {
                randomDirection = GetRandomDirection(ShortMinDistance);
                yield return null;
            }
        }
 
        if (!_enemyMovePointsController.CurrentPoins.Contains(randomDirection))
            _enemyMovePointsController.CurrentPoins.Add(randomDirection);
        
        _animator.SetTrigger("Walk");
        
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, ShortMoveRadius, 1);
        Vector3 finalPosition = hit.position;
        _navMeshAgent.speed = GameRoot.Instance.GameSettings.BotMoveSpeed;
        _navMeshAgent.destination = finalPosition;

        yield return new WaitForSeconds(0.5f);

        float Timer = 2f;

        while (_navMeshAgent.velocity.sqrMagnitude != 0f)
        {
            Timer -= Time.deltaTime;

            if (Timer <= 0f)
            {
                yield break;
            }

            yield return null;
        }
        
        if (_enemyMovePointsController.CurrentPoins.Contains(randomDirection))
            _enemyMovePointsController.CurrentPoins.Remove(randomDirection);
        
        _navMeshAgent.SetDestination(transform.position);
   
        yield return null;
    }

    public void StopCoroutines()
    {
        StopAllCoroutines();
    }
    
    private void LongMove()
    {
        StartCoroutine(DoLongMove());
    }
    
    private IEnumerator DoLongMove()
    {
        Vector3 randomDirection = GetRandomDirection(LongMinDistance);
        
        while (Vector3.Distance(transform.position, randomDirection) < LongMinDistance)
        {
            randomDirection = GetRandomDirection(LongMinDistance);
            yield return null;
        }
        
        if (_enemyMovePointsController.CurrentPoins != null)
        {
            while (_enemyMovePointsController.CurrentPoins.Any(x => Vector3.Distance(randomDirection, x) < 4f))
            {
                randomDirection = GetRandomDirection(ShortMinDistance);
                yield return null;
            }
        }
        
        if (!_enemyMovePointsController.CurrentPoins.Contains(randomDirection))
            _enemyMovePointsController.CurrentPoins.Add(randomDirection);

        _animator.SetTrigger("Walk");
        
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, LongMoveRadius, 1);
        Vector3 finalPosition = hit.position;
        _navMeshAgent.speed = GameRoot.Instance.GameSettings.BotMoveSpeed;
        _navMeshAgent.destination = finalPosition;
        
        yield return new WaitForSeconds(0.5f);
        
        float Timer = 2f;

        while (_navMeshAgent.velocity.sqrMagnitude != 0f)
        {
            Timer -= Time.deltaTime;

            if (Timer <= 0f)
            {
                yield break;
            }

            yield return null;
        }
     
        if (_enemyMovePointsController.CurrentPoins.Contains(randomDirection))
            _enemyMovePointsController.CurrentPoins.Remove(randomDirection);
        
        _navMeshAgent.SetDestination(transform.position);
        
        yield return null;
    }

    private IEnumerator DoLookAtPlayer()
    {
        var player = FindObjectOfType<PlayerController>();

        if (player != null)
        {
            transform.LookAt(player.transform);
            transform.rotation =
                Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        yield return null;
    }

    private Vector3 GetRandomDirection(float minDistance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * ShortMoveRadius;
        
        if(!areaCollider.bounds.Contains(randomDirection + transform.position))
        {
            randomDirection = -randomDirection; // if next point outside boundary, do a 180
        }
        
        if(!areaCollider.bounds.Contains(randomDirection + transform.position))
        {
            randomDirection = -randomDirection; // if next point outside boundary, do a 180
        }
        
        randomDirection += transform.position;
        
        return randomDirection;
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

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.yellow;
    //     float theta = 0;
    //     float x = ShortMoveRadius * Mathf.Cos(theta);
    //     float y = ShortMoveRadius * Mathf.Sin(theta);
    //     Vector3 pos = transform.position + new Vector3(x, 0, y);
    //     Vector3 newPos = pos;
    //     Vector3 lastPos = pos;
    //     for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
    //     {
    //         x = ShortMoveRadius * Mathf.Cos(theta);
    //         y = ShortMoveRadius * Mathf.Sin(theta);
    //         newPos = transform.position + new Vector3(x, 0, y);
    //         Gizmos.DrawLine(pos, newPos);
    //         pos = newPos;
    //     }
    //     Gizmos.DrawLine(pos, lastPos);
    // }
}

