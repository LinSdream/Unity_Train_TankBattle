using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

namespace MyWork.Network.Tanks
{

    public class NetTankShooting : MonoBehaviourPunCallbacks
    {

        #region Public Fields

        //public int PlayerNum = 1;
        public Rigidbody Shell;
        public Transform FireTransform;
        public Slider AimSlider;
        public AudioSource ShootingAudio;
        public AudioClip ChargingClip;//充能
        public AudioClip FireClip;
        public float MinLaunchForce = 15;
        public float MaxLaunchForce = 30f;
        public float MaxChargeTime = 0.75f;//充能时间

        #endregion

        #region Private Fields

        NetworkTankCol _networkCol;
        string _fireButton;
        float _currentLaunchForce;
        float _chargeSpeed;
        bool _fired;

        #endregion

        #region MonoBehaviour Callbacks

        // Start is called before the first frame update
        void Start()
        {
            _fireButton = "Fire";
            _chargeSpeed = (MaxLaunchForce - MinLaunchForce) / MaxChargeTime;
            _networkCol = GetComponent<NetworkTankCol>();
        }
        
        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine)
            {
                AimSlider.value = MinLaunchForce;
                if (_currentLaunchForce >= MaxLaunchForce && !_fired)
                {
                    //at max charge , not yet fired；当充能打到最大值时候，充能设为最大值，并且发射shell
                    _currentLaunchForce = MaxLaunchForce;
                    Fire();
                }
                else if (Input.GetButtonDown(_fireButton))
                {
                    //have we pressed fire for the first time? 当按下发射按钮的时候，开始充能，
                    _fired = false;
                    _currentLaunchForce = MinLaunchForce;

                    ShootingAudio.clip = ChargingClip;
                    ShootingAudio.Play();
                }
                else if (Input.GetButton(_fireButton) && !_fired)
                {
                    //hold the fire button ,not yet fired;保持不变的时候，一直按着南涧，进行充能
                    _currentLaunchForce += _chargeSpeed * Time.deltaTime;

                    AimSlider.value = _currentLaunchForce;
                }
                else if (Input.GetButtonUp(_fireButton) && !_fired)
                {
                    //we released the button , having not fired yet;放手的时候发射子弹
                    Fire();
                }
            }
        }

        #endregion

        #region Private Methods 

        void Fire()
        {
            _fired = true;
            Rigidbody shellInstance = Instantiate(Shell, FireTransform.position, FireTransform.rotation) as Rigidbody;
            shellInstance.velocity = _currentLaunchForce * FireTransform.forward;

            _networkCol.SendFireMsg(_currentLaunchForce);

            ShootingAudio.clip = FireClip;
            ShootingAudio.Play();
            _currentLaunchForce = MinLaunchForce;
        }

        #endregion

        #region Public Methods
        
        public void FireNetwork(float launchForce)
        {
            Rigidbody shellInstance = Instantiate(Shell, FireTransform.position, FireTransform.rotation) as Rigidbody;
            shellInstance.velocity = launchForce * FireTransform.forward;

            ShootingAudio.clip = FireClip;
            ShootingAudio.Play();
        }
        #endregion


        #region PUN2 Callbacks
        public override void OnEnable()
        {
            base.OnEnable();

            _currentLaunchForce = MinLaunchForce;
            AimSlider.value = MinLaunchForce;
        }

        #endregion
    }

}