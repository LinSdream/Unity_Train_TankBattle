using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCameraCol : MonoBehaviour
{

    public Transform Target;
    

    void FixedUpdate()
    {
        this.transform.position = Target.position;
    }
}
