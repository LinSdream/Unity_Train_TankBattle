using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.SceneManagement;

namespace MyWork.Network.Tanks
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkTankCol : MonoBehaviourPunCallbacks,IPunInstantiateMagicCallback,IPunObservable
    {
        
        #region Public Fields

        public CameraControl LocalCameraCol;

        public UserInfo Mine = new UserInfo();

        #endregion

        #region Private Fields

        GameManager _manager;

        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            LocalCameraCol = GameObject.Find("CameraRig").GetComponent<CameraControl>();
            if (LocalCameraCol == null)
            {
                Debug.LogError("NetworkTankCol / Start Error : Can't get the component 'CameraControl' !");
                return;
            }
            _manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
            
        }

        #endregion

        #region Public Methods

        public void SetColor()
        {
            MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = Mine.PlayerColor;
            }
        }

        public void ResetPos()
        {
            if (Mine == null)
            {
                Debug.LogError("NetworkTankCol/ResetPos Error : the Mine info is null");
                return;
            }
            gameObject.transform.rotation = _manager.PlayerEnterPoints[Mine.PlayerID - 1].rotation;
            gameObject.transform.position = _manager.PlayerEnterPoints[Mine.PlayerID - 1].position;
        }

        public void SendFireMsg(float launchForce)
        {
            photonView.RPC("RPC_FireInfo", RpcTarget.Others, launchForce, photonView.Owner);
        }

        public void SendDamageMsg(float amount)
        {
            if (photonView.IsMine)
            {
                photonView.RPC("RPC_DamageInfo", RpcTarget.All, amount, photonView.Owner);
            }
        }
        
        public void SendRemoveCameraMsg()
        {
            photonView.RPC("RPC_RemoveCameraObj", RpcTarget.Others, photonView.Owner);
        }

        #endregion

        #region PUN2 RPCs

        [PunRPC]
        void RPC_RemoveCameraObj(Player info)
        {
            if (LocalCameraCol == null)
            {
                Debug.LogWarning("NetworkTankCol / RPC_RemoveCameraObj  Warning : Can't find Camera ! Now will find camera renew !");
                LocalCameraCol = GameObject.Find("CameraRig").GetComponent<CameraControl>();
            }

            LocalCameraCol.Targets.Remove((GameObject)info.TagObject);
        }

        [PunRPC]
        void RPC_FireInfo(float launchForce, Player info)
        {
            var tank = info.TagObject as GameObject;
            var shooting = tank.GetComponent<NetTankShooting>();

            shooting.FireNetwork(launchForce);
        }

        [PunRPC]
        void RPC_DamageInfo(float amount, Player info)
        {
            var obj = info.TagObject as GameObject;
            var health = obj.GetComponent<NetTankHealth>();

            health.Damage(amount);
        }
        
        #endregion

        #region PUN2 Callbacks

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            if (photonView.IsMine)
            {
                MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material.color = LocalDataManager.Instance.User.PlayerColor;
                }
            }
            gameObject.name = LocalDataManager.Instance.User.PlayerID.ToString();
            info.Sender.TagObject = this.gameObject;

            Mine = photonView.InstantiationData[0] as UserInfo;

            if (gameObject == null)
                return;
            GameManager.Instance.SendSpawnSucceedInfo();
            
        }
        
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            SceneManager.LoadScene("03_NetworkPVP_Lobby");
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            return;
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
        }
        
        #endregion

    }

}