using System;

namespace TandC.RunIfYouWantToLive
{
    public interface ITimerManager
    {
        void StopTimer(Action<object[]> handler);
        void AddTimer(Action<object[]> handler, object[] parameters = null, float time = 1, bool loop = false, bool storeTimer = false);
    }
}