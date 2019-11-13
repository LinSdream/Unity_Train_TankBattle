using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using MyWork.Network.Tanks;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using System.Text;
using System;
using System.Linq;

namespace MyWork.Network
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        public GameObject ControlPanel;

        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        public GameObject ProgressPanel;

        public GameObject LobbyPanel;

        [Tooltip("The UI Label to join successed in the room that every player info display box")]
        public GameObject RoomPanel;

        public GameObject CreateRoomPanel;

        public GameObject JoinRoomInputPanel;

        public GameObject JoinFailedWindowPanel;
        
        public GameObject PlayersGirdsGroup;

        [Tooltip("The Game how much round ")]
        public int RoundNumber = 5;

        [Tooltip("The Game win round")]
        public int WinNumber = 3;

        public GameObject ItemPrefab;

        public List<GameObject> RoomsList;
      
        #endregion

        #region Private Serializable Fields

        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't " +
            "be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;
        
        UnityEngine.UIElements.ScrollView _view;

        #endregion

        #region Private Fields

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "2";

        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool isConnecting = false;

        /// <summary>
        /// Save Player Info for every one.
        /// </summary>
        [SerializeField]
        List<UserInfo> _allUsers;

        [SerializeField]
        bool _requestID = true;
        #endregion

        #region MonoBehaviour CallBacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
            _allUsers = new List<UserInfo>();
            RoomsList = new List<GameObject>();
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            if (!PhotonNetwork.InRoom)
            {
                ControlPanel.SetActive(true);
                ProgressPanel.SetActive(false);
                RoomPanel.SetActive(false);
                LobbyPanel.SetActive(false);
                JoinRoomInputPanel.SetActive(false);
                JoinFailedWindowPanel.SetActive(false);
            }
            else
            {
                LobbyPanel.SetActive(false);
                ControlPanel.SetActive(false);
                JoinRoomInputPanel.SetActive(false);
                ProgressPanel.SetActive(false);
                JoinFailedWindowPanel.SetActive(false);
                RoomPanel.SetActive(true);
                ResetRoomInfo();
                SetButton();
            }
            PhotonPeer.RegisterType(typeof(UserInfo), (byte)'U', SerializePlayerInfo, DeSerializePlayerInfo);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 从游戏状态返回游戏开始房间，进行房间信息的恢复
        /// </summary>
        public void ResetRoomInfo()
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            RoomPanel.transform.GetChild(3).GetComponent<Text>().text = PhotonNetwork.CurrentRoom.MasterClientId.ToString();
            _allUsers = LocalDataManager.Instance.AllUser;

            if (PhotonNetwork.IsMasterClient)
            {
                FindUser(LocalDataManager.Instance.User.PlayerID).RPC_Master_IsReady = true;
            }

            foreach (UserInfo info in _allUsers)
            {
                SetRoomPanel(info);
            }
        }

        #endregion

        #region Serialize and Deserialize PlayerInfo Class

        /// <summary> 序列化UserInfo 不对RPC_Master_IsReady 进行序列化</summary>
        static byte[] SerializePlayerInfo(object info)
        {
            int index = 0;
            UserInfo playerInfo = (UserInfo)info;

            byte[] strBytes = UTF8Encoding.Default.GetBytes(playerInfo.NickName);

            byte[] bytes = new byte[4 * 4 + strBytes.Length];

            Protocol.Serialize(playerInfo.PlayerID, bytes, ref index);
            Protocol.Serialize(playerInfo.Score, bytes, ref index);
            Protocol.Serialize(playerInfo.WinRound, bytes, ref index);

            int colorKey = 0;
            if (playerInfo.PlayerColor == Color.white)
                colorKey = 0;
            else if (playerInfo.PlayerColor == Color.red)
                colorKey = 1;
            else if (playerInfo.PlayerColor == Color.blue)
                colorKey = 2;
            else if (playerInfo.PlayerColor == Color.green)
                colorKey = 3;
            else
                colorKey = 4;

            Protocol.Serialize(colorKey, bytes, ref index);

            strBytes.CopyTo(bytes, index);

            return bytes;
        }

        /// <summary> 反序列化UserInfo </summary>
        static object DeSerializePlayerInfo(byte[] bytes)
        {
            UserInfo p = new UserInfo();
            int index = 0;
            Protocol.Deserialize(out p.PlayerID, bytes, ref index);
            Protocol.Deserialize(out p.Score, bytes, ref index);
            Protocol.Deserialize(out p.WinRound, bytes, ref index);
            int key;

            Protocol.Deserialize(out key, bytes, ref index);

            switch (key)
            {
                case 0:
                    p.PlayerColor = Color.white;
                    break;
                case 1:
                    p.PlayerColor = Color.red;
                    break;
                case 2:
                    p.PlayerColor = Color.blue;
                    break;
                case 3:
                    p.PlayerColor = Color.green;
                    break;
                case 4:
                    p.PlayerColor = Color.yellow;
                    break;
                default:
                    p.PlayerColor = Color.black;
                    break;
            }

            p.NickName = UTF32Encoding.Default.GetString(bytes, index, bytes.Length - 4 * 4);

            return p;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// 设置RoomList
        /// </summary>
        void SetRoomInfo(List<RoomInfo> roomInfos)
        {
            foreach (RoomInfo info in roomInfos)
            {
                if (!RoomsList.Find((value) =>
                {
                    return value.GetComponent<RoomScript>().RoomName == info.Name;
                }))
                {
                    var gird = LobbyPanel.GetComponentInChildren<GridLayoutGroup>().gameObject;
                    GameObject item = Instantiate(ItemPrefab, gird.transform);
                    item.GetComponent<RoomScript>().SetRoomInfoInLobby(info, JoinCurrentRoom_Btn);
                    RoomsList.Add(item);
                }
            }
        }

        /// <summary>
        /// 重置RoomPanel面板信息
        /// </summary>
        void ResetRoomPanel()
        {
            //所有显示框变为白色，隐藏其文本框
            for (int i = 0; i < 4; i++)
            {
                var target = PlayersGirdsGroup.transform.GetChild(i);
                target.GetComponent<Image>().color = Color.white;
                var panel = target.GetChild(0).gameObject;
                if (panel.activeInHierarchy)
                {
                    panel.GetComponentInChildren<Text>().text = string.Empty;
                    panel.SetActive(false);
                }
            }

            //隐藏按键上的信息Text
            var button = RoomPanel.transform.GetChild(1).gameObject;

            button.transform.GetChild(0).gameObject.SetActive(false);
            button.transform.GetChild(0).gameObject.SetActive(false);
        }

        /// <summary>
        /// 设置玩家的显示框信息
        /// </summary>
        /// <param name="user">目标玩家</param>
        void SetRoomPanel(UserInfo user)
        {
            int id = user.PlayerID;
            Image target = PlayersGirdsGroup.transform.GetChild(id - 1).GetComponent<Image>();

            //设置显示框的颜色，根据房间内的玩家id
            switch (id)
            {
                case 1:
                    target.color = Color.red;
                    break;
                case 2:
                    target.color = Color.blue;
                    break;
                case 3:
                    target.color = Color.green;
                    break;
                case 4:
                    target.color = Color.yellow;
                    break;
            }
            GameObject text = target.transform.Find("Panel").gameObject;
            text.SetActive(true);
            text.GetComponentInChildren<Text>().text = user.NickName;
        }

        /// <summary>
        /// 设置按键显示信息，非主机显示Ready，主机玩家显示Start Game
        /// </summary>
        void SetButton()
        {
            GameObject button = RoomPanel.transform.GetChild(1).gameObject;
            button.transform.GetChild(0).gameObject.SetActive(false);
            button.transform.GetChild(1).gameObject.SetActive(false);
            if (PhotonNetwork.IsMasterClient)
            {
                button.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                button.transform.GetChild(1).gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        void StartGame()
        {
            foreach (UserInfo u in _allUsers)
            {
                if (!u.RPC_Master_IsReady)
                    return;
            }

            LocalDataManager.Instance.RoundNumber = RoundNumber;
            LocalDataManager.Instance.WinNumber = WinNumber;

            PhotonNetwork.CurrentRoom.IsOpen = false;

            photonView.RPC("RPC_LoadScene", RpcTarget.All);
        }

        /// <summary>
        /// 非主机玩家准备游戏
        /// </summary>
        void ReadyGame()
        {
            //玩家准备状态对所有玩家进行广播
            photonView.RPC("RPC_ALL_IsReady", RpcTarget.All, LocalDataManager.Instance.User.PlayerID, true);
        }

        /// <summary>
        /// 设置玩家在本房间内的玩家id
        /// </summary>
        /// <returns></returns>
        int SetPlayerId()
        {
            List<int> ids = new List<int>();
            for (int i = 0; i < _allUsers.Count; i++)
                ids.Add(_allUsers[i].PlayerID);
            ids.Sort();

            List<int> all = new List<int> { 1, 2, 3, 4 };
            ids = all.Except(ids).ToList();
            if (ids.Count != 0)
                return ids[0];
            Debug.LogError("Launcher/SetPlayerId Error : the playerId is -1 , list of ids is " + ids.Count);
            return -1;
        }

        void SetDisplayBox()
        {
            _allUsers.Add(LocalDataManager.Instance.User);
            SetRoomPanel(LocalDataManager.Instance.User);

            //Set LocalTankColor
            switch (LocalDataManager.Instance.User.PlayerID)
            {
                case 1:
                    LocalDataManager.Instance.User.PlayerColor = Color.red;
                    break;
                case 2:
                    LocalDataManager.Instance.User.PlayerColor = Color.blue;
                    break;
                case 3:
                    LocalDataManager.Instance.User.PlayerColor = Color.green;
                    break;
                case 4:
                    LocalDataManager.Instance.User.PlayerColor = Color.yellow;
                    break;
            }

            //Wall Others Players Set Local UserInfo.
            photonView.RPC("RPC_SetAllUserInfo", RpcTarget.Others, LocalDataManager.Instance.User);

            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        }

        /// <summary>
        /// 根据id获取玩家信息_C
        /// </summary>
        /// <param name="id">玩家id</param>
        UserInfo FindUser(int id)
        {
            foreach (UserInfo user in _allUsers)
            {
                if (user.PlayerID == id)
                    return user;
            }
            return null;
        }

        #endregion

        #region PUN2 RPCs

        /// <summary>
        /// 设置玩家的玩家信息
        /// </summary>
        /// <param name="user">加入房间的玩家的玩家信息</param>
        [PunRPC]
        void RPC_SetAllUserInfo(UserInfo user)
        {
            if (!_allUsers.Contains(user))
            {
                _allUsers.Add(user);
                SetRoomPanel(user);
            }
        }

        /// <summary> 请求房间内所有其余玩家发送自己的玩家信息 </summary>
        [PunRPC]
        void RPC_RequestGetAllUserInfo()
        {
            photonView.RPC("RPC_SetAllUserInfo", RpcTarget.Others, LocalDataManager.Instance.User);
        }

        /// <summary> 加载场景 </summary>
        [PunRPC]
        void RPC_LoadScene()
        {
            LocalDataManager.Instance.AllUser = _allUsers;
            PhotonNetwork.LoadLevel("03_NetWorkPVP_Game");
        }

        /// <summary>
        /// 向主机发送玩家准备状态信息
        /// </summary>
        /// <param name="playerId">玩家id</param>
        /// <param name="RPC_IsReady">准备状态</param>
        [PunRPC]
        void RPC_ALL_IsReady(int playerId, bool RPC_IsReady)
        {
            var user = FindUser(playerId);
            if (user == null)
                return;
            user.RPC_Master_IsReady = RPC_IsReady;
        }

        /// <summary>
        /// 玩家离开房间后的RPC远程调用
        /// </summary>
        [PunRPC]
        void RPC_LeaveRoom(int playerId)
        {
            _allUsers.Remove(FindUser(playerId));
            Transform panel = PlayersGirdsGroup.transform.GetChild(playerId - 1).GetChild(0);
            if (panel.gameObject.activeInHierarchy)
            {
                panel.GetComponentInChildren<Text>().text = string.Empty;
                panel.parent.GetComponent<Image>().color = Color.white;
                panel.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Laucher/RPC_LeaveRoom Warning ： Can't find the gameObject whithc PlayerId is " + playerId);
            }
        }

        /// <summary>
        /// 向主机请求分配PlayerId
        /// </summary>
        /// <param name="player"></param>
        [PunRPC]
        void RPC_Master_RequestSetPlayerId(Player player)
        {
            int id = SetPlayerId();
            photonView.RPC("RPC_Client_SetId", player, id);
        }

        /// <summary>
        /// 向目标客户机发送分配好的id
        /// </summary>
        /// <param name="id"></param>
        [PunRPC]
        void RPC_Client_SetId(int id)
        {
            _requestID = false;
            LocalDataManager.Instance.User.PlayerID = id;
        }
        #endregion

        #region Coroutine

        /// <summary>
        /// 等待ID分配
        /// </summary>
        IEnumerator WaitForRequestID()
        {
            yield return new WaitUntil(() => !_requestID);
            SetDisplayBox();
        }

        #endregion

        #region PUN2 Callbacks

        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
                PhotonNetwork.JoinLobby();
            }

            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            ProgressPanel.SetActive(false);
            ControlPanel.SetActive(true);
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });

            ProgressPanel.SetActive(false);
            CreateRoomPanel.SetActive(false);
            ControlPanel.SetActive(false);
            RoomPanel.SetActive(false);
            LobbyPanel.SetActive(true);

            PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.NickName+"'room", new RoomOptions { MaxPlayers = maxPlayersPerRoom });

        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            ProgressPanel.SetActive(false);
            CreateRoomPanel.SetActive(false);
            ControlPanel.SetActive(false);
            RoomPanel.SetActive(false);
            LobbyPanel.SetActive(true);

            JoinFailedWindowPanel.SetActive(true);
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount > 4)
            {
                Debug.LogError("PUN2/Laucher.cs/OnJoinedRoom Error: the room number of players only have 4 ");
            }

            //Set Button
            SetButton();

            //Set Panel.
            ProgressPanel.SetActive(false);
            ControlPanel.SetActive(false);
            RoomPanel.SetActive(true);
            LobbyPanel.SetActive(false);
            CreateRoomPanel.SetActive(false);

            //Join room ,if room player not only myself,request others send there UserInfo to init.
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPC_RequestGetAllUserInfo", RpcTarget.Others);
                photonView.RPC("RPC_Master_RequestSetPlayerId", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
            }
            else
            {
                LocalDataManager.Instance.User.RPC_Master_IsReady = true;
                LocalDataManager.Instance.User.PlayerID = SetPlayerId();
                SetDisplayBox();
            }

            //Set Local UserInfo.
            photonView.StartCoroutine(WaitForRequestID());

            RoomPanel.transform.GetChild(3).GetComponent<Text>().text = PhotonNetwork.CurrentRoom.MasterClientId.ToString();

            return;
        }

        public override void OnLeftRoom()
        {
            Debug.Log("Into OnLefrRoom");
            isConnecting = false;
            _requestID = true;
            LocalDataManager.Instance.Clear();
            _allUsers.Clear();
            return;
        }

        /// <summary>
        /// 加入大厅
        /// </summary>
        public override void OnJoinedLobby()
        {
            LobbyPanel.SetActive(true);
            ProgressPanel.SetActive(false);
            ControlPanel.SetActive(false);
            RoomPanel.SetActive(false);
            CreateRoomPanel.SetActive(false);
        }

        /// <summary>
        /// 房间信息更新后回调
        /// </summary>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            SetRoomInfo(roomList);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            SetButton();
            FindUser(LocalDataManager.Instance.User.PlayerID).RPC_Master_IsReady = true;
            Debug.Log(newMasterClient.NickName);
            Debug.Log(PhotonNetwork.MasterClient.NickName);
            Debug.Log(PhotonNetwork.LocalPlayer.NickName);
            Debug.Log(gameObject.GetComponentInChildren<PhotonView>().ViewID);
        }
        
        #endregion

        #region Buttons

        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect_Btn()
        {
            // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
            isConnecting = true;

            ProgressPanel.SetActive(true);
            ControlPanel.SetActive(false);
            RoomPanel.SetActive(false);
            LobbyPanel.SetActive(false);

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinLobby();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        /// <summary>
        /// 开始或者准备游戏按钮
        /// </summary>
        public void BeginGame_Btn()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartGame();
            }
            else
            {
                ReadyGame();
            }
        }

        /// <summary>
        /// 离开房间按钮
        /// </summary>
        public void LeaveRoom_Btn()
        {
            ResetRoomPanel();

            LobbyPanel.SetActive(true);
            RoomPanel.SetActive(false);
            ControlPanel.SetActive(false);
            ProgressPanel.SetActive(false);
            CreateRoomPanel.SetActive(false);
            JoinFailedWindowPanel.SetActive(false);

            photonView.RPC("RPC_LeaveRoom", RpcTarget.All, LocalDataManager.Instance.User.PlayerID);
            PhotonNetwork.LeaveRoom();
            #region Before Code
            //if (!PhotonNetwork.IsMasterClient)
            //{
            //    ResetRoomPanel();

            //    LobbyPanel.SetActive(true);
            //    RoomPanel.SetActive(false);
            //    ControlPanel.SetActive(false);
            //    ProgressPanel.SetActive(false);
            //    CreateRoomPanel.SetActive(false);
            //    JoinFailedWindowPanel.SetActive(false);

            //    photonView.RPC("RPC_LeaveRoom", RpcTarget.All, LocalDataManager.Instance.User.PlayerID);
            //    PhotonNetwork.LeaveRoom();
            //}
            //else
            //{

            //}
            #endregion
        }

        /// <summary>
        /// 随机加入房间
        /// </summary>
        public void QuickJoin_Btn()
        {
            ControlPanel.SetActive(false);
            LobbyPanel.SetActive(false);
            ProgressPanel.SetActive(true);
            RoomPanel.SetActive(false);
            PhotonNetwork.JoinRandomRoom();
        }

        /// <summary>
        /// 加入指定房间
        /// </summary>
        public void Join_Btn()
        {
            JoinRoomInputPanel.SetActive(true);
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        public void CreateRoom_Btn()
        {
            CreateRoomPanel.SetActive(true);
        }

        public void JoinCurrentRoom_Btn(string name)
        {
            PhotonNetwork.JoinRoom(name);
        }

        public void InputRoomInfoToCreateRoom_Btn()
        {
            var input = CreateRoomPanel.transform.GetChild(0).GetComponentInChildren<InputField>();
            
            PhotonNetwork.CreateRoom(input.text,new RoomOptions { MaxPlayers=maxPlayersPerRoom});
        }

        public void JoinRoomInput_Btn()
        {
            JoinRoomInputPanel.SetActive(false);
            var input = JoinRoomInputPanel.transform.GetChild(0).GetComponentInChildren<InputField>();
            //if (input.text == string.Empty)
            //{
            //    Debug.LogError("Launcher/JoinRoomInput_Btn Error : the InputField can't empty !");
            //    return;
            //}
            if (!PhotonNetwork.JoinRoom(input.text))
            {
                Debug.Log("!");
                JoinFailedWindowPanel.SetActive(true);
            }
        }

        public void CloseWindow_Btn()
        {
            JoinFailedWindowPanel.SetActive(false);
        }

        #endregion

    }
}