using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MyWork.Network.Tanks
{
    [System.Serializable]
    public class UserInfo
    {
        /// <summary> 玩家在房间内的房间玩家ID </summary>
        public int PlayerID = 0;
        /// <summary> 玩家昵称 </summary>
        public string NickName = "";
        /// <summary>  玩家得分  </summary>
        public int Score = 0;
        /// <summary> 玩家在本局游戏中获胜局数 </summary>
        public int WinRound = 0;
        /// <summary> 是否准备好 </summary>
        public bool RPC_Master_IsReady = false;
        /// <summary> 玩家游戏中的颜色</summary>
        public Color PlayerColor = Color.white;
    }

}