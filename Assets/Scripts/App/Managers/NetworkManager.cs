using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TandC.RunIfYouWantToLive.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace TandC.RunIfYouWantToLive
{
    public class NetworkManager : IService, INetworkManager
    {
        private string _pathDomain = "http://tandcgamedevelopment.party/backend/";
        public void Dispose()
        {
        }

        public void Init()
        {

        }

        public void Update()
        {

        }

        public void StartSend(string name, int score, string endTime)
        {
            WWWForm form = new WWWForm();
            form.AddField("Name", name);
            form.AddField("Score", score);
            form.AddField("EndTime", endTime);
            MainApp.Instance.StartCoroutine(Send(form, Constants.DOMAINTOSEND, OnSendInfoSucces, OnSendInfoError));
        }

        public void StartGetData(Action<string> OnCompleteRequest = null, Action<string> OnErrorRequest = null) 
        {
            WWWForm form = new WWWForm();
            MainApp.Instance.StartCoroutine(Send(form, Constants.DOMAINTOGET, OnCompleteRequest, OnErrorRequest));
        }

        private void OnSendInfoSucces(string json) 
        {
            Debug.Log(json);
        }
        private void OnSendInfoError(string json) 
        {
            Debug.LogError(json);
        }

        public async Task<string> GetRequest(string url, Dictionary<string, string> headers = null)
        {
            using (UnityWebRequest uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET))
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                        uwr.SetRequestHeader(header.Key, header.Value);
                }

                DownloadHandler downloadHandler = new DownloadHandlerBuffer();
                uwr.downloadHandler = downloadHandler;

                var operation = uwr.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Delay(100);
                }

                if (uwr.result != UnityWebRequest.Result.Success)
                {
#if UNITY_EDITOR
                    Debug.LogError($"Failed to load: {url} due to error: {uwr.error}");
#endif
                    return null;
                }
                else
                {
                    return System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data);
                }
            }
        }

        private System.Collections.IEnumerator Send(WWWForm param, string url, Action<string> OnCompleteRequest = null, Action<string> OnErrorRequest = null) 
        {
            
            string path = _pathDomain + url;
            WWW www = new WWW(path, param);
            yield return www;
            if(www.error != null) 
            {
                OnErrorRequest?.Invoke(www.error);
            }
            else 
            {
                OnCompleteRequest?.Invoke(www.text);
            }
            
        }

        public bool IsHasInternetConnection()
        {
            return !(UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable);

        }
    }
}