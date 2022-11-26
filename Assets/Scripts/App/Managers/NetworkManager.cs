using System;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using UnityEngine;

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