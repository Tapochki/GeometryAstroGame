using TandC.RunIfYouWantToLive.Common;
using System;
using UnityEngine;


namespace TandC.RunIfYouWantToLive
{
    public sealed class AppStateManager : IService, IAppStateManager
    {
        private IUIManager _uiManager;
        private IDataManager _dataManager;

        private float _backButtonTimer,
                      _backButtonResetDelay = 0.5f;

        private int _backButtonClicksCount;
        private bool _isBackButtonCounting;

        public Enumerators.AppState AppState { get; set; }

        public void Dispose()
        {

        }

        public void Init()
        {
            _uiManager = GameClient.Get<IUIManager>();
            _dataManager = GameClient.Get<IDataManager>();
        }

        public void Update()
        {
            CheckBackButton();
        }

        public void ChangeAppState(Enumerators.AppState stateTo)
        {
            if (AppState == stateTo)
                return;

            switch (stateTo)
            {
                case Enumerators.AppState.APP_INIT_LOADING:
                    {
                        _dataManager.StartLoadCache();
                    }
                    break;
                case Enumerators.AppState.MAIN_MENU:
                    {
                        _uiManager.SetPage<MainPage>();
                    }
                    break;
                default:
                    throw new NotImplementedException("Not Implemented " + stateTo.ToString() + " state!");
            }

            AppState = stateTo;
        }

        private void CheckBackButton()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isBackButtonCounting = true;
                _backButtonClicksCount++;
                _backButtonTimer = 0f;

                if (_backButtonClicksCount >= 2)
                {
                    Application.Quit();
                }
            }

            if (_isBackButtonCounting)
            {
                _backButtonTimer += Time.deltaTime;

                if (_backButtonTimer >= _backButtonResetDelay)
                {
                    _backButtonTimer = 0f;
                    _backButtonClicksCount = 0;
                    _isBackButtonCounting = false;
                }
            }
        }
    }
}