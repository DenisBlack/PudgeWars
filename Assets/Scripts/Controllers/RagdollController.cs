using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public List<Rigidbody> rigidbodies;
    private List<Collider> colliders;
    public Transform Bone;

    private bool CanFreezRagdoll;
    private bool CanHideRagdoll;

    private float TimerCanFreezRagdoll;
    private float TimerCanHideRagdoll;
    private float TimerSpeedHide;
    public bool IsActive
    {
        get => isActive;
        set
        {
            Switch(value);
            isActive = value;
        }
    }
    private bool isActive = false;
    
    void Awake()
    {
        Init();
    }

    private void Start()
    {
        CanFreezRagdoll = true;

        TimerCanFreezRagdoll = GameRoot.Instance.GameSettings.RagdollTimerToEnd;
        TimerCanHideRagdoll = GameRoot.Instance.GameSettings.RagdollTimerToDespawn;
        TimerSpeedHide = GameRoot.Instance.GameSettings.RagdollSpeedHide;
    }

    private void Update()
    {
        if (CanFreezRagdoll)
        {
            TimerCanFreezRagdoll -= Time.deltaTime;
            if (TimerCanFreezRagdoll <= 0f)
            {
                CanFreezRagdoll = false;
                Switch(false);

                CanHideRagdoll = true;
            }
        }

        if (CanHideRagdoll)
        {
            TimerCanHideRagdoll -= Time.deltaTime;
            if (TimerCanHideRagdoll <= 0f)
            {
                CanHideRagdoll = false;
                this.enabled = false;
                gameObject.SetActive(false);
            }
            
            transform.Translate(Vector3.down * TimerSpeedHide * Time.deltaTime);
        }
    }

    public void ActiveWithDelay(Vector3 force)
    {
        StartCoroutine(DelayActive(force));
    }

    IEnumerator DelayActive(Vector3 force)
    {
        ActiveTriggerColliders(true);
        yield return new WaitForSeconds(0.1f);
        IsActive = true;
        AddForceAtPoint(force);
        
        yield return new WaitForSeconds(0.3f);
        ActiveTriggerColliders(false);
    }

    private void Init()
    {
        rigidbodies = transform.GetComponentsInChildren<Rigidbody>().ToList();
        colliders = transform.GetComponentsInChildren<Collider>().Where(x => x.isTrigger == false).ToList();
        IsActive = false;
    }
    
    private void Switch(bool isEnable)
    {
        foreach (var rigid in rigidbodies)
        {
            rigid.isKinematic = !isEnable;
        }
    }

    public void ActiveTriggerColliders(bool isEnable)
    {
        foreach (var col in colliders )
        {
            col.isTrigger = isEnable;
        }
    }
    
    public void SetData(Transform BonesRoot)
    {
        var bones = BonesRoot.GetComponentsInChildren<Transform>().ToDictionary(x => x.gameObject.name, x => x);
        var ragdollBones = Bone.GetComponentsInChildren<Transform>();
        foreach (var ragdollBone in ragdollBones)
        {
            if(bones.ContainsKey(ragdollBone.gameObject.name) == false)
                continue;
            var sourceBone = bones[ ragdollBone.gameObject.name ];
            ragdollBone.position = sourceBone.position;
            ragdollBone.rotation = sourceBone.rotation;
        }
    }
    
    public void AddForceAtPoint(Vector3 force)
    {
        transform.DOKill(false);

        foreach (var rigid in rigidbodies)
        {
            if(rigid != null)
                rigid.AddForce(force * 5f, ForceMode.Impulse);
        }
    }
}
