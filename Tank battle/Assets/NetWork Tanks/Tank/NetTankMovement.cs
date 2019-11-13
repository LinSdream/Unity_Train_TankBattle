using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace MyWork.Network.Tanks
{
    public class NetTankMovement : MonoBehaviourPun
    {
        #region Public Fields
        //public int PlayerNumber = 1;//玩家id,可以通过玩家id来判断是谁
        public float MoveSpeed = 12f;//移动速度
        public float TurnSpeed = 180f;//转弯速度
        public AudioSource MovementAudio;//移动脚本的音频脚本
        public AudioClip DrivingAudioClip;//开车音频剪辑
        public AudioClip EngineIdling;//刹车音频剪辑
        public float PitchRange = 0.2f;//音频音量浮动范围

        #endregion

        #region Private Fields

        string _movementAxisName;//移动方向轴名称
        string _turnAxisName;//转向轴名称
        Rigidbody _body;//刚体
        float _movementInputValue;
        float _turnInpoutValue;
        float _originalPitch;//原始音量
        NetworkTankCol _networkCol;
        #endregion

        #region MonoBehaviours Callbacks

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
        }

        void OnEnable()
        {
            _body.isKinematic = false;
            _movementInputValue = 0f;
            _turnInpoutValue = 0f;
        }


        private void Start()
        {
            _networkCol = GetComponent<NetworkTankCol>();

            _movementAxisName = "Vertical";
            _turnAxisName = "Horizontal";

            _originalPitch = MovementAudio.pitch;
        }

        private void Update()
        {
            _movementInputValue = Input.GetAxis(_movementAxisName);
            _turnInpoutValue = Input.GetAxis(_turnAxisName);

            EngineAudio();
        }


        private void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                Move();
                Turn();
            }
        }
        
        void OnDisable()
        {
            _body.isKinematic = true;
        }

        #endregion

        #region Public Methods

        public void GetInputInfo(out float move,out float turn)
        {
            move = _movementInputValue;
            turn = _turnInpoutValue;
        }

        public void AudioPlay(float distance,float turn)
        {
            if (distance < 0.1f && turn < 0.1f)
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

        #endregion

        #region Private Methods
        void EngineAudio()
        {
            //
            if (Mathf.Abs(_movementInputValue) < 0.1f && Mathf.Abs(_turnInpoutValue) < 0.1f)
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

        void Move()
        {
            var movement = transform.forward * MoveSpeed * _movementInputValue * Time.deltaTime;
            _body.MovePosition(_body.position + movement);
        }

        void Turn()
        {
            float turnValue = TurnSpeed * _turnInpoutValue * Time.deltaTime;
            Quaternion turn = Quaternion.Euler(0f, turnValue, 0f);
            _body.MoveRotation(_body.rotation * turn);
        }

        #endregion
        
    }

}