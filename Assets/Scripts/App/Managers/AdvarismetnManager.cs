using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace TandC.RunIfYouWantToLive 
{
    public class AdvarismetnManager : IAdvarismetnManager, IService, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        //private static AdvarismetnManager _Instance;
        //public static AdvarismetnManager Instance
        //{
        //    get { return _Instance; }
        //    private set { _Instance = value; }
        //}
        private string _androidGameId = "5038095";
        private string _iosGameId = "5038095";

        private string _video;
        private string _rewardedVideo;
        private bool _testMode = false;

        private Action OnCompleteAds;
        private Action OnFailedAds;

        private string _gameId;
        public bool IsLoadRewardVideo { get; private set; }
        public bool IsLoadVideo { get; private set; }

        public void Init()
        {
#if UNITY_ANDROID
            _gameId = _androidGameId;
            _video = "Interstitial_Android";
            _rewardedVideo = "Rewarded_Android";
#elif UNITY_IOS

            _gameId = _iosGameId;
            _video = "Interstitial_iOS";
            _rewardedVideo = "Rewarded_iOS";
#endif
            Advertisement.Initialize(_gameId, _testMode, this);
        }

        public void OnInitializationComplete()
        {
                LoadAd();
                LoadRewardVideo();
            Debug.Log("Initialize Ads Complete");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Error Ads Initialize: ({error} - {message})");
        }

        public void Update()
        {

        }

        public void Dispose()
        {

        }

        public void LoadRewardVideo()
        {
            Advertisement.Load(_rewardedVideo, this);
        }

        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
        {
            if (adUnitId.Equals(_rewardedVideo) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                Debug.Log("Unity Ads Rewarded Ad Completed");
                // Grant a reward.
                OnCompleteAds?.Invoke();
                // Load another ad:
            }
            else
            {
                OnFailedAds?.Invoke();
            }

            if(adUnitId == _video) 
            {
                LoadAd();
            }
            if(adUnitId == _rewardedVideo) 
            {
                LoadRewardVideo();
            }
        }

        public void LoadAd()
        {
            // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
            Debug.Log("Loading Ad: " + _video);
            Advertisement.Load(_video, this);
        }

        public void ShowAdsVideo(Action CompleteEvent, Action FailedEvent) 
        {
            if (IsLoadRewardVideo) 
            {
                OnCompleteAds = CompleteEvent;
                OnFailedAds = FailedEvent;
                IsLoadRewardVideo = false;
                Advertisement.Show(_rewardedVideo, this);
            }
        }

        // Show the loaded content in the Ad Unit:
        public void ShowAd()
        {
            if (IsLoadVideo) 
            {
                IsLoadVideo = false;
                Debug.Log("Showing Ad: " + _video);
                Advertisement.Show(_video, this);
            }
        }

        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            if (adUnitId == _rewardedVideo) 
            {
                IsLoadRewardVideo = true;
            }
            else 
            {
                IsLoadVideo = true;
            }

        }

        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");

        }

        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowStart(string adUnitId) 
        {

        }
        public void OnUnityAdsShowClick(string adUnitId) 
        {

        }

    }
}

