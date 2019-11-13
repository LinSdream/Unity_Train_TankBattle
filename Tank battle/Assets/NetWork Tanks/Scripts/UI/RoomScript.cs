using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyWork.Network
{
    public class RoomScript : MonoBehaviour
    {
        #region Fields

        [HideInInspector]
        public System.Action<string> JoinRoom;

        [HideInInspector]
        public string RoomName;

        Text _roomName;
        Text _roomNum;

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            _roomName = transform.GetChild(0).GetComponentInChildren<Text>();
            _roomNum = transform.GetChild(1).GetComponentInChildren<Text>();
        }

        #endregion

        #region Public Methods
        public void SetRoomInfoInLobby(Photon.Realtime.RoomInfo room,System.Action<string> joinRoomAction=null)
        {
            RoomName = room.Name;
            JoinRoom = joinRoomAction;
            _roomName.text = RoomName;
            _roomNum.text = room.PlayerCount.ToString() + "/4";
        }

        public void Join_Btn()
        {
            JoinRoom?.Invoke(_roomName.text);
        }
        
        #endregion
    }

}