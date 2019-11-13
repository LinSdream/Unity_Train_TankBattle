using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int PlayerNumber = 1;//玩家id,可以通过玩家id来判断是谁
    public float MoveSpeed = 12f;//移动速度
    public float TurnSpeed = 180f;//转弯速度
    public AudioSource MovementAudio;//移动脚本的音频脚本
    public AudioClip DrivingAudioClip;//开车音频剪辑
    public AudioClip EngineIdling;//刹车音频剪辑
    public float PitchRange = 0.2f;//音频音量浮动范围

    string _movementAxisName;//移动方向轴名称
    string _turnAxisName;//转向轴名称
    Rigidbody _body;//刚体
    float _movementInputValue;
    float _turnInpoutValue;
    float _originalPitch;//原始音量

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _body.isKinematic = false;
        _movementInputValue = 0f;
        _turnInpoutValue = 0f;
    }

    private void OnDisable()
    {
        _body.isKinematic = true;
    }

    private void Start()
    {
        _movementAxisName = "Vertical" + PlayerNumber;
        _turnAxisName = "Horizontal" + PlayerNumber;

        _originalPitch = MovementAudio.pitch;
    }

    private void Update()
    {
        _movementInputValue = Input.GetAxis(_movementAxisName);
        _turnInpoutValue = Input.GetAxis(_turnAxisName);

        EngineAudio();
    }

    void EngineAudio()
    {
        //
        if(Mathf.Abs(_movementInputValue)<0.1f && Mathf.Abs(_turnInpoutValue) < 0.1f)
        {
            if (MovementAudio.clip == DrivingAudioClip)
            {
                MovementAudio.clip = EngineIdling;
                MovementAudio.pitch = Random.Range(_originalPitch - PitchRange, _originalPitch + PitchRange);
                MovementAudio.Play();
            }
        }
        else
        {
            if (MovementAudio.clip == EngineIdling)
            {
                MovementAudio.clip = DrivingAudioClip;
                MovementAudio.pitch = Random.Range(_originalPitch - PitchRange, _originalPitch + PitchRange);
                MovementAudio.Play();
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
        Turn();
    }

    void Move()
    {
        var movement = transform.forward * MoveSpeed * _movementInputValue * Time.deltaTime;
        _body.MovePosition(_body.position + movement);
    }

    void Turn()
    {
        float turnValue = TurnSpeed * _turnInpoutValue * Time.deltaTime;
        Quaternion turn = Quaternion.Euler(0f, turnValue, 0f);
        _body.MoveRotation(_body.rotation*turn);
    }
}
