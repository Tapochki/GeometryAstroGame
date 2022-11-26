using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public interface IUIElement
    {
        void Init();
        void Show();
        void Hide();
        void Update();
        void Dispose();
    }
}