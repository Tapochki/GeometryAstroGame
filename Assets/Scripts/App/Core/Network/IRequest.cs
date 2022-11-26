using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public interface IRequest
    {
        void SendRequest(string[] obj);
        bool GetResponse();
    }
}
