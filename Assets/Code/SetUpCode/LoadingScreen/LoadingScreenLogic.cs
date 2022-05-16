using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIRA.Analytics;
using UnityEngine.SceneManagement;

public class LoadingScreenLogic : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image _fillImage;
    // Start is called before the first frame update
    void Start()
    {
        _fillImage.fillAmount = 0;
        StartCoroutine(ProgressBar());
    }

    IEnumerator ProgressBar()
    {
        while (!(MaxSdk.IsRewardedAdReady(AdsEvents.rewardedAdUnitId) || MaxSdk.IsInterstitialReady(AdsEvents.interstitialAdUnitId) || Application.internetReachability == NetworkReachability.NotReachable))
        {
            
            if(_fillImage.fillAmount < 0.55f)
            {
                _fillImage.fillAmount += 0.001f;
            }
            yield return null;
        }
        _fillImage.fillAmount = 0.55f;
        while (!(MaxSdk.IsRewardedAdReady(AdsEvents.rewardedAdUnitId) && MaxSdk.IsInterstitialReady(AdsEvents.interstitialAdUnitId) || Application.internetReachability == NetworkReachability.NotReachable))
        {
            if (_fillImage.fillAmount < 0.85f)
            {
                _fillImage.fillAmount += 0.001f;
            }
            
            yield return null;
        }
        _fillImage.fillAmount = 0.85f;
        AsyncOperation op = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        while (!op.isDone)
        {
            if (_fillImage.fillAmount < 0.95f)
            {
                _fillImage.fillAmount += 0.001f;
            }
            
            yield return null;
        }
        _fillImage.fillAmount = 1;
    }


}
