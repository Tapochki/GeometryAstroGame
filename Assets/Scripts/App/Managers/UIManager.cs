using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TandC.RunIfYouWantToLive
{
    public class UIManager : IService, IUIManager
    {
        public List<IUIElement> Pages { get { return _uiPages; } }

        private List<IUIElement> _uiPages;
        private List<IUIPopup> _uiPopups;

        public IUIElement CurrentPage { get; set; }
        public IUIPopup CurrentPopup { get; set; }
        public IUIElement PreviuosPage { get; set; }
        public IUIPopup PreviuosPopup { get; set; }

        public CanvasScaler CanvasScaler { get; set; }
        public GameObject Canvas { get; set; }

        public Camera UICamera { get; set; }

        public void Dispose()
        {
            foreach (var page in _uiPages)
                page.Dispose();

            foreach (var popup in _uiPopups)
                popup.Dispose();
        }

        public void Init()
        {
            Canvas = GameObject.Find("CanvasUI");
            UICamera = GameObject.Find("CameraUI").GetComponent<Camera>();
            CanvasScaler = Canvas.GetComponent<CanvasScaler>();

            _uiPages = new List<IUIElement>()
            {
                new MainPage(),
                new GamePage(),
                new LeaderBoardPage(),
                new SettingsPage(),
                new TutorialPage(),
                new SelectShopPage(),
                new CustomisationPage(),
                new UpgradesPage(),
            };
            foreach (var page in _uiPages)
                page.Init();

            _uiPopups = new List<IUIPopup>()
            {
                new LevelUpPopup(),
                new PausePopup(),
                new ChestPopup(),
                new GameOverPopup(),
                new AboutUSPopup(),
                new DoubleMoneyPopup(),
                new BossPopup(),
            };

            foreach (var popup in _uiPopups)
                popup.Init();
        }

        public void Update()
        {
            foreach (var page in _uiPages)
                page.Update();

            foreach (var popup in _uiPopups)
                popup.Update();
        }

        public void HideAllPages()
        {
            foreach (var _page in _uiPages)
            {
                _page.Hide();
            }
        }

        public void SetPage<T>(bool hideAll = false) where T : IUIElement
        {
            if (hideAll)
            {
                HideAllPages();
            }
            else
            {
                if (CurrentPage != null)
                    CurrentPage.Hide();
            }

            foreach (var _page in _uiPages)
            {
                if (_page is T)
                {
                    CurrentPage = _page;
                    break;
                }
            }
            CurrentPage.Show();
        }

        public void SaveCurrentPage()
        {
            if (CurrentPage != null)
                PreviuosPage = CurrentPage;
        }
        public void LoadPreviousPage()
        {
            if (CurrentPage != null)
                CurrentPage.Hide();
            if (PreviuosPage != null)
                PreviuosPage.Show();

            CurrentPage = PreviuosPage;

            PreviuosPage = null;
        }
        public void SaveCurrentPopup()
        {
            if (CurrentPopup != null)
                PreviuosPopup = CurrentPopup;
        }
        public void LoadPreviousPopup()
        {
            if (CurrentPopup != null)
                CurrentPopup.Hide();
            if (PreviuosPopup != null)
                PreviuosPopup.Show();

            CurrentPopup = PreviuosPopup;

            PreviuosPopup = null;
        }

        public void DrawPopup<T>(object message = null, bool setMainPriority = false) where T : IUIPopup
        {
            foreach (var _popup in _uiPopups)
            {
                if (_popup is T)
                {
                    CurrentPopup = _popup;
                    break;
                }
            }

            if (setMainPriority)
                CurrentPopup.SetMainPriority();

            if (message == null)
                CurrentPopup.Show();
            else
                CurrentPopup.Show(message);
        }

        public void HidePopup<T>() where T : IUIPopup
        {
            foreach (var _popup in _uiPopups)
            {
                if (_popup is T)
                {
                    _popup.Hide();
                    break;
                }
            }
        }

        public IUIPopup GetPopup<T>() where T : IUIPopup
        {
            IUIPopup popup = null;
            foreach (var _popup in _uiPopups)
            {
                if (_popup is T)
                {
                    popup = _popup;
                    break;
                }
            }

            return popup;
        }

        public IUIElement GetPage<T>() where T : IUIElement
        {
            IUIElement page = null;
            foreach (var _page in _uiPages)
            {
                if (_page is T)
                {
                    page = _page;
                    break;
                }
            }

            return page;
        }
    }
}