using System.Collections;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TandC.RunIfYouWantToLive
{
    public class DoubleMoneyPopup : IUIPopup
    {
        private GameObject _selfObject;

        private IUIManager _uiManager;
        private ILoadObjectsManager _loadObjectsManager;
        private ILocalizationManager _localizationManager;
        private IGameplayManager _gameplayManager;
        private IAdvarismetnManager _advarismetnManager;

        private TextMeshProUGUI _textOldMoney,
                                _textNewMoney,
                                _textContinueButton,
                                _textDoubleMoneyButton;

        private int _earnedMoney,
                    _doubledMoney;

        private Button _buttonDoubleMoney,
                       _buttonContinue;


        public void Init()
        {
            _uiManager = GameClient.Get<IUIManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();
            _loadObjectsManager = GameClient.Get<ILoadObjectsManager>();
            _gameplayManager = GameClient.Get<IGameplayManager>();
            _advarismetnManager = GameClient.Get<IAdvarismetnManager>();

            _selfObject = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/Popups/DoubleMoneyPopup"), _uiManager.Canvas.transform, false);

            _textOldMoney = _selfObject.transform.Find("Container/Text_OldMoney").GetComponent<TextMeshProUGUI>();
            _textNewMoney = _selfObject.transform.Find("Container/Text_NewMoney").GetComponent<TextMeshProUGUI>();
            _textContinueButton = _selfObject.transform.Find("Container/Button_Continue/Text_Title").GetComponent<TextMeshProUGUI>();
            _textDoubleMoneyButton = _selfObject.transform.Find("Container/Button_DoubleMoney/Text_Title").GetComponent<TextMeshProUGUI>();

            _buttonContinue = _selfObject.transform.Find("Container/Button_Continue").GetComponent<Button>();
            _buttonDoubleMoney = _selfObject.transform.Find("Container/Button_DoubleMoney").GetComponent<Button>();

            _buttonContinue.onClick.AddListener(ContinueButtonHandlers);
            _buttonDoubleMoney.onClick.AddListener(DoubleMoneyButtonHandlers);

            UpdateLocalisation();

            _localizationManager.LanguageWasChangedEvent += UpdateLocalisationEventHandler;

            Hide();
        }

        private void UpdateLocalisationEventHandler(Enumerators.Language language)
        {
            UpdateLocalisation();
        }

        private void UpdateLocalisation()
        {
            _textContinueButton.text = _localizationManager.GetUITranslation("KEY_CONTINUE");
            _textDoubleMoneyButton.text = _localizationManager.GetUITranslation("KEY_DOUBLE_MONEY");
        }

        public void Show()
        {
            _selfObject.SetActive(true);
            _earnedMoney = _gameplayManager.GetController<PlayerController>().EarnedMoney;
            _doubledMoney = _earnedMoney * 2;
            _textOldMoney.text = _earnedMoney.ToString();
            _textNewMoney.text = _doubledMoney.ToString();
        }

        public void Show(object data)
        {
            Show();
        }

        public void Hide()
        {
            _selfObject.SetActive(false);
        }

        public void Update()
        {
        }

        public void Dispose()
        {
        }

        public void SetMainPriority()
        {
        }

        #region Button Handlers
        private void ContinueButtonHandlers()
        {
            Hide();
            _gameplayManager.PlayerGetMoney(_earnedMoney);
            _uiManager.SetPage<MainPage>();
        }

        private void DoubleMoneyButtonHandlers()
        {
            _advarismetnManager.ShowAdsVideo(OnAdsSuccessEventHandler, OnAdsFailedEventHandler);
        }
        #endregion

        #region Ad events
        private void OnAdsSuccessEventHandler()
        {
            _gameplayManager.PlayerGetMoney(_doubledMoney);
            Hide();
            _uiManager.SetPage<MainPage>();
        }

        private void OnAdsFailedEventHandler()
        {
            ContinueButtonHandlers();
        }
        #endregion
    }
}