using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

namespace MyWork.Network.Tanks
{
    public class GameManager : MonoBehaviourPunCallbacks,IPunObservable
    {

        #region Public Fields
        [Tooltip("The prefab is used to instantiate player in network")]
        public GameObject PlayerPrefabToNet;
        [Tooltip("The time of wait for game start")]
        public float StartDelay = 3f;
        [Tooltip("The time of wait for game end")]
        public float EndDelay = 1f;
        
        public static GameManager Instance;
        public Transform[] PlayerEnterPoints;

        public GameObject Message;

        public CameraControl LocalCameraCol;

        [HideInInspector]
        public UserInfo roundUserInfo;
        [HideInInspector]
        public UserInfo winnerUserInfo;

        public GameObject SettingsPanel;

        #endregion;

        #region Private Fields

        GameObject _player;
        int _currentRound = 0;
        Text _messageText;
        bool _settingsOpen = false;

        NetworkTankCol _netController;
        NetTankHealth _health;
        NetTankShooting _shooting;
        NetTankMovement _movement;

        WaitForSeconds _startWait;
        WaitForSeconds _endWait;

        Animator _settingsPanelAnim;

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            _startWait = new WaitForSeconds(StartDelay);
            _endWait = new WaitForSeconds(EndDelay);
            _messageText = Message.GetComponentInChildren<Text>();

            winnerUserInfo = null;
            
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("03_NetworkPVP_Lobby");
                return;
            }
            
            if (PlayerPrefabToNet == null)
            {
                Debug.LogError("GameManager/Start Error : the PlayerPrefabToNet can't be empty !");
            }
            else
            {
                if (LocalDataManager.Instance.User.PlayerID <= 0)
                {
                    Debug.LogError("GameManager/Start Error : the playerId doesn't be distribution");
                    return;
                }

                Debug.Log("GameManager/Start  Log : UserInfo.PlayerID : "+LocalDataManager.Instance.User.PlayerID);
                _player = PhotonNetwork.Instantiate(this.PlayerPrefabToNet.name,
                PlayerEnterPoints[LocalDataManager.Instance.User.PlayerID-1].position
                , PlayerEnterPoints[LocalDataManager.Instance.User.PlayerID-1].rotation,0,new object[] { LocalDataManager.Instance.User});
                
                _health = _player.GetComponent<NetTankHealth>();
                _shooting = _player.GetComponent<NetTankShooting>();
                _movement = _player.GetComponent<NetTankMovement>();

                _netController = _player.GetComponent<NetworkTankCol>();
                
            }
            
            if (PhotonNetwork.IsMasterClient)
            {
                LocalDataManager.Instance.ResetReadyInfoALl();//重置所有玩家的准备信息，该准备信息表示为玩家是否已完成实例化
                
                photonView.StartCoroutine(WaitForAllPlayerInstanceSucceed());
            }

