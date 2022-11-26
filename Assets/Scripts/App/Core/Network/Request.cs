using System;

namespace TandC.RunIfYouWantToLive
{
    public class Request : RequestBase
    {
        private Action<string> _responseCallback;

        public Request(Action<string> response)
        {
            _responseCallback = response;
        }

        protected override void ResponseHandler(string data)
        {
            if (_responseCallback != null)
                _responseCallback(data);
        }
    }
}
