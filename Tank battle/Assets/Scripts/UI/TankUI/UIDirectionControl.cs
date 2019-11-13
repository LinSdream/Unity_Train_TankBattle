using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this class is used to make sure the health slider whether face to player 
public class UIDirectionControl : MonoBehaviour
{
    public bool UseRelativeRotation = true; //是否使用相对旋转

    Quaternion _relativeRotation;//实际旋转四元数

    private void Start()
    {
        _relativeRotation = transform.parent.localRotation;//初始化的时候将该物体旋转角度与Tank相同
    }

    private void Update()
    {
        if (UseRelativeRotation)//这里之所以这么写，就是为了能够调整血条的先对位置，确保血条能够相对于玩家的方向
            transform.rotation = _relativeRotation;
    }
}