            _settingsPanelAnim = SettingsPanel.GetComponent<Animator>();

        }

        private void Update()
        {
            if (_settingsOpen)
            {
                TouchScreen();
            }
        }

        #endregion

        #region Public Methods

        public void Btn_Exit()
        {
            SendLeaveRoom();
            LocalCameraCol.Targets.Clear();
            LocalDataManager.Instance.Clear();
            Application.Quit();
        }

        public void Btn_OpenSettingsPanel()
        {
            _settingsPanelAnim.SetTrigger("Open");
            _settingsOpen = true;
            SettingsPanel.GetComponentInChildren<Text>().text = TopMessage();
        }
        
        public void Btn_LeaveRoom()
        {
            SendLeaveRoom();
            LocalCameraCol.Targets.Clear();
            LocalDataManager.Instance.Clear();
            PhotonNetwork.LeaveRoom();
        }

        /// <summary>
        /// 游戏控制
        /// </summary>
        public void StartGame()
        {
            photonView.StartCoroutine(GameLoop());
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        public void EndGame()
        {
            SendEndGame();
        }

        public void RoundStartController()
        {
            ResetPlayer();
            DisableController();
            SendResetCameraMsg();
            _currentRound++;
            _messageText.text = "Round " + _currentRound + " ! ";
        }

        public void RoundPlayingController()
        {
            EnableController();
            _messageText.text = string.Empty;
            Message.SetActive(false);
        }

        public void RoundEndContorller()
        {
            DisableController();
            Message.SetActive(true);
            _messageText.text = EndMessage();
            roundUserInfo = null;
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// 重置玩家
        /// </summary>
        void ResetPlayer()
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                var tank = PhotonNetwork.PlayerList[i].TagObject as GameObject;
                tank.SetActive(false);
                tank.SetActive(true);
                tank.GetComponent<NetworkTankCol>().ResetPos();
            }
        }

        /// <summary>
        /// 禁止玩家进行操作
        /// </summary>
        void DisableController()
        {
            _movement.enabled = false;
            _shooting.enabled = false;

            Message.SetActive(true);
        }

        /// <summary>
        /// 允许玩家进行操作
        /// </summary>
        void EnableController()
        {
            _movement.enabled = true;
            _shooting.enabled = true;

            Message.SetActive(false);
        }


        UserInfo GetRoundWinner()
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                var tank = PhotonNetwork.PlayerList[i].TagObject as GameObject;
                if (tank.activeSelf)
                {
                    var info = tank.GetComponent<NetworkTankCol>().Mine;
                    UserInfo winner = LocalDataManager.Instance.FindUser(info.PlayerID);
                    winner.WinRound++;
                    return winner;
                }

            }
            return null;
        }

        UserInfo GetWinner()
        {
            foreach(UserInfo info in LocalDataManager.Instance.AllUser)
            {
                if (info.WinRound == LocalDataManager.Instance.WinNumber)
                    return info;
            }
            return null;
        }

        string EndMessage()
        {
            string message = "DRAW";

            if (winnerUserInfo != null)
            {
                message = "<color=#" + ColorUtility.ToHtmlStringRGB(roundUserInfo.PlayerColor) +
                    ">PLAYER " + roundUserInfo.NickName +"</color>" + " :  Wins the game ! ";
                return message;
            }

            if (roundUserInfo != null)
            {
                message = "<color=#" + ColorUtility.ToHtmlStringRGB(roundUserInfo.PlayerColor) +
                    ">PLAYER " + roundUserInfo.NickName + "</color>" + " wins thr round ! ";
               
            }
            message += "\n\n\n\n";
            foreach(UserInfo info in LocalDataManager.Instance.AllUser)
            {
                message += "<color=#" + ColorUtility.ToHtmlStringRGB(info.PlayerColor) +
                    ">PLAYER " + info.NickName + "</color>" + " : " + info.WinRound + " Wins\n";
            }
            
            return message;
        }

        bool OnLeftPlayer()
        {
            int player = 0;
            for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                var tank = PhotonNetwork.PlayerList[i].TagObject as GameObject;
                if (tank.activeSelf)
                    player++;
            }
            return player <= 1;
        }

        string TopMessage()
        {
            string message = string.Empty;
            foreach (UserInfo u in LocalDataManager.Instance.AllUser)
            {
                message+= "<color=#" + ColorUtility.ToHtmlStringRGB(u.PlayerColor) +
                    ">PLAYER " + u.NickName + "</color>" + " wins : " + u.WinRound + " \n";
            }

            message += "\n\n\n";

            bool flag = false;

            foreach(Player p in PhotonNetwork.PlayerList)
            {
                var tank = p.TagObject as GameObject;
                if (!tank.activeInHierarchy)
                    continue;
                flag = true;
                var controller = tank.GetComponent<NetworkTankCol>();
                message += "<color=#" + ColorUtility.ToHtmlStringRGB(controller.Mine.PlayerColor) +
                    ">PLAYER " + controller.Mine.NickName + "  </color>";
            }
            if (flag)
                message += "tanks have be alive !";

            return message;
        }

        void TouchScreen()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.currentSelectedGameObject==null)
                {
                    _settingsPanelAnim.SetTrigger("Close");
                    _settingsOpen = false;
                }
            }
        }

        #endregion

        #region RPCs Send Methods

        public void SendResetCameraMsg()
        {
            photonView.RPC("RPC_ALL_ResetCamera", RpcTarget.All);
        }

        public void SendRoundStartMsg()
        {
            photonView.RPC("RPC_ALL_RoundStart", RpcTarget.All);
        }

        public void SendRoundPlayingMsg()
        {
            photonView.RPC("RPC_ALL_RoundPlaying", RpcTarget.All);
        }

        public void SendRoundWinner(UserInfo roundWinner, UserInfo gameWinner)
        {
            photonView.RPC("RPC_ALL_RoundWinner", RpcTarget.All, roundWinner, gameWinner);
        }

        public void SendRouondEnd()
        {
            photonView.RPC("RPC_ALL_RoundEnd", RpcTarget.All);
        }

        public void SendSpawnSucceedInfo()
        {
            photonView.RPC("RPC_Master_SpawnSucceedInfo", RpcTarget.MasterClient, LocalDataManager.Instance.User.PlayerID);
        }
        
        public void SendLeaveRoom()
        {
            if (_netController == null)
            {
                Debug.LogError("GameManager/SendLeaveRoom Error : Component of _netController Can't find , Reset _netComntroller again !");
                _netController = _player.GetComponent<NetworkTankCol>();
            }
            photonView.RPC("RPC_Others_LeaveRoom", RpcTarget.Others, LocalDataManager.Instance.User.PlayerID,_netController.photonView.Owner);
        }

        public void SendEndGame()
        {
            photonView.RPC("RPC_ALL_EndGame", RpcTarget.All);
        }

        #endregion

        #region PUN2 RPCs
        /// <summary>
        /// 所有玩家准备游戏
        /// </summary>
        [PunRPC]
        void RPC_ALL_RoundStart()
        {
            RoundStartController();
        }

        /// <summary>
        /// 所有玩家开始游戏
        /// </summary>
        [PunRPC]
        void RPC_ALL_RoundPlaying()
        {
            RoundPlayingController();
        }

        /// <summary>
        /// 所有玩家结束当前轮
        /// </summary>
        [PunRPC]
        void RPC_ALL_RoundEnd()
        {
            RoundEndContorller();
        }

        /// <summary>
        /// 广播所有玩家获取当前轮获胜者
        /// </summary>
        [PunRPC]
        void RPC_ALL_RoundWinner(UserInfo roundWinner, UserInfo gameWinner)
        {
            roundUserInfo = roundWinner;
            winnerUserInfo = gameWinner;
            LocalDataManager.Instance.FindUser(roundWinner.PlayerID).WinRound = roundWinner.WinRound;
        }

        /// <summary> 远程调用所有玩家重新设置相机</summary>
        [PunRPC]
        void RPC_ALL_ResetCamera()
        {
            LocalCameraCol.Targets.Clear();

            Player[] playersinfo = PhotonNetwork.PlayerList;
            
            GameObject[] players = new GameObject[playersinfo.Length];

            for (int i = 0; i < playersinfo.Length; i++)
            {
                var obj = playersinfo[i].TagObject as GameObject;
                players[i] = obj;
                LocalCameraCol.Targets.Add(obj);
            }

            //List<GameObject> buf = new List<GameObject>(players);

            //if (LocalCameraCol == null)
            //{
            //    Debug.LogWarning("NetworkTankCol / RPC_RemoveCameraObj  Warning : Can't find Camera ! Now will find camera renew !");
            //    LocalCameraCol = GameObject.Find("CameraRig").GetComponent<CameraControl>();
            //}

            //LocalCameraCol.Targets = buf.Union(LocalCameraCol.Targets).ToList<GameObject>();

            LocalCameraCol.SetStartPositionAndSize();
        }

        /// <summary>
        /// 移除摄像机目标
        /// </summary>
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

        /// <summary>
        /// 向主机发送已经完成实例化
        /// </summary>
        [PunRPC]
        void RPC_Master_SpawnSucceedInfo(int id)
        {
            UserInfo user = LocalDataManager.Instance.FindUser(id);
            if (user == null)
            {
                Debug.LogError("NetwrokTankCol/ RPC_Client_SpawnSucceedInfo Error : Can't find UserInfo whitch id is " + id);
                return;
            }
            user.RPC_Master_IsReady = true;
        }

        /// <summary>
        /// 玩家退出房间
        /// </summary>
        [PunRPC]
        void RPC_Others_LeaveRoom(int id,Player player)
        {
            UserInfo buf = LocalDataManager.Instance.FindUser(id);//通过id查询，防止因round信息不匹配而无法查询到玩家
            if (buf == null)
                return;
            LocalDataManager.Instance.AllUser.Remove(buf);//从本地维护的信息表中删除
            if (player == null)
            {
                Debug.LogError("Player not exist !");
                return;

            }
            else
            {
                Debug.Log(player.IsInactive);
            }

            var tank = player.TagObject as GameObject;//检查当前玩家的实例化对象是否活跃
            if (tank.activeInHierarchy)
            {
                LocalCameraCol.Targets.Remove(tank);
                tank.SetActive(false);//移除并且删除
            }
        }

        [PunRPC]
        void RPC_ALL_EndGame()
        {
            LocalDataManager.Instance.ResetInfoAfterGameEnd();
            SceneManager.LoadScene("03_NetworkPVP_Lobby");
        }

        #endregion

        #region Coroutines Only Master can use these Corouines 
        
        IEnumerator WaitForAllPlayerInstanceSucceed()
        {

            LocalDataManager.Instance.FindUser(LocalDataManager.Instance.User.PlayerID).RPC_Master_IsReady = true;

            //yield return new WaitForSeconds(10);
            yield return new WaitUntil(() =>
            {
                Debug.Log("!");
                foreach (UserInfo info in LocalDataManager.Instance.AllUser)
                {
                    if (!info.RPC_Master_IsReady)
                        return false;
                }
                return true;
            });

            foreach(Player p in PhotonNetwork.PlayerList)
            {
                var tank = p.TagObject as GameObject;
                Debug.Log(tank == null ? "Yes" : "No");
                tank.GetComponent<NetworkTankCol>().SetColor();
            }

            StartGame();
        }
        
        /// <summary>
        /// 游戏轮数逻辑
        /// </summary>
        IEnumerator GameLoop()
        {
            yield return photonView.StartCoroutine(RoundStarting());
            Debug.Log("Into RoundPlaying");
            yield return photonView.StartCoroutine(RoundPlaying());
            yield return photonView.StartCoroutine(RoundEnding());
            if (winnerUserInfo == null)
            {
                photonView.StartCoroutine(GameLoop());
            }
            else
            {
                EndGame();
            }
        }

        /// <summary>
        ///游戏开始
        /// </summary>
        IEnumerator RoundStarting()
        {
            SendRoundStartMsg();
            yield return _startWait;
        }

        /// <summary>
        /// 游戏游玩ing
        /// </summary>
        IEnumerator RoundPlaying()
        {
            Debug.Log("RoundPlaying");
            SendRoundPlayingMsg();
            while (!OnLeftPlayer())
            {
                yield return null;
            }
        }

        /// <summary>
        /// 单轮游戏结束
        /// </summary>
        IEnumerator RoundEnding()
        {
            Debug.Log("RoundEnding");
            SendRoundWinner(GetRoundWinner(),GetWinner());
            SendRouondEnd();
            yield return _endWait;
        }
        #endregion

        #region Photon Callbacks
    
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            SceneManager.LoadScene("03_NetworkPVP_Lobby");
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            return;
        }

        #endregion

    }
}