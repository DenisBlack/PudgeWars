using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VIRA.Analytics
{
    public class AdsEvents : MonoBehaviour
    {
        public static event Action InterstialDismissed = delegate { };
        public static string interstitialAdUnitId = "343c17802fae212b";
        int retryAttempt;
        private static AdsEvents instance = null;
        public int InterTime;

        public static AdsEvents Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AdsEvents>();
                }

                return instance;
            }
        }

        IEnumerator LoadRW()
        {
            while (true)
            {
                if (MaxSdk.IsRewardedAdReady(rewardedAdUnitId) == false)
                {
                    LoadRewardedAd();
                }
                yield return new WaitForSeconds(10f);
            }
        }

        public void Start()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += MaxSdkCallbacks_OnSdkInitializedEvent;
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
                if (sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.Applies)
                {
                    if (PlayerPrefs.GetInt(_accepted, 0) == 0)
                    {
                        // SimpleGDPR.ShowDialog(new TermsOfServiceDialog().
                        //  SetTermsOfServiceLink("https://www.applovin.com/terms/").
                        //  SetPrivacyPolicyLink("https://www.applovin.com/privacy/"),
                        //  TermsOfServiceDialogClosed);
                    }
                }
                else if (sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.DoesNotApply)
                {
                    // No need to show consent dialog, proceed with initialization
                }
                else
                {
                    if (PlayerPrefs.GetInt(_accepted, 0) == 0)
                    {
                        // SimpleGDPR.ShowDialog(new TermsOfServiceDialog().
                        // SetTermsOfServiceLink("https://www.applovin.com/terms/").
                        // SetPrivacyPolicyLink("https://www.applovin.com/privacy/"),
                        // TermsOfServiceDialogClosed);
                    }
                }
            };
            MaxSdk.SetSdkKey("6AQkyPv9b4u7yTtMH9PT40gXg00uJOTsmBOf7hDxa_-FnNZvt_qTLnJAiKeb5-2_T8GsI_dGQKKKrtwZTlCzAR");
            MaxSdk.InitializeSdk();

            //StartCoroutine(CheckNoDoing());
            StartCoroutine(AddCoolDown());
        }

        const string _accepted = "accepted";

        private void TermsOfServiceDialogClosed()
        {
            PlayerPrefs.SetInt(_accepted, 1);
            MaxSdk.SetHasUserConsent(true);
        }

        private void OnApplicationQuit()
        {
            if (PlayerPrefs.GetInt(_accepted, 0) == 0)
            {
                MaxSdk.SetHasUserConsent(false);
            }
        }

        public void InitializeInterstitialAds()
        {
            // Attach callback
            MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialDismissedEvent;


            // Load the first interstitial
            LoadInterstitial();
        }

        private void MaxSdkCallbacks_OnSdkInitializedEvent(MaxSdkBase.SdkConfiguration obj)
        {
            //Debug.LogError("MaxSdkCallbacks_OnSdkInitializedEvent");

            MaxSdk.SetTestDeviceAdvertisingIdentifiers(new string[] { "660449e1-7ae7-4872-a242-197ae6151f83" });
            MaxSdk.SetTestDeviceAdvertisingIdentifiers(new string[] { "728e3ace-7232-4e2a-9042-ob7677186249" });
            MaxSdk.SetTestDeviceAdvertisingIdentifiers(new string[] { "eaa6e5dd-4fbf-43f4-b8b1-70447ddae709" });


            InitializeInterstitialAds();
            InitializeRewardedAds();
            StartCoroutine(LoadRW());

            //MaxSdk.ShowMediationDebugger();
            //Debug.LogError("ShowMediationDebugger");
        }

        private void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(interstitialAdUnitId);
        }

        private void OnInterstitialLoadedEvent(string adUnitId)
        {
            // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'

            // Reset retry attempt
            retryAttempt = 0;
        }

        private void OnInterstitialFailedEvent(string adUnitId, int errorCode)
        {
            // Interstitial ad failed to load 
            // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)

            retryAttempt++;
            double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttempt));

            Invoke("LoadInterstitial", (float)retryDelay);
        }

        private void InterstitialFailedToDisplayEvent(string adUnitId, int errorCode)
        {
            // Interstitial ad failed to display. We recommend loading the next ad
            LoadInterstitial();
        }

        private void OnInterstitialDismissedEvent(string adUnitId)
        {
            // Interstitial ad is hidden. Pre-load the next ad
            InterstialDismissed.Invoke();
            LoadInterstitial();
        }


        Coroutine routine = null;

        /* IEnumerator CheckNoDoing()
         {
             while (true)
             {
                 if (Input.GetMouseButton(0) == false)
                 {
                     if (routine == null)
                     {
                         routine = StartCoroutine(WaitTime());
                     }
                 }
                 else
                 {
                     if (routine != null)
                     {
                         StopCoroutine(routine);
                         routine = null;
                     }
                 }

                 yield return null;
             }
         }

         IEnumerator WaitTime()
         {
             yield return new WaitForSeconds(30f);
             if (AdsEvents.canShow && MaxSdk.IsInterstitialReady(AdsEvents.interstitialAdUnitId))
             {
                 MaxSdk.ShowInterstitial(AdsEvents.interstitialAdUnitId);
                 if (Application.internetReachability == NetworkReachability.NotReachable)
                 {
                     Debug.Log("No Internet");
                     AdsEvents.AppMetricaEvents.Instance.SendAdsAvaliable(AdsEvents.AppMetricaEvents.AdsType.inter,
                         "inaction", AdsEvents.AppMetricaEvents.Available.success, false);
                     AdsEvents.AppMetricaEvents.Instance.SendAdsStart(AdsEvents.AppMetricaEvents.AdsType.inter, "inaction",
                         false);
                     AdsEvents.AppMetricaEvents.Instance.SendAdsWatch(AdsEvents.AppMetricaEvents.AdsType.inter, "inaction",
                         false);
                 }
                 else
                 {
                     AdsEvents.AppMetricaEvents.Instance.SendAdsAvaliable(AdsEvents.AppMetricaEvents.AdsType.inter,
                         "inaction", AdsEvents.AppMetricaEvents.Available.success, true);
                     AdsEvents.AppMetricaEvents.Instance.SendAdsStart(AdsEvents.AppMetricaEvents.AdsType.inter, "inaction",
                         true);
                     AdsEvents.AppMetricaEvents.Instance.SendAdsWatch(AdsEvents.AppMetricaEvents.AdsType.inter, "inaction",
                         true);
                 }
             }

             routine = null;
         }
         */
        public static bool canShow = false;

        IEnumerator AddCoolDown()
        {
            while (true)
            {
                yield return new WaitUntil(() => canShow == false);
                yield return new WaitForSeconds(InterTime);
                //canShow = true && (VIRA.PlayerStats.PlayerStatistics.Level >= 2);
            }
        }


        public static string rewardedAdUnitId = "d4b191a582013007";
        public static event Action GetReward = delegate { };
        public static event Action RewardDismissed = delegate { };
        int retryAttemptR;

        public void InitializeRewardedAds()
        {
            // Attach callback
            MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.OnRewardedAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.OnRewardedAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.OnRewardedAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            // Load the first RewardedAd
            LoadRewardedAd();
        }

        private void LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(rewardedAdUnitId);
        }

        private void OnRewardedAdLoadedEvent(string adUnitId)
        {
            // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'

            // Reset retry attempt
            retryAttemptR = 0;
        }

        private void OnRewardedAdFailedEvent(string adUnitId, int errorCode)
        {
            // Rewarded ad failed to load 
            // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)

            retryAttemptR++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

            Invoke("LoadRewardedAd", (float)retryDelay);
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, int errorCode)
        {
            // Rewarded ad failed to display. We recommend loading the next ad
            LoadRewardedAd();
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId)
        {
        }

        private void OnRewardedAdClickedEvent(string adUnitId)
        {
        }

        private void OnRewardedAdDismissedEvent(string adUnitId)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            RewardDismissed.Invoke();
            LoadRewardedAd();
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward)
        {
            // Rewarded ad was displayed and user should receive the reward
            GetReward.Invoke();
        }


        public static int time;


        float timer;
        Coroutine routineTimer = null;

        IEnumerator StartTimer()
        {
            timer = 0;
            while (true)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        public void StartTimerRoutine()
        {
            if (routineTimer == null)
            {
            }
            else
            {
                StopCoroutine(routineTimer);
                routineTimer = null;
            }

            routineTimer = StartCoroutine(StartTimer());
        }

        public void EndTimer()
        {
            StopCoroutine(routineTimer);
            routineTimer = null;
            time = (int)(timer % 60);
            timer = 0;
        }
    }
}
