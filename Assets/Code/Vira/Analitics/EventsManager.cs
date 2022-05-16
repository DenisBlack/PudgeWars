using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIRA.Analytics
{
    public class EventsManager
    {
        private static EventsManager instance;

        public static EventsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventsManager();
                    return instance;
                }
                else
                {
                    return instance;
                }
            }
        }

        public static class AdsType
        {
            public static string inter => "interstitial";
            public static string rewarded => "rewarded";
        }

        public static class Available
        {
            public static string success => "success";
            public static string not_available => "not_available";
        }

        public void DebugLogEventsData(Dictionary<string, object> eventData)
        {
            string line = "";
            foreach(string key in eventData.Keys)
            {
                line += " " + key + eventData[key].ToString();
            }
            Debug.Log("SendEvent: " + line);
        }

        public void SendAdsAvaliable(string ad_type, string placement, string available, bool connection)
        {
            string videoAds = "video_ads_available";
            Dictionary<string, object> eventsData = new Dictionary<string, object>();
            eventsData.Add("ad_type", ad_type);
            eventsData.Add("placement", placement);
            eventsData.Add("result", available);
            eventsData.Add("connection", connection);

            AppMetrica.Instance.ReportEvent(videoAds, eventsData);
            AppMetrica.Instance.SendEventsBuffer();
        }

        public void SendAdsStart(string ad_type, string placement, bool connection, string result = "start")
        {
            string videoAds = "video_ads_started";
            Dictionary<string, object> eventsData = new Dictionary<string, object>();
            eventsData.Add("ad_type", ad_type);
            eventsData.Add("placement", placement);
            eventsData.Add("result", result);
            eventsData.Add("connection", connection);

            AppMetrica.Instance.ReportEvent(videoAds, eventsData);
            AppMetrica.Instance.SendEventsBuffer();
        }

        public void SendAdsWatch(string ad_type, string placement, bool connection, string result = "watched")
        {
            string videoAds = "video_ads_watch";
            Dictionary<string, object> eventsData = new Dictionary<string, object>();
            eventsData.Add("ad_type", ad_type);
            eventsData.Add("placement", placement);
            eventsData.Add("result", result);
            eventsData.Add("connection", connection);

            AppMetrica.Instance.ReportEvent(videoAds, eventsData);
            AppMetrica.Instance.SendEventsBuffer();
        }

        public void SendLevelStart(int level_number, string level_name, int level_count, int level_loop,
            bool level_random = false, string level_diff = "standart")
        {
            string name = "level_start";
            Dictionary<string, object> eventsData = new Dictionary<string, object>();
            eventsData.Add("level_number", level_number);
            eventsData.Add("level_name", level_name);
            eventsData.Add("level_count", level_count);
            eventsData.Add("level_diff", level_diff);
            eventsData.Add("level_loop", level_loop);
            eventsData.Add("level_random", level_random);

            DebugLogEventsData(eventsData);
            AppMetrica.Instance.ReportEvent(name, eventsData);
            AppMetrica.Instance.SendEventsBuffer();
        }

        public void SendLevelFinish(int level_number, string level_name, int level_count, int level_loop,
            string level_result, int time, int progress, bool level_random = false, string level_diff = "standart")
        {
            string name = "level_finish";
            Dictionary<string, object> eventsData = new Dictionary<string, object>();
            eventsData.Add("level_number", level_number);
            eventsData.Add("level_name", level_name);
            eventsData.Add("level_count", level_count);
            eventsData.Add("level_diff", level_diff);
            eventsData.Add("level_loop", level_loop);
            eventsData.Add("level_random", level_random);
            eventsData.Add("result", level_result);
            eventsData.Add("time", time);
            eventsData.Add("progress", progress);
            DebugLogEventsData(eventsData);
            AppMetrica.Instance.ReportEvent(name, eventsData);
            AppMetrica.Instance.SendEventsBuffer();
        }
    }

}
