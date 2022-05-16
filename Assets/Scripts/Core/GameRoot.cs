using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using JBStateMachine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VIRA.Analytics;


public class GameRoot : MonoBehaviourSingleton<GameRoot>
{
    //---------------------------- INSPECTOR ------------------------
    [SerializeField] private LevelBuilderState _levelBuilderState;
    [SerializeField] private GameState _gameState;
    [SerializeField] private EndGameState _endGameState;
    
    public GameSettings GameSettings;
    
    private StateMachine<States, Triggers> stateMachine;

    public LevelData CurrentLevelData;

    public FloatingJoystick Joystick;
    public BoxCollider EnemyBoxCollider;
    public BoxCollider AlliesBoxCollider;
    public EnemyMovePointsController EnemyMovePointsController;

    public LeaderBoardController LeaderBoardController;

    public bool GameCanStarted = false;

    public UnityEvent GameFinished;
    
    public event Action<int> LevelStarted = delegate { };            //int = level num
    public event Action<bool, int, int> LevelFinished = delegate { };

    public TMP_Text TouchCount;

    [Header("Shake Camera")]
    public CinemachineVirtualCamera CinemachineVirtualCamera;
    public float CurrentShakeTime;
    public bool CanShakeCamera;
    public Vector3 CameraFinisPos;
    public bool CanFinishPos;
    public float FinishPosSpeed;
    
    public Image VibrationButtonImage;

    public UnityEvent LevelInited;

    public bool IsGameFinished;
    
    public int CurrentLevel
    {
        get
        {
            if (debugSettings.Enabled)
            {
                if (currentDebugLevel.HasValue == false)
                {
                    currentDebugLevel = debugSettings.Level;
                }
                return currentDebugLevel.Value;
            }
            var level = PlayerPrefs.GetInt("Level");
            return level;
        }
        set
        {
            if (debugSettings.Enabled)
            {
                if (debugSettings.Loop)
                    return;
                currentDebugLevel++;
                return;
            }
            var val = value;
            PlayerPrefs.SetInt("Level", value);
        }
    }
    
    private static int? currentDebugLevel;
    private DebugSettings debugSettings => GameSettings.Instance.DebugSettings;
    
    private void Start()
    {
        CurrentLevelData = GameSettings.LevelSettings.GetLevel(CurrentLevel);
     
        var events = FindObjectOfType<LvlEvents>();
        events.Init();
        
        stateMachine = new StateMachine<States, Triggers>(States.Default);
        
        stateMachine.Configure(States.Default, null)
            .Permit(Triggers.GameInitialized, States.LevelInitialization);

        stateMachine.Configure(States.LevelInitialization, _levelBuilderState)
            .OnGetStateData(transition =>
            {
                return new LevelBuilderState.EnterData()
                {
                    Level = CurrentLevel
                };
            })
            .Permit(Triggers.LevelInitialized, States.GameState);

        stateMachine.Configure(States.GameState, _gameState)
            .OnGetStateData(transition =>
            {
                return new GameState.EnterData()
                {
                    Level = CurrentLevel
                };
            })
            .Permit(Triggers.CanFinishLevel, States.EndGame);

        stateMachine.Configure(States.EndGame, _endGameState);
        
        stateMachine.Fire(Triggers.GameInitialized);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            CanFinishPos = true;
        
        if (CanShakeCamera)
        {
            if (CurrentShakeTime >= 0)
            {
                CurrentShakeTime -= Time.deltaTime;
                if (CurrentShakeTime <= 0f)
                {
                    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                        CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                    cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
                    cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 0;

                    CanShakeCamera = false;
                }
            }
        }

        if (CanFinishPos)
        {
            var transposer = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = Vector3.Slerp(transposer.m_FollowOffset, CameraFinisPos, Time.deltaTime * GameSettings.Instance.EndCameraSpeed);

            if (transposer.m_FollowOffset == CameraFinisPos)
            {
                CanFinishPos = false;
            }
        }
    }

    public void Fire(Triggers trigger)
    {
        stateMachine.Fire(trigger);
    }

    public void SendLevelStarted()
    {
        LevelStarted.Invoke(CurrentLevel);
    }
    
    public void SendLevelFinished(bool isWin, int place)
    {
        LevelFinished.Invoke(isWin, 1, CurrentLevel);
    }
    
    public void GoNextLevel()
    {
        CurrentLevel++;
        SceneManager.LoadScene(0);
    }

    public void GoRetry()
    {
        SceneManager.LoadScene(0);
    }

    public void ShakeCamera()
    {
        if(CanShakeCamera)
            return;
        
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = GameSettings.IntensityShake;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = GameSettings.FrequencyShake;
        
        CurrentShakeTime = GameSettings.TimeShake;
        
        CanShakeCamera = true;
    }
    
    public bool CanVibration()
    {
        return PlayerPrefs.GetInt("Vibration") == 0;
    }
    
    private void UpdateVibrationButton()
    {
        var vibrationValue = PlayerPrefs.GetInt("Vibration");
        switch (vibrationValue)
        {
            case 0:
                VibrationButtonImage.color = Color.white;
                break;
            case 1:
                VibrationButtonImage.color = Color.gray;
                break;
        }
    }
    
    public void SwitchVibration()
    {
        int currentValue = 0;
        var vibrationValue = PlayerPrefs.GetInt("Vibration");
        switch (vibrationValue)
        {
            case 0:
                currentValue = 1;
                break;
            case 1:
                currentValue = 0;
                break;
        }
        PlayerPrefs.SetInt("Vibration", currentValue);

        UpdateVibrationButton();
    }
    
    public enum States
    {
        Default,
        LevelInitialization,
        GameState,
        EndGame,
    }

    public enum Triggers
    {
        GameInitialized,
        LevelInitialized,
        CanFinishLevel,
        RestartRequest,
    }
}
