/*using GameAnalyticsSDK;
using UnityEngine;

public class GameAnaliticsInit : MonoBehaviour
{
    // Start is called before the first frame update\

    void Awake()
    {
        GameAnalytics.Initialize();
        GameAnalytics.OnRemoteConfigsUpdatedEvent += MyOnRemoteConfigsUpdateFunction;
    }
    private static void MyOnRemoteConfigsUpdateFunction()
    {
        if (GameAnalytics.IsRemoteConfigsReady())
        {
            string value = GameAnalytics.GetABTestingVariantId();
            if (value == "0")
            {
                ABTestingTriggers.profile = false;
            }
            else
            {
                ABTestingTriggers.profile = true;
            }

        }
    }

    public static void SendLvlStartEvent(int lvl)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "RegularLvls", "Level_" + lvl.ToString());
    }

    public static void SendBonusLvlStartEvent(int bonusLvlCount)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "BonusLvls", "BonusLevel_" + bonusLvlCount.ToString());
    }

    public static void SendMenuQuitEvent(int lvl, double seconds)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Quit, "Menu", "AfterLevel_" + lvl.ToString(), "TimeOnMenu_" + seconds.ToString());
    }

    public static void SendLvlQuitEvent(int lvl, double seconds)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Quit, "RegularLvls", "Level_" + lvl.ToString(), "TimeOnLevel_" + seconds.ToString());
    }

    public static void SendBonusLvlQuitEvent(int bonusLvlCount, double seconds)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Quit, "BonusLvls", "BonusLevel_" + bonusLvlCount.ToString(), "TimeOnLevel_" + seconds.ToString());
    }

    public static void SendLvlRetryEvent(int lvl, int count)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "RegularLvls", "Level_" + lvl.ToString(), "Count_" + count.ToString());
    }

    public static void SendBonusLvlRetryEvent(int bonusLvlCount, int count)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "BonusLvls", "BonusLevel_" + bonusLvlCount.ToString(), "Count_" + count.ToString());
    }

    public static void SendLvlEndEvent(int lvl, float score)
    {
        if (score < 0.5f)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "level_" + lvl.ToString(), (int)(score * 100));
        }
        else
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level_" + lvl.ToString(), (int)(score * 100));
        }

    }

    public static void SendBonusLvlEndEvent(int lvl, float score)
    {
        if (score < 0.5f)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "BonusLvls", "level_" + lvl.ToString(), (int)(score * 100));
        }
        else
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "BonusLvls", "level_" + lvl.ToString(), (int)(score * 100));
        }
    }
}
*/