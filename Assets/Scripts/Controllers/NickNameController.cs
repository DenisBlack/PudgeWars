using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NickNameController : MonoBehaviour
{
    private Transform mainCamera;
    public TMP_Text NickNameLabel;
    void Start()
    {
        mainCamera = Camera.main.transform;
    }

    public void SetNickName(string value)
    {
        NickNameLabel.text = value;
    }
    
    void Update()
    {
        transform.LookAt(mainCamera);
    }
}
