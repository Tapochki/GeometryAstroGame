using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public class RequestBase : IRequest
    {
        private int _timeoutCounter = 0;
        private WWW _www;
        private string _url = "/index.php";

        public RequestBase()
        {
            
        }

        public void SendRequest(string[] obj)
        {
            _www = new WWW(ServerConfiguration.GetServerDomain() + _url, GenerateWWWForm(obj));
        }

        public bool GetResponse()
        {
            if (CheckResponseTimeout())
            {
                ResponseHandler("\"result\":\"false\",\"data\":\"Request timeout.\"");
                return true;
            }
            if (_www != null && _www.isDone)
            {
                if (string.IsNullOrEmpty(_www.error))
                {
                    ResponseHandler(_www.text);
                    return true;
                }
                else
                {
                    ResponseHandler("\"result\":\"false\",\"data\":\"" + _www.error + "\"");
                    return false;
                }
            }
            return false;
        }

        protected virtual WWWForm GenerateWWWForm(string[] obj)
        {
            string formData = string.Empty;
            WWWForm form = new WWWForm();
            for (int i = 0; i < obj.Length - 1; i++)
            {
                form.AddField(obj[i], obj[i + 1]);
                formData += obj[i] + " " + obj[i + 1] + "\n";
                i++;
            }

            return form;
        }

        private bool CheckResponseTimeout()
        {
            _timeoutCounter++;
            if (_timeoutCounter > ServerConfiguration.GetRequestTimeout())
                return true;
            return false;
        }

        protected virtual void ResponseHandler(string data)
        {
        }        
    }
}

