using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SDKInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        DOTween.Init(true, true, LogBehaviour.Default);
    }

}
