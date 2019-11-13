using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyWork
{
    public class LocalPvEManager : MonoBehaviour
    {
        #region Private Fields
        static LocalPvEManager _instance;
        #endregion

        #region Public Fields
        public static LocalPvEManager Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LocalPvEManager>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        _instance = obj.AddComponent<LocalPvEManager>();
                    }
                }
                return _instance;
            }
        }

        public LevelData Data;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            if(Global.Instance.CurrentLevel==null || Global.Instance.CurrentLevel == string.Empty)
            {
                Debug.LogError("LocalPvEManager/Error : Current Level Name is null or Empty !");
                return;
            }
            GameObject prefab = Resources.Load("Level/" + Global.Instance.CurrentLevel) as GameObject;
            Instantiate(prefab);

            Data = new LevelData();

            Data = Global.Instance.GetLevelData(Global.Instance.CurrentLevel);
        }

        #endregion

        #region Public Methods

        public bool GameStart()
        {
            return true;
        }

        public void GameOver(Action action=null)
        {
            if (Data.Status == 0)
            {
                Global.Instance.Levels.Find((data) =>
                {
                    data.data = Data;
                    return data;
                }).data.Status = 1;
            }

            List<LevelData> datas = new List<LevelData>();
            foreach(Level level in Global.Instance.Levels)
            {
                datas.Add(level.data);
            }

            Global.Instance.SetData("Level1.sav", datas);
            action?.Invoke(); 
        }

        #endregion
    }

}