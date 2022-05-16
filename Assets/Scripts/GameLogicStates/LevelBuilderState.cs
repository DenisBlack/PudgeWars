using System.Collections;
using System.Collections.Generic;
using JBStateMachine;
using UnityEngine;

public class LevelBuilderState : MonoBehaviour, IStateController
{
    public GameObject TutorialPanel;

    public void OnEntered(EnterDataBase data)
    {
        var enterData = data as EnterData;
        var level = enterData.Level;

        StartCoroutine(StartInit());
        
        GameRoot.Instance.SendLevelStarted();
    }

    IEnumerator DoTutorial()
    {
        TutorialPanel.SetActive(true);
        
        if (PlayerPrefs.GetInt("Level") == 0)
        {
            while (!Input.GetMouseButtonDown(0))
                yield return null;
        }
        
        TutorialPanel.SetActive(false);
        
        yield return null;
    }
    
    IEnumerator StartInit()
    {
        var currentLevel = GameRoot.Instance.CurrentLevelData; //GameSettings.Instance.LevelSettings.Current.GetLevel(level);

        if (currentLevel.Enemies.Length > 0)
        {
            foreach (var enemyData in currentLevel.Enemies)
            {
                var enemyClone = Instantiate(enemyData.Character);
                enemyClone.transform.position = enemyData.Position;
                enemyClone.transform.rotation = Quaternion.Euler(0,enemyData.Rotation.y,0);

                var characterData = GameSettings.Instance.CharacterData;
                characterData = new CharacterData()
                {
                    NickName = GetRandomName(),
                    Score = 0
                };
            
                var enemyController = enemyClone.GetComponent<BotController>();
                enemyController.SetData(characterData);
                enemyController.SpawnPoint = enemyData.Position;
            
                GameRoot.Instance.LeaderBoardController.CurrentData.Add(characterData);
            }

        }
        
        if (currentLevel.EnemiesSniper.Length > 0)
        {
            foreach (var enemyData in currentLevel.EnemiesSniper)
            {
                var enemyClone = Instantiate(enemyData.Character);
                enemyClone.transform.position = enemyData.Position;
                enemyClone.transform.rotation = Quaternion.Euler(0,enemyData.Rotation.y,0);

                var characterData = GameSettings.Instance.CharacterData;
                characterData = new CharacterData()
                {
                    NickName = GetRandomName(),
                    Score = 0
                };
            
                var enemyController = enemyClone.GetComponent<BotController>();
                enemyController.SetData(characterData);
                enemyController.SpawnPoint = enemyData.Position;
            
                GameRoot.Instance.LeaderBoardController.CurrentData.Add(characterData);
            }

        }

        if (currentLevel.Allies.Length > 0)
        {
            foreach (var alliesData in currentLevel.Allies)
            {
                var enemyClone = Instantiate(alliesData.Character);
                enemyClone.transform.position = alliesData.Position;
                enemyClone.transform.rotation = Quaternion.Euler(0,alliesData.Rotation.y,0);
            
                var characterData = GameSettings.Instance.CharacterData;
                characterData = new CharacterData()
                {
                    NickName = GetRandomName(),
                    Score = 0
                };
            
                var allyController = enemyClone.GetComponent<BotController>();
                allyController.SetData(characterData);
                allyController.SpawnPoint = alliesData.Position;
                GameRoot.Instance.LeaderBoardController.CurrentData.Add(characterData);
            }
        }

        //Player
        var playerData = currentLevel.Player;
        var playerClone = Instantiate(playerData.Character);
        playerClone.transform.position = playerData.Position;
        playerClone.transform.rotation = Quaternion.Euler(0, playerData.Rotation.y,0);
        
        var playerController = playerClone.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SpawnPoint = playerData.Position;
            
            playerController.CharacterData = new CharacterData()
            {
                NickName = "Player",
                Score = 0,
                IsPlayer = true
            };
        }
        GameRoot.Instance.LeaderBoardController.CurrentData.Add(playerController.CharacterData);
        GameRoot.Instance.LeaderBoardController.UpdatePlaces();

        //var arrow = Instantiate(GameRoot.Instance.GameSettings.ArrowPrefab);
        
        if (PlayerPrefs.GetInt("Level") == 0)
            yield return DoTutorial();
        
        GameRoot.Instance.Fire(GameRoot.Triggers.LevelInitialized);
        
        GameRoot.Instance.LevelInited.Invoke();
        
        yield return null;
    }

    private string GetRandomName()
    {
        var NickNames = GameSettings.Instance.NickNames.text;
        var lines = NickNames.Split('\n');
        return lines[Random.Range(0, lines.Length)];
    }
    
    public ExitDataBase OnExited()
    {
        return new ExitData()
        {
           
        };
    }

    public class ExitData : ExitDataBase
    {
  
    }

    public class EnterData : EnterDataBase
    {
        public int Level;
    }
}
