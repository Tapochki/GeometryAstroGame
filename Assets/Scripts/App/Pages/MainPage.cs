using TandC.RunIfYouWantToLive.Common;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using TMPro;
using I2.Loc;
using System;

namespace TandC.RunIfYouWantToLive
{
    public class MainPage : IUIElement
    {
        private GameObject _selfPage;

        private IUIManager _uiManager;
        private ILoadObjectsManager _loadObjectsManager;
        private IAdvarismetnManager _advarismetnManager;
        private ILocalizationManager _localizationManager;
        private IGameplayManager _gameplayManager;
        private IDataManager _dataManager;
        private Button _buttonStart,
                       _buttonShop,
                       _buttonSettings,
                       _buttonLeaderboard;

        private TextMeshProUGUI _textStartButton,
                                _textShopButton,
                                _textLeaderboardButton,
                                _textInfoTitle,
                                _textInfoValue,
                                _textInfoNoSignal,
                                _moneyValueText;

        private Animator _animator;

        private bool _showInfoTimer,
                        _hideInfoTimer;
        private float _maxHideInfoTimer = 10f,
                      _maxShowInfoTimer = 5f,
                      _currentInfoTimer;

        private int _infoIndex;



        public void Init()
        {
            _uiManager = GameClient.Get<IUIManager>();
            _loadObjectsManager = GameClient.Get<ILoadObjectsManager>();
            _advarismetnManager = GameClient.Get<IAdvarismetnManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();
            _gameplayManager = GameClient.Get<IGameplayManager>();
            _dataManager = GameClient.Get<IDataManager>();
            _dataManager.EndLoadCache += OnDataLoad;
            _selfPage = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/MainPage"), _uiManager.Canvas.transform, false);

            _buttonStart = _selfPage.transform.Find("Container/Container_Buttons/Button_Start").GetComponent<Button>();
            _buttonShop = _selfPage.transform.Find("Container/Container_Buttons/Button_Shop").GetComponent<Button>();
            _buttonSettings = _selfPage.transform.Find("Container/Button_Settings").GetComponent<Button>();
            _buttonLeaderboard = _selfPage.transform.Find("Container/Container_Buttons/Button_Leaderboard").GetComponent<Button>();

            _textStartButton = _buttonStart.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>();
            _textShopButton = _buttonShop.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>();
            _textLeaderboardButton = _buttonLeaderboard.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>();
            _textInfoTitle = _selfPage.transform.Find("Container/Container_Info/Image_InfoPanel/Text _Title").GetComponent<TextMeshProUGUI>();
            _textInfoValue = _selfPage.transform.Find("Container/Container_Info/Image_InfoPanel/Text_Value").GetComponent<TextMeshProUGUI>();
            _textInfoNoSignal = _selfPage.transform.Find("Container/Container_Info/Image_NoSignal/Text").GetComponent<TextMeshProUGUI>();
            _moneyValueText = _selfPage.transform.Find("Container/Image_CoinContainer/Text_CoinValue").GetComponent<TextMeshProUGUI>();
            _animator = _selfPage.transform.Find("Container/Container_Info").GetComponent<Animator>();

            _buttonStart.onClick.AddListener(StartButtonOnClickHandler);
            _buttonShop.onClick.AddListener(UpgradesButtonOnClickHandler);
            _buttonSettings.onClick.AddListener(SettingsButtonOnClickHandler);
            _buttonLeaderboard.onClick.AddListener(LeaderboardButtonOnClickHandler);

            _localizationManager.LanguageWasChangedEvent += LanguageWasChangedEvent;

            _infoIndex = 0;
            UpdateLocalization();
            Hide();
        }

        private void OnDataLoad() 
        {
            if (_dataManager.CachedUserLocalData.AviableCustomization.Count < Enum.GetValues(typeof(Enumerators.CustomisationType)).Length)
            {
                _dataManager.CachedUserLocalData.InitCustomisation();
            }
        }


        private void LanguageWasChangedEvent(Enumerators.Language obj)
        {
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            _textStartButton.text = _localizationManager.GetUITranslation("KEY_MAIN_MENU_START");
            _textShopButton.text = _localizationManager.GetUITranslation("KEY_MAIN_MENU_SHOP");
            _textLeaderboardButton.text = _localizationManager.GetUITranslation("KEY_MAIN_MENU_LEADERBOARD");
            _textInfoTitle.text = _localizationManager.GetUITranslation("KEY_MAIN_MENU_INFO_TITLE");
            ShowRandomInfo();
            _textInfoNoSignal.text = _localizationManager.GetUITranslation("KEY_MAIN_MENU_INFO_NO_SIGNAL");
        }

        public void Hide()
        {
            _selfPage.SetActive(false);
            _showInfoTimer = false;
            _hideInfoTimer = false;
        }

        public void Show()
        {
            _advarismetnManager.LoadAd();
            _gameplayManager.PauseOff();
            _selfPage.SetActive(true);
            if (_infoIndex >= 4) _infoIndex = 0;
            ShowRandomInfo();
            _animator.Play("Show", -1, 0);
            _infoIndex++;
            _hideInfoTimer = true;
            _currentInfoTimer = _maxHideInfoTimer;
            _moneyValueText.text = _dataManager.CachedUserLocalData.MoneyCount.ToString();
        }

        private void ShowRandomInfo()
        {
            _textInfoValue.text = _localizationManager.GetUITranslation($"KEY_MAIN_MENU_INFO_VALUE_{_infoIndex}");

        }

        public void Update()
        {
            if (_hideInfoTimer)
            {
                _currentInfoTimer -= Time.deltaTime;

                if (_currentInfoTimer <= 0)
                {
                    _hideInfoTimer = false;
                    _animator.Play("Hide", -1, 0);
                    _currentInfoTimer = _maxShowInfoTimer;
                    _showInfoTimer = true;
                }
            }

            if (_showInfoTimer)
            {
                _currentInfoTimer -= Time.deltaTime;

                if (_currentInfoTimer <= 0)
                {
                    _showInfoTimer = false;
                    if (_infoIndex >= 4) _infoIndex = 0;
                    ShowRandomInfo();
                    _animator.Play("Show", -1, 0);
                    _infoIndex++;
                    _currentInfoTimer = _maxHideInfoTimer;
                    _hideInfoTimer = true;
                }
            }
        }

        public void Dispose()
        {

        }

        #region Button handlers
        private void StartButtonOnClickHandler()
        {
           // _advarismetnManager.ShowAd();
            if (!_dataManager.CachedUserLocalData.IstutorialComplete) 
            {
                Hide();
                (_uiManager.GetPage<TutorialPage>() as TutorialPage).StartGameAfterTutorial = true;
                _uiManager.SetPage<TutorialPage>();
                return;
            }
            Hide();
            GameClient.Get<IGameplayManager>().StartGameplay();
            _uiManager.SetPage<GamePage>();
        }

        private void UpgradesButtonOnClickHandler()
        {
            _uiManager.SetPage<SelectShopPage>();
        }

        private void SettingsButtonOnClickHandler()
        {
            _uiManager.SaveCurrentPage();
            _uiManager.SetPage<SettingsPage>();
            //_advarismetnManager.ShowAdsVideo(OnComplete, OnFail);
        }

        private void LeaderboardButtonOnClickHandler()
        {
            _uiManager.SetPage<LeaderBoardPage>();
        }
        #endregion
    }
}