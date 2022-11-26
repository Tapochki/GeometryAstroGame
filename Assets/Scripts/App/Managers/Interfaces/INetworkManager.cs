using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TandC.RunIfYouWantToLive
{
    public interface INetworkManager
    {
        void StartSend(string name, int score, string endTime);
        void StartGetData(Action<string> OnCompleteRequest = null, Action<string> OnErrorRequest = null);
        bool IsHasInternetConnection();
    }
}
