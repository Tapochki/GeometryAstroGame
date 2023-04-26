using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TandC.RunIfYouWantToLive
{
    public interface INetworkManager
    {
        void StartSend(string name, int score, string endTime);
        void StartGetData(Action<string> OnCompleteRequest = null, Action<string> OnErrorRequest = null);

        Task<string> GetRequest(string url, Dictionary<string, string> headers = null);
        bool IsHasInternetConnection();
    }
}
