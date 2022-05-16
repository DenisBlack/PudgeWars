using System;
//using Facebook.Unity;
using UnityEngine;

namespace VIRA.Analytics
{
    public class Analytics : MonoBehaviour
    {
        /*private event Action OnFBInitialized;

        private void Awake()
        {
            OnFBInitialized += Analytics_OnFBInitialized;
            OnApplicationRun();
        }

        private void Analytics_OnFBInitialized()
        {
            FB.ActivateApp();
            FB.GetAppLink(DeepLinkCallback);
            FB.Mobile.FetchDeferredAppLinkData(DeferredDeepLinkCallback);
        }

        private void DeepLinkCallback(IAppLinkResult result)
        {
            if (!String.IsNullOrEmpty(result.Url))
            {
                var index = (new Uri(result.Url)).Query.IndexOf("request_ids");
                if (index == -1)
                {
                    Debug.LogError("DeepLinkCallback ALARM!!!!!!!");
                }
            }
        }

        private void DeferredDeepLinkCallback(IAppLinkResult result)
        {
            if (String.IsNullOrEmpty(result.Url))
            {
                Debug.Log("DeferredDeepLinkCallback_ALARM!!!!!!!");
            }
        }

        void OnApplicationRun()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                FB.Init(() => { OnFBInitialized.Invoke(); });
            }
        }

        /*void OnPlayButtonClick()
        {
            SendEvent("OnPlayButtonClick");
        }

        void OnStartLevelPlay() {
            //SendEvent(Constants.ON_START_EVENT_PLAY);
        }

        //void OnReadyForLevelCompleate() => SendEvent(Constants.ON_READY_FOR_LEVEL_COMPLETE);
        //void OnNextButtonClick() => SendEvent(Constants.ON_NEXT_BUTTON_CLICK, _sceneController.porcentOfWin);


        void OnFinishButtonClick(int level)
        {

        }

        public string GetEventType(string byNameEvent)
        {
            switch (byNameEvent)
            {
                //case Constants.ON_FINISH_BUTTON_CLICK : return AppEventName.AchievedLevel;
                //case Constants.ON_START_EVENT_PLAY : return AppEventName.ViewedContent;
                //case Constants.ON_NEXT_BUTTON_CLICK: return AppEventName.CompletedTutorial;
            }

            return byNameEvent;
        }

        void SendEvent(string eventName, int val = 0)
        {
            //var fbEvent = new Dictionary<string, object>();
            //fbEvent[eventName] = eventName;
            /*FB.LogAppEvent(
                GetEventType(eventName),
                val
            //fbEvent
            );
            string str = eventName;
            if (val > 0) str += val.ToString();
            Debug.Log(str);*/
        //}*/
    }
}