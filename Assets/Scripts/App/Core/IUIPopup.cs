using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public interface IUIPopup
    {
        void Init();
        void Show();
        void Show(object data);
        void Hide();
        void Update();
        void Dispose();
        void SetMainPriority();
    }
}