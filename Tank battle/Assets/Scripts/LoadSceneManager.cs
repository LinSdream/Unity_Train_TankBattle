using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyWork
{
    public class LoadSceneManager : MonoBehaviour
    {
        #region Public Fields

        public GameObject Menu;
        public GameObject LevelSelectPanel;
        public Text VersionText;

        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            Menu.SetActive(true);
            LevelSelectPanel.SetActive(false);
            if (Global.Instance.LoginSceneLevel)
            {
                Global.Instance.LoginSceneLevel = false;
                Btn_SingleModel();
            }
            VersionText.text = "Version: " + Global.Instance.Settings.Version;
        }

        #endregion

        #region Private Methods

        private void Load(string sceneName)
        {
            Global.Instance.LoadNextSceneName = sceneName;
            SceneManager.LoadScene("-1_TransitionScene");
        }

        #endregion

        #region Buttons

        public void Btn_Menu()
        {
            Menu.SetActive(true);
            LevelSelectPanel.SetActive(false);
        }

        public void Btn_SingleModel()
        {
            LevelSelectPanel.SetActive(true);
            Menu.SetActive(false);
            Global.Instance.InitLevelData();
            SetLevelPanel();
        }

        public void Btn_MultiplayerModel(string name)
        {
            Load(name);
            Menu.SetActive(false);
        }

        public void Btn_Exit()
        {
            List<LevelData> datas = new List<LevelData>();
            foreach (Level level in Global.Instance.Levels)
            {
                datas.Add(level.data);
            }
            Global.Instance.SetData("Level1.sav", datas);
            Application.Quit();
        }

        public void Btn_SelectedLevel(string level)
        {
            Global.Instance.CurrentLevel = level;
            Load("02_LocalPVE");
        }

        #endregion

        #region Private Methods
        void SetLevelPanel()
        {
            LevelSelectPanel.transform.GetChild(1).gameObject.SetActive(true);

            for (int i = 1; i < Global.Instance.Levels.Count; i++)
            {
                if (Global.Instance.Levels[i].data.Status == 0 && Global.Instance.Levels[i - 1].data.Status==0)
                    break;
                LevelSelectPanel.transform.GetChild(i+1).gameObject.SetActive(true);
            }
        }
        #endregion
    }

}