using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TankShooting : MonoBehaviour
{

    #region Public Fields

    public int PlayerNum = 1;
    public Rigidbody Shell;
    public Transform FireTransform;
    public Slider AimSlider;
    public AudioSource ShootingAudio;
    public AudioClip ChargingClip;//充能
    public AudioClip FireClip;
    public float MinLaunchForce = 15;
    public float MaxLaunchForce = 30f;
    public float MaxChargeTime = 0.75f;//充能时间
    public float Damage = 100f;
    
    //public float FireBufferForce;//开火缓冲力，防止，开火的时候开动tank而导致对自己造成伤害

    #endregion

    #region Private Fields

    string _fireButton;
    float _currentLaunchForce;
    float _chargeSpeed;
    bool _fired;

    #endregion

    #region MonoBehaviour Callbacks

    // Start is called before the first frame update
    void Start()
    {
        _fireButton = "Fire" + PlayerNum;
        _chargeSpeed = (MaxLaunchForce - MinLaunchForce) / MaxChargeTime;
    }
    
    void OnEnable()
    {
        _currentLaunchForce = MinLaunchForce;
        AimSlider.value = MinLaunchForce;
    }

    // Update is called once per frame
    void Update()
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

    #endregion

    #region private methods 

    void Fire()
    {
        _fired = true;

        Rigidbody shellInstance = Instantiate(Shell, FireTransform.position, FireTransform.rotation) as Rigidbody;
        var script = shellInstance.gameObject.GetComponent<ShellExplosion>();
        script.WhoShell = gameObject;
        script.MaxDamage = Damage;

        shellInstance.velocity = _currentLaunchForce * FireTransform.forward;

        ShootingAudio.clip = FireClip;
        ShootingAudio.Play();
        _currentLaunchForce = MinLaunchForce;
    }

    #endregion

    #region AI control methods

    public void FireByAI(float launchForce)
    {
        Rigidbody shellInstance = Instantiate(Shell, FireTransform.position, FireTransform.rotation) as Rigidbody;
        var script = shellInstance.gameObject.GetComponent<ShellExplosion>();
        script.WhoShell = gameObject;
        script.MaxDamage = Damage;

        shellInstance.velocity = launchForce * FireTransform.forward;

        ShootingAudio.clip = FireClip;
        ShootingAudio.Play();

    }

    #endregion

}
