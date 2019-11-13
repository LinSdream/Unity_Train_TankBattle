using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int NumRoundsToWin;
    public int StartDelay;
    public int EndDelay;
    public GameObject TankPrefab;
    public Text MessageText;
    public CameraControl CameraCol;
    public TankManager[] Tanks;

    int _roundNum;
    WaitForSeconds _startWait;
    WaitForSeconds _endWait;
    TankManager _roundWinner;
    TankManager _gameWinner;
    // Start is called before the first frame update
    void Start()
    {
        _startWait = new WaitForSeconds(StartDelay);
        _endWait = new WaitForSeconds(EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

        StartCoroutine(GameLoop());

    }

    /// <summary> spawn all tanks  </summary>
    void SpawnAllTanks()
    {
        for(int i = 0; i < Tanks.Length; i++)
        {
            Tanks[i].Instance = 
                Instantiate(TankPrefab, Tanks[i].SpawnPoint.position, 
                Tanks[i].SpawnPoint.rotation) as GameObject;
            
            Tanks[i].PlayerNum = i + 1;
            Tanks[i].Setup();
        }
    }

    /// <summary> set Camera position </summary>
    void SetCameraTargets()
    {
        Transform[] targets = new Transform[Tanks.Length];
        for(int i = 0; i < targets.Length; i++)
        {
            targets[i] = Tanks[i].Instance.transform;
        }
        CameraCol.Targets = targets;
    }

    #region Coroutine method uses to control the game's round logic

    /// <summary>游戏轮数逻辑循环</summary>
    IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());//轮数开始

        yield return StartCoroutine(RoundPlaying());//轮数正在进行，游玩ing

        yield return StartCoroutine(RoundEnding());//轮数结束，几份

        
        if(_gameWinner != null)
        {
            SceneManager.LoadScene("01_LocalPVP");
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }

    IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();
        CameraCol.SetStartPositionAndSize();
        _roundNum++;
        MessageText.text = "Round " + _roundNum;
        yield return _startWait;
    }

    IEnumerator RoundPlaying()
    {
        EnableTankControl();
        MessageText.text = string.Empty;
        while (!OneTankLeft())
        {
            yield return null;
        }
    }

    IEnumerator RoundEnding()
    {
        DisableTankControl();
        _roundWinner = null;
        _roundWinner = GetRoundWinner();
        if (_roundWinner != null)
            _roundWinner.Wins++;

        _gameWinner = GetGameWinner();

        MessageText.text = EndMessage();
        
        yield return _endWait;
    }

    void ResetAllTanks()
    {
        for(int i = 0; i < Tanks.Length; i++)
        {
            Tanks[i].Reset();
        }
    }

    void DisableTankControl()
    {
        for (int i = 0; i < Tanks.Length; i++)
        {
            Tanks[i].DisableControl();
        }
    }

    void EnableTankControl()
    {
        for (int i = 0; i < Tanks.Length; i++)
        {
            Tanks[i].EnableControl();
        }
    }

    bool OneTankLeft()
    {
        int tankCount = 0;
        for(int i = 0; i < Tanks.Length; i++)
        {
            if (Tanks[i].Instance.activeSelf)
                tankCount++;
        }
        return tankCount <= 1;
    }

    TankManager GetRoundWinner()
    {
        for(int i = 0; i < Tanks.Length; i++)
        {
            if (Tanks[i].Instance.activeSelf)
                return Tanks[i];
        }
        return null;
    }

    TankManager GetGameWinner()
    {
        for(int i = 0; i < Tanks.Length; i++)
        {
            if (Tanks[i].Wins == NumRoundsToWin)
                return Tanks[i];
        }
        return null;
    }

    string EndMessage()
    {
        string message = "DRAW";
        if (_roundWinner != null)
            message = _roundWinner.ColoredPlayerText + " wins the round !";
        message += "\n\n\n\n";
        for(int i = 0; i < Tanks.Length; i++)
        {
            message += Tanks[i].ColoredPlayerText + " : " + Tanks[i].Wins + " Wins\n";
        }
        if (_gameWinner != null)
            message = _gameWinner.ColoredPlayerText + " Wins the game !";
        return message;
    }

    #endregion
}
