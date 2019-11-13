using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LS.Common;

namespace MyWork.Network.Tanks
{
    public class LocalDataManager : ASingletonBasis<LocalDataManager>
    {
        #region Public Fields

        //[HideInInspector]
        public UserInfo User;

        //[HideInInspector]
        public List<UserInfo> AllUser;
        
        public int RoundNumber = 0;

        public int WinNumber = 0;

        #endregion

        #region MonoBehaviour Callbacks

        protected override void Awake()
        {
            base.Awake();
            User = new UserInfo();
            AllUser = new List<UserInfo>();
        }

        private void Start()
        {
            if (PlayerPrefs.GetString("PlayerName") == null && PlayerPrefs.GetString("PlayerName") == string.Empty)
                return;
            User.NickName = PlayerPrefs.GetString("PlayerName");
        }

        #endregion

        #region Public  Methods

        public void Clear()
        {
            RoundNumber = 0;
            WinNumber = 0;
            User.PlayerID = 0;
            User.WinRound = 0;
            User.Score = 0;
            User.PlayerColor = Color.white;
            AllUser.Clear();
        }

        public void ResetInfoAfterGameEnd()
        {
            foreach(UserInfo user in AllUser)
            {
                user.WinRound = 0;
                user.Score = 0;
                user.RPC_Master_IsReady = false;
            }
        }
        
        public void ResetReadyInfoALl()
        {
            foreach(UserInfo info in AllUser)
            {
                info.RPC_Master_IsReady = false;
            }
        }

        /// <summary>
        /// 根据id获取玩家信息_C
        /// </summary>
        /// <param name="id">玩家id</param>
        public UserInfo FindUser(int id)
        {
            foreach (UserInfo user in AllUser)
            {
                if (user.PlayerID == id)
                    return user;
            }
            return null;
        }

        #endregion
    }

}