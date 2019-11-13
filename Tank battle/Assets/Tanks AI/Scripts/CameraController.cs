using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform TargetObj;//目标对象
    public float DistanceAway = 10f;//摄像头与人物的距离，z轴位移
    public float DistanceUp = 2;//摄像头与人物的高度，y轴距离
    
    Vector3 _target;

    // Start is called before the first frame update
    void Start()
    {
        _target = TargetObj.position - transform.position;

    }

    //放在LateUpdate中是因为会发生镜头抖动，主要原因就是跟随对象的刷新频率的不同
    private void LateUpdate()
    {
        Rotate();
        _target = TargetObj.position + Vector3.up * DistanceUp - TargetObj.forward * DistanceAway;//人物的距离+新的

        transform.position = _target;
    }

    void Rotate()
    {
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, TargetObj.eulerAngles.y, transform.eulerAngles.z);
        transform.rotation = targetRotation;
    }
}
