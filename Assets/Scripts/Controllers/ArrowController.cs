using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [Header("Enemies Check")] 
    public List<GameObject> EnemiesList = new List<GameObject>();
    public GameObject Arrow;

    private bool CanFollow;
    public Transform PlayerTransform;

    public Transform CurrentTarget;
    void Start()
    {
        PlayerTransform = FindObjectOfType<PlayerController>().transform;

        GetEnemiesList();
        
        //StartCoroutine(DoShowArrowToEnemy());
    }
    
    private void Update()
    {
        transform.position = PlayerTransform.position;
        
        var targetEnemy = EnemiesList.Where(x=> !x.GetComponent<BotController>().IsHooked).OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).FirstOrDefault();
        if(targetEnemy == null)
            return;
        
        CurrentTarget = targetEnemy.transform;

        transform.LookAt(targetEnemy.transform);
        transform.rotation = Quaternion.Euler(90f, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    IEnumerator DoShowArrowToEnemy()
    {
        yield return new WaitForSeconds(0.5f);
        
        while (true)
        {
            var targetEnemy = EnemiesList.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).FirstOrDefault();
            CurrentTarget = targetEnemy.transform;

            transform.LookAt(targetEnemy.transform);
            transform.rotation = Quaternion.Euler(90f, transform.eulerAngles.y, transform.eulerAngles.z);
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void GetEnemiesList()
    {
        var enemies = FindObjectsOfType<BotController>();
        foreach (var item in enemies)
        {
            if(item.IsAlliace)
                continue;
            
            EnemiesList.Add(item.transform.gameObject);
        }
    }
}
