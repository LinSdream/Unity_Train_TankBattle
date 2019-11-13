using MyWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TanksAI
{
    public class AIGameManager : MonoBehaviour
    {

        #region Public Fields

        public GameObject Game;
        public GameObject TimeLine;

      //  public GameObject PlayerPrefab;
        public int SpawnNumberOfAITanks;
        public GameObject[] TanksAIPrefabs;
        public AITankManger[] TanksAIForManagers;
        //public TankManager[] PlayerForManager;
        
        public List<Transform> WayPointsForAI;
        public Transform[] SpawnPointsForAI;

        public GameObject Player;

        public GameObject LosePanel;
        public GameObject SettingsPanel;
        public Text ScoreText;

        public float MinSpawnAITanksTime = 1f;
        public float MaxSpawnAITanksTime = 5f;

        #endregion

        #region Private Fields

        float _spawnAITankTime;
        int _AITanksCount;
        Animator _settingsAnim;
        bool _settingsOpen = false;
        bool _endSpawn = false;
        bool _win = false;
        int _deadEnemyCount = 0;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            TanksAIForManagers = new AITankManger[SpawnNumberOfAITanks];
            _spawnAITankTime = 0f;
            _AITanksCount = 0;
        }

        private void Start()
        {
            if (LocalPvEManager.Instance.Data.Status == 0)
            {
                if (TimeLine != null)
                {
                    Game.SetActive(false);
                    StartCoroutine(WaitForAnimator());
                    TimeLine.SetActive(true);
                }
                else
                {
                    Game.SetActive(true);

                    _settingsAnim = SettingsPanel.GetComponent<Animator>();
                    //  SpawnAllPlayer();
                    InitAIBaseInfo();
                    StartCoroutine(WaitForGameStart());
                }
                
            }
            else
            {
                Game.SetActive(true);
                if (TimeLine != null)
                {
                    TimeLine.SetActive(false);
                }

                _settingsAnim = SettingsPanel.GetComponent<Animator>();
                //  SpawnAllPlayer();
                InitAIBaseInfo();
                StartCoroutine(WaitForGameStart());
            }

            
        }

        private void Update()
        {
            if (_settingsOpen)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.currentSelectedGameObject == null)
                    {
                        Time.timeScale = 1;
                        _settingsAnim.SetTrigger("Close");
                        _settingsOpen = false;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods
        //void SpawnAllPlayer()
        //{
        //    if (PlayerPrefab == null)
        //    {
        //        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        //        Debug.Log(player.Length);
        //        PlayerForManager = new TankManager[player.Length];

        //        Debug.Log(PlayerForManager.Length);
        //        for(int i = 0; i < player.Length; i++)
        //        {
        //            PlayerForManager[i] = new TankManager();
        //            PlayerForManager[i].Instance = player[i];
        //            PlayerForManager[i].PlayerNum = i + 1;
        //            PlayerForManager[i].Setup();
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < PlayerForManager.Length; i++)
        //        {
        //            PlayerForManager[i].Instance= Instantiate(PlayerPrefab, PlayerForManager[i].SpawnPoint.position,
        //            PlayerForManager[i].SpawnPoint.rotation) as GameObject; ;
        //            PlayerForManager[i].PlayerNum = i + 1;
        //            PlayerForManager[i].Setup();
        //        }
        //    }

        //}

        void TankDead()
        {
            _deadEnemyCount++;
            ScoreText.text = _deadEnemyCount.ToString() + " / " + SpawnNumberOfAITanks.ToString();
        }

        void InitAIBaseInfo()
        {
            for(int i = 0; i < SpawnNumberOfAITanks; i++)
            {
                TanksAIForManagers[i] = new AITankManger();
                TanksAIForManagers[i].SpawnPoint = RandomTanksAIPoint();
                TanksAIForManagers[i].PlayerNum = 101;
            }
        }

        GameObject RandomTankAIPrefab()
        {
            int index = Random.Range(0, TanksAIPrefabs.Length - 1);
            return TanksAIPrefabs[index];
        }

        Transform RandomTanksAIPoint()
        {
            int index = Random.Range(0, SpawnPointsForAI.Length - 1);
            return SpawnPointsForAI[index];
        }

        #endregion

        #region Coroutines

        IEnumerator SpawnAITank()
        {
            
            yield return new WaitForSeconds(_spawnAITankTime);

            TanksAIForManagers[_AITanksCount].Instance = Instantiate(RandomTankAIPrefab(), 
                TanksAIForManagers[_AITanksCount].SpawnPoint.position,
                TanksAIForManagers[_AITanksCount].SpawnPoint.rotation) as GameObject;

            TanksAIForManagers[_AITanksCount].SetupAI(WayPointsForAI,TankDead);

            _spawnAITankTime = Random.Range(MinSpawnAITanksTime, MaxSpawnAITanksTime);

            _AITanksCount++;
            
            if (_AITanksCount < SpawnNumberOfAITanks)
            {
                yield return StartCoroutine(SpawnAITank());
            }
            else
            {
                _endSpawn = true;
                yield return null;
            }
        }

        IEnumerator WaitForAnimator()
        {
            yield return new WaitForSeconds(17);
            Game.SetActive(true);
            TimeLine.SetActive(false);

            _settingsAnim = SettingsPanel.GetComponent<Animator>();
            //  SpawnAllPlayer();
            InitAIBaseInfo();
            StartCoroutine(WaitForGameStart());
        }

        IEnumerator WaitForSettlement()
        {
            if (!_win)
            {
                LosePanel.SetActive(true);
                LosePanel.GetComponent<Animator>().SetTrigger("Lose");
            }
            yield return new WaitForSeconds(10);
            if (!_win)
            {
                Btn_Restart();
            }
            else
            {
                LocalPvEManager.Instance.GameOver(() => {
                    Global.Instance.LoginSceneLevel = true;
                    Global.Instance.LoadNextSceneName = "00_Menu";
                    SceneManager.LoadScene(1);
                });
            }
            
        }

        IEnumerator WaitForGameStart()
        {
            yield return new WaitUntil(()=>
            {
                return LocalPvEManager.Instance.GameStart();
            });
            StartCoroutine(SpawnAITank());
            StartCoroutine(GameOver());
            ScoreText.text = _deadEnemyCount.ToString() + " / " + SpawnNumberOfAITanks.ToString();
        }

        IEnumerator GameOver()
        {
            yield return new WaitUntil(() =>
            {
                if (!Player.activeInHierarchy)//如果玩家死亡，游戏结束
                    return true;
                if (!_endSpawn)//如果tanks还没生产完，游戏还未结束
                    return false;
                foreach (AITankManger tank in TanksAIForManagers)
                {
                    if (tank.Instance.activeSelf)//如果tank全部生产完，但存在未被消灭的，则游戏还未结束
                        return false;
                }
                _win = true;
                return true;
            });
            LocalPvEManager.Instance.Data.Score = _deadEnemyCount;
            StartCoroutine(WaitForSettlement());
        }

        #endregion

        #region Buttons

        public void Btn_Settings()
        {
            _settingsAnim.SetTrigger("Open");
            _settingsOpen = true;
            Time.timeScale = 0;
        }

        public void Btn_Restart()
        {
            Time.timeScale = 1;
            Global.Instance.LoadNextSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(1);
        }

        public void Btn_Menu()
        {
            Time.timeScale = 1;
            Global.Instance.LoadNextSceneName = "00_Menu";
            SceneManager.LoadScene(1);
        }

        public void Btn_Exit()
        {
            Application.Quit();
        }
        #endregion

        #region Tank Game Manager Debug

        //public int NumRoundsToWin;
        //public int StartDelay;
        //public int EndDelay;
        //public GameObject TankPrefab;
        //public Text MessageText;
        //public CameraControl CameraCol;
        //public GameObject[] TankAIPrefab;
        //public TankManager[] Tanks;
        //public AITankManger[] AITanks;
        //public List<Transform> WayPointsForAIToPatrol;

        //int _roundNum;
        //WaitForSeconds _startWait;
        //WaitForSeconds _endWait;
        //TankManager _roundWinner;
        //TankManager _gameWinner;
        //// Start is called before the first frame update
        //void Start()
        //{
        //    _startWait = new WaitForSeconds(StartDelay);
        //    _endWait = new WaitForSeconds(EndDelay);

        //    SpawnAllTanks();
        //    SetCameraTargets();

        //    StartCoroutine(GameLoop());

        //}

        ///// <summary> spawn all tanks  </summary>
        //void SpawnAllTanks()
        //{
        //    for (int i = 0; i < Tanks.Length; i++)
        //    {
        //        Tanks[i].Instance =
        //            Instantiate(TankPrefab, Tanks[i].SpawnPoint.position,
        //            Tanks[i].SpawnPoint.rotation) as GameObject;

        //        Tanks[i].PlayerNum = i + 1;
        //        Tanks[i].Setup();
        //    }

        //    for(int i = 0; i < AITanks.Length; i++)
        //    {
        //        AITanks[i].Instance=
        //            Instantiate(RandomTankAIPrefab(), AITanks[i].SpawnPoint.position,
        //            AITanks[i].SpawnPoint.rotation) as GameObject;
        //        AITanks[i].PlayerNum = 101;
        //        AITanks[i].SetupAI(WayPointsForAIToPatrol);
        //    }
        //}

        ///// <summary> set Camera position </summary>
        //void SetCameraTargets()
        //{
        //    Transform[] targets = new Transform[Tanks.Length];
        //    for (int i = 0; i < targets.Length; i++)
        //    {
        //        targets[i] = Tanks[i].Instance.transform;
        //    }
        //    CameraCol.Targets = targets;
        //}

        ///// <summary>游戏轮数逻辑循环</summary>
        //IEnumerator GameLoop()
        //{
        //    yield return StartCoroutine(RoundStarting());//轮数开始

        //    yield return StartCoroutine(RoundPlaying());//轮数正在进行，游玩ing

        //    yield return StartCoroutine(RoundEnding());//轮数结束，几份


        //    if (_gameWinner != null)
        //    {
        //        SceneManager.LoadScene("01_LocalPVP");
        //    }
        //    else
        //    {
        //        StartCoroutine(GameLoop());
        //    }
        //}

        //IEnumerator RoundStarting()
        //{
        //    ResetAllTanks();
        //    DisableTankControl();
        //    CameraCol.SetStartPositionAndSize();
        //    _roundNum++;
        //    MessageText.text = "Round " + _roundNum;
        //    yield return _startWait;
        //}

        //IEnumerator RoundPlaying()
        //{
        //    EnableTankControl();
        //    MessageText.text = string.Empty;
        //    while (!OneTankLeft())
        //    {
        //        yield return null;
        //    }
        //}

        //IEnumerator RoundEnding()
        //{
        //    DisableTankControl();
        //    _roundWinner = null;
        //    _roundWinner = GetRoundWinner();
        //    if (_roundWinner != null)
        //        _roundWinner.Wins++;

        //    _gameWinner = GetGameWinner();

        //    MessageText.text = EndMessage();

        //    yield return _endWait;
        //}

        //void ResetAllTanks()
        //{
        //    for (int i = 0; i < Tanks.Length; i++)
        //    {
        //        Tanks[i].Reset();
        //    }
        //}

        //void DisableTankControl()
        //{
        //    for (int i = 0; i < Tanks.Length; i++)
        //    {
        //        Tanks[i].DisableControl();
        //    }
        //}

        //void EnableTankControl()
        //{
        //    for (int i = 0; i < Tanks.Length; i++)
        //    {
        //        Tanks[i].EnableControl();
        //    }
        //}

        //bool OneTankLeft()
        //{
        //    int tankCount = 0;
        //    for (int i = 0; i < Tanks.Length; i++)
        //    {
        //        if (Tanks[i].Instance.activeSelf)
        //            tankCount++;
        //    }
        //    return tankCount <= 1;
        //}

        //TankManager GetRoundWinner()
        //{
        //    for (int i = 0; i < Tanks.Length; i++)
        //    {
        //        if (Tanks[i].Instance.activeSelf)
        //            return Tanks[i];
        //    }
        //    return null;
        //}

        //TankManager GetGameWinner()
        //{
        //    for (int i = 0; i < Tanks.Length; i++)
        //    {
        //        if (Tanks[i].Wins == NumRoundsToWin)
        //            return Tanks[i];
        //    }
        //    return null;
        //}

        //string EndMessage()
        //{
        //    string message = "DRAW";
        //    if (_roundWinner != null)
        //        message = _roundWinner.ColoredPlayerText + " wins the round !";
        //    message += "\n\n\n\n";
        //    for (int i = 0; i < Tanks.Length; i++)
        //    {
        //        message += Tanks[i].ColoredPlayerText + " : " + Tanks[i].Wins + " Wins\n";
        //    }
        //    if (_gameWinner != null)
        //        message = _gameWinner.ColoredPlayerText + " Wins the game !";
        //    return message;
        //}

        //GameObject RandomTankAIPrefab()
        //{
        //    int index = Random.Range(0, TankAIPrefab.Length - 1);
        //    return TankAIPrefab[index];
        //}

        #endregion

    }

}