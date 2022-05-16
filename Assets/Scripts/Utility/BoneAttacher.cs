using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoneAttacher : MonoBehaviour
{
    public Transform Bone;
    
    void Update()
    {
        transform.position = Bone.position;
        transform.rotation = Bone.rotation;
    }
}
