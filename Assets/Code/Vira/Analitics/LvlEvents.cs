using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIRA.Analytics
{
    public class LvlEvents : MonoBehaviour
    {
        System.DateTime _lvlStartTime;
        
        public void Init()
        {
            GameRoot.Instance.LevelFinished += LevelFinished;
            GameRoot.Instance.LevelStarted += LevelStart;
        }

        private void LevelStart(int Level)
        {
            _lvlStartTime = System.DateTime.Now;
            EventsManager.Instance.SendLevelStart(Level, "Level_" + Level.ToString(), Level, 1);
        }

        private void LevelFinished(bool won, int place, int level)
        {
            int progress = won == true ? 100 : 0;
            int seconds = (int)(System.DateTime.Now - _lvlStartTime).TotalSeconds;
            EventsManager.Instance.SendLevelFinish(level, "Level_" + level.ToString(), level, 1, (won ? "Won" : "Lost"), seconds, progress);
        }
    }
}

