using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace MyWork.Network.Tanks
{
    public class SynchronizeTransform : MonoBehaviour, IPunObservable
    {
        #region Private Fields

        private float m_Distance;
        private float m_Angle;

        private PhotonView m_PhotonView;

        private Vector3 m_Direction;
        private Vector3 m_NetworkPosition;
        private Vector3 m_StoredPosition;

        private Quaternion m_NetworkRotation;

        public bool m_SynchronizePosition = true;
        public bool m_SynchronizeRotation = true;
        public bool m_SynchronizeScale = false;

        Rigidbody _body;

        bool m_firstTake = false;

        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            m_PhotonView = GetComponent<PhotonView>();

            m_StoredPosition = transform.position;
            m_NetworkPosition = Vector3.zero;

            m_NetworkRotation = Quaternion.identity;
        }

        void OnEnable()
        {
            m_firstTake = true;
        }

        private void Start()
        {
            _body = GetComponent<Rigidbody>();
        }

        void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, this.m_NetworkPosition, 
                this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));

            gameObject.GetComponent<NetTankMovement>().AudioPlay(m_Distance,m_Angle);

        }

        #endregion

        #region PUN2 Callbacks
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (this.m_SynchronizePosition)
                {
                    this.m_Direction = transform.position - this.m_StoredPosition;
                    this.m_StoredPosition = transform.position;

                    stream.SendNext(transform.position);
                    stream.SendNext(this.m_Direction);
                }

                if (this.m_SynchronizeRotation)
                {
                    stream.SendNext(transform.rotation);
                }

                if (this.m_SynchronizeScale)
                {
                    stream.SendNext(transform.localScale);
                }
            }
            else
            {
                if (this.m_SynchronizePosition)
                {
                    this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
                    this.m_Direction = (Vector3)stream.ReceiveNext();

                    if (m_firstTake)
                    {
                        transform.position = this.m_NetworkPosition;
                        this.m_Distance = 0f;
                    }
                    else
                    {
                        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                        this.m_NetworkPosition += this.m_Direction * lag;
                        this.m_Distance = Vector3.Distance(transform.position, this.m_NetworkPosition);
                    }


                }

                if (this.m_SynchronizeRotation)
                {
                    this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                    if (m_firstTake)
                    {
                        this.m_Angle = 0f;
                        transform.rotation = this.m_NetworkRotation;
                    }
                    else
                    {
                        this.m_Angle = Quaternion.Angle(transform.rotation, this.m_NetworkRotation);
                    }
                }

                if (this.m_SynchronizeScale)
                {
                    transform.localScale = (Vector3)stream.ReceiveNext();
                }

                if (m_firstTake)
                {
                    m_firstTake = false;
                }
            }
        }
        #endregion
    }

}
