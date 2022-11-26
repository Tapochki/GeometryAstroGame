using System;

namespace TandC.RunIfYouWantToLive
{
    public interface IAdvarismetnManager
    {
        bool IsLoadRewardVideo { get; }
        bool IsLoadVideo { get; }

        void ShowAdsVideo(Action CompleteEvent, Action FailedEvent);
        void LoadAd();
        void ShowAd();
    }
}