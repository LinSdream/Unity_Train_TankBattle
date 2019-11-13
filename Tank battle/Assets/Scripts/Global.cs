using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LS.Common;
using System;

namespace MyWork {

    public class Global : ASingletonBasis<Global>
    {

        #region Public Fields

        public SettingsFile Settings;

        public string CurrentLevel;
        
        public List<Level> Levels;

        public bool LoginSceneLevel = false;

        [HideInInspector]
        public string LoadNextSceneName;
        [HideInInspector]
        public string SaveDirPath =>Application.persistentDataPath+ "/"+Settings.SaveName;
        
        protected override void Start()
        {
            if (!IOHelper.IsDirectoryExists(SaveDirPath))
            {
                IOHelper.CreateDirectory(SaveDirPath);
            }
        }

        public void SetData(string fileName,List<LevelData> obj)
        {
            if (IOHelper.IsFileExists(SaveDirPath + "/"))
            {
                IOHelper.SetData(SaveDirPath + "/" + fileName, obj);
            }
            else
            {
                IOHelper.CreateFile(SaveDirPath + "/" + fileName, IOHelper.SerializeObject(obj));
            }
        }

        public object GetData(string fileName,Type type)
        {
            Debug.Log(SaveDirPath);
            if (IOHelper.IsFileExists(SaveDirPath + "/" + fileName))
                return IOHelper.GetData(SaveDirPath + "/" + fileName, type);
            Debug.LogWarning("Global/GetData Warning : Can't find the Save file ,the file path is " + SaveDirPath + "/" + fileName);
            return null;
        }

        public void InitLevelData()
        {
            List<LevelData> buf = new List<LevelData>();
            buf =GetData("Level1.sav", typeof(List<LevelData>)) as List<LevelData>;
            if (buf == null)
            {
                for(int i = 0; i < Levels.Count; i++)
                {
                    Levels[i].data = new LevelData
                    {
                        LevelName = Levels[i].LevelName
                    };
                }

                buf = new List<LevelData>();
                foreach(Level data in Levels)
                {
                    buf.Add(data.data);
                }

                SetData("Level1.sav",buf);
            }
            else
            {
                for(int i = 0; i < Levels.Count; i++)
                {
                    Levels[i].data = new LevelData
                    {
                        LevelName = buf[i].LevelName,
                        Status = buf[i].Status,
                        Score = buf[i].Score
                    };
                }
            }
        }

        public LevelData GetLevelData(string name)
        {
            for(int i = 0; i < Levels.Count; i++)
            {
                if (Levels[i].data.LevelName == name)
                    return Levels[i].data;
            }
            Debug.LogError("Global/GetLevelData Error : Can't get the level data , Level Name is " + name);
            return null;
        }
       
        #endregion

    }


}
