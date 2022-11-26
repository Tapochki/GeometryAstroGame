using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TandC.RunIfYouWantToLive
{
    public interface IUIManager
    {
        GameObject Canvas { get; set; }
        CanvasScaler CanvasScaler { get; set; }

        Camera UICamera { get; set; }

        IUIElement CurrentPage { get; set; }
        IUIPopup CurrentPopup { get; set; }
        IUIElement PreviuosPage { get; set; }
        IUIPopup PreviuosPopup { get; set; }
        void SaveCurrentPage();
        void LoadPreviousPage();
        void SaveCurrentPopup();
        void LoadPreviousPopup();
        void SetPage<T>(bool hideAll = false) where T : IUIElement;
        void DrawPopup<T>(object message = null, bool setMainPriority = false) where T : IUIPopup;
        void HidePopup<T>() where T : IUIPopup;
        IUIPopup GetPopup<T>() where T : IUIPopup;
        IUIElement GetPage<T>() where T : IUIElement;

        void HideAllPages();
    }
}