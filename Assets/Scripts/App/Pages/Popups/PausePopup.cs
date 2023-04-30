using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TandC.RunIfYouWantToLive.Common;
using System;

namespace TandC.RunIfYouWantToLive
{
    public class PausePopup : IUIPopup
    {
        public event Action<Enumerators.SkillType> OnSkillChoiceEvent;

        private ILoadObjectsManager _loadObjectManager;
        private IGameplayManager _gameplayManager;
        private IUIManager _uIManager;
        private ILocalizationManager _localizationManager;

        private GameObject _selfObject;

        private TextMeshProUGUI _scoreText;

        private Button _resumeButton,
                        _backToMenuButton,
                        _settingButton;

        private TextMeshProUGUI _textTitle1,
                                _textTitle2,
                                _textTitle3,
                                _textScoreTitle,
                                _textExitButton,
                                _textContinueButton;

        public void Init()
        {
            _loadObjectManager = GameClient.Get<ILoadObjectsManager>();
            _gameplayManager = GameClient.Get<IGameplayManager>();
            _uIManager = GameClient.Get<IUIManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();

            _selfObject = MonoBehaviour.Instantiate(_loadObjectManager.GetObjectByPath<GameObject>("Prefabs/UI/Popups/PausePopup"), _uIManager.Canvas.transform);
            _scoreText = _selfObject.transform.Find("Container/Image_Background/Text_ScoreValue").GetComponent<TextMeshProUGUI>();
            _resumeButton = _selfObject.transform.Find("Container/Image_Background/Button_Continue").GetComponent<Button>();
            _backToMenuButton = _selfObject.transform.Find("Container/Image_Background/Button_Exit").GetComponent<Button>();
            _settingButton = _selfObject.transform.Find("Container/Button_Settings").GetComponent<Button>();

            _textTitle1 = _selfObject.transform.Find("Container/Image_Background/Container_Title/Text_TitleLight").GetComponent<TextMeshProUGUI>();
            _textTitle2 = _selfObject.transform.Find("Container/Image_Background/Container_Title/Text_TitleDark").GetComponent<TextMeshProUGUI>();
            _textTitle3 = _selfObject.transform.Find("Container/Image_Background/Container_Title/Text_TitleMain").GetComponent<TextMeshProUGUI>();
            _textScoreTitle = _selfObject.transform.Find("Container/Image_Background/Text_ScoreTitle").GetComponent<TextMeshProUGUI>();
            _textExitButton = _selfObject.transform.Find("Container/Image_Background/Button_Exit/Text_Title").GetComponent<TextMeshProUGUI>();
            _textContinueButton = _selfObject.transform.Find("Container/Image_Background/Button_Continue/Text_Title").GetComponent<TextMeshProUGUI>();

            _resumeButton.onClick.AddListener(OnResumeButtonClickHandler);
            _backToMenuButton.onClick.AddListener(OnBackToMenuClickHandler);
            _settingButton.onClick.AddListener(SettingButtonOnClickHandler);

            _localizationManager.LanguageWasChangedEvent += LanguageWasChangedEvent;
            UpdateLocalization();
            Hide();
        }

        private void UpdateLocalization() 
        {
            _textTitle1.text = _localizationManager.GetUITranslation("KEY_PAUSE_TITLE");
            _textTitle2.text = _localizationManager.GetUITranslation("KEY_PAUSE_TITLE");
            _textTitle3.text = _localizationManager.GetUITranslation("KEY_PAUSE_TITLE");
            _textScoreTitle.text = _localizationManager.GetUITranslation("KEY_SCORE");
            _textExitButton.text = _localizationManager.GetUITranslation("KEY_EXIT") + ":";
            _textContinueButton.text = _localizationManager.GetUITranslation("KEY_CONTINUE");
        }

        private void LanguageWasChangedEvent(Enumerators.Language obj)
        {
            UpdateLocalization();
        }

        public void Show() 
        {
            _selfObject.gameObject.SetActive(true);
            _gameplayManager.PauseGame(true);
            _scoreText.text = _gameplayManager.GetController<EnemyController>().ScoreCount.ToString();
        }

        public void Show(object data)
        {
            Show();
        }

        public void Hide()
        {
            _selfObject.gameObject.SetActive(false);

        }

        public void Update()
        {
        }

        public void Dispose()
        {
            MonoBehaviour.Destroy(_selfObject);
        }

        public void SetMainPriority()
        {
        }

        private void OnResumeButtonClickHandler() 
        {
            _gameplayManager.PauseGame(false);
            Hide();
        }

        private void OnBackToMenuClickHandler() 
        {
            _gameplayManager.PauseGame(false);
            _gameplayManager.StopGameplay();
            _uIManager.SetPage<MainPage>();
            Hide();
        }

        private void SettingButtonOnClickHandler()
        {
            Hide();
            _uIManager.SaveCurrentPage();
            _uIManager.SaveCurrentPopup();
            _uIManager.SetPage<SettingsPage>();
        }
    }
}