using System;
using System.Collections;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TandC.RunIfYouWantToLive
{
    public class SettingsPage : IUIElement
    {
        private GameObject _selfObject;
        private ILoadObjectsManager _loadObjectsManager;
        private IUIManager _uiManager;
        private ILocalizationManager _localizationManager;
        private IDataManager _dataManager;

        private Button _closeButton,
                        _aboutUSButton,
                        _tutorialButton,
            _showSelectLanguageButton,
            _hideSelectLanhuageButton;

        private TextMeshProUGUI _selectLanguageShowButtonText, _selectLanguageShowButtonShadowText;
        private TextMeshProUGUI _selectLanguageHideButtonText, _selectLanguageHideButtonShadowText;

        private GameObject _selectLanugagePanel;
        private GameObject _languagePrefab;
        private Transform _languageContent;

        private Toggle _isStaticJoyStickToggle;

        private EditSlider _soundSlider;
        private EditSlider _musicSlider;

        private List<LanguageElement> _languagesElements;

        public void Init()
        {
            _loadObjectsManager = GameClient.Get<ILoadObjectsManager>();
            _uiManager = GameClient.Get<IUIManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();
            _dataManager = GameClient.Get<IDataManager>();

            _dataManager.EndLoadCache += OnEndLoadCahce;
            _languagePrefab = _loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/UIObjects/Button_LanguagePresset");
            _selfObject = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/SettingsPage"), _uiManager.Canvas.transform, false);

            _closeButton = _selfObject.transform.Find("Container/Image_Background/Image_PanelClose/Button_Close").GetComponent<Button>();
            _aboutUSButton = _selfObject.transform.Find("Container/Image_Background/Button_AboutUS").GetComponent<Button>();
            _tutorialButton = _selfObject.transform.Find("Container/Image_Background/Button_Tutorial").GetComponent<Button>();
            _selectLanugagePanel = _selfObject.transform.Find("Container/Image_Background/Container_Settings/Container_Language/Panel_SelectLanguage").gameObject;
            _hideSelectLanhuageButton = _selectLanugagePanel.transform.Find("Button_ChangeLanguage").GetComponent<Button>();
            _languageContent = _selectLanugagePanel.transform.Find("ScrollView/Viewport/Content");
            _showSelectLanguageButton = _selfObject.transform.Find("Container/Image_Background/Container_Settings/Container_Language/Button_ChangeLanguage").GetComponent<Button>();
            _selectLanguageShowButtonText = _showSelectLanguageButton.transform.Find("Text_SelectedLanguage").GetComponent<TextMeshProUGUI>();
            _selectLanguageShowButtonShadowText = _showSelectLanguageButton.transform.Find("Text_SelectedLanguageShadow").GetComponent<TextMeshProUGUI>();
            _selectLanguageHideButtonText = _hideSelectLanhuageButton.transform.Find("Text_SelectedLanguage").GetComponent<TextMeshProUGUI>();
            _selectLanguageHideButtonShadowText = _hideSelectLanhuageButton.transform.Find("Text_SelectedLanguageShadow").GetComponent<TextMeshProUGUI>();
            _languagesElements = new List<LanguageElement>() 
            {
                new LanguageElement(MonoBehaviour.Instantiate(_languagePrefab, _languageContent), Enumerators.Language.English, "English"),
                new LanguageElement(MonoBehaviour.Instantiate(_languagePrefab, _languageContent), Enumerators.Language.Ukrainian, "Українська"),
                new LanguageElement(MonoBehaviour.Instantiate(_languagePrefab, _languageContent), Enumerators.Language.Russian, "Русский"),
            };

            _isStaticJoyStickToggle = _selfObject.transform.Find("Container/Image_Background/Container_Settings/Container_Joystick/Toggle_JoyStick").GetComponent<Toggle>();
            _soundSlider = _selfObject.transform.Find("Container/Image_Background/Container_Settings/Container_Sounds/Slider_Value").GetComponent<EditSlider>();
            _musicSlider = _selfObject.transform.Find("Container/Image_Background/Container_Settings/Container_Music/Slider_Value").GetComponent<EditSlider>();

            _soundSlider.EndDrag += OnEndSoundSliderDrag;
            _musicSlider.EndDrag += OnEndMusicSliderDrag;

            _soundSlider.onValueChanged.AddListener(OnSoundSliderValueChange);
            _musicSlider.onValueChanged.AddListener(OnMusicSliderValueChange);


            _isStaticJoyStickToggle.onValueChanged.AddListener(OnJoyStickToogleHandler);
            foreach (var language in _languagesElements) 
            {
                language.OnSelfButtonClickEvent += SetNewLanguage;
            }
            _closeButton.onClick.AddListener(CloseButtonOnClickHandler);
            _aboutUSButton.onClick.AddListener(AboutUSButtonOnClickHandler);
            _tutorialButton.onClick.AddListener(TutorialButtonOnClickHandler);
            _showSelectLanguageButton.onClick.AddListener(() => { ShowHideLanguage(true); });
            _hideSelectLanhuageButton.onClick.AddListener(() => { ShowHideLanguage(false); });
            _localizationManager.LanguageWasChangedEvent += LanguageWasChangedEvent;
            UpdateLocalization();
            Hide();
        }

        private void OnEndMusicSliderDrag(float value) 
        {
            OnMusicSliderValueChange(value);
            _dataManager.SaveCache(Enumerators.CacheDataType.USER_LOCAL_DATA);
        }

        private void OnEndSoundSliderDrag(float value)
        {
            OnSoundSliderValueChange(value);
            _dataManager.SaveCache(Enumerators.CacheDataType.USER_LOCAL_DATA);
        }

        private void OnEndLoadCahce() 
        {
            _soundSlider.value = _dataManager.CachedUserLocalData.SoundValue;
            _musicSlider.value = _dataManager.CachedUserLocalData.MusicValue;
            _isStaticJoyStickToggle.isOn = _dataManager.CachedUserLocalData.IsStaticJoyStick;
        }

        private void OnMusicSliderValueChange(float value) 
        {
            _dataManager.CachedUserLocalData.MusicValue = value;
        }

        private void OnSoundSliderValueChange(float value)
        {
            _dataManager.CachedUserLocalData.SoundValue = value;
        }

        private void OnJoyStickToogleHandler(bool isOn) 
        {
            _dataManager.CachedUserLocalData.IsStaticJoyStick = isOn;
            _dataManager.SaveCache(Enumerators.CacheDataType.USER_LOCAL_DATA);
        }

        private void ShowHideLanguage(bool value) 
        {
            _selectLanugagePanel.SetActive(value);
        }

        private void SetNewLanguage(Enumerators.Language lang) 
        {
            ShowHideLanguage(false);
            _localizationManager.SetLanguage(lang);
            _dataManager.SaveCache(Enumerators.CacheDataType.USER_LOCAL_DATA);
        }

        public void Show()
        {
            _selfObject.SetActive(true);
        }

        public void Hide()
        {
            ShowHideLanguage(false);
            _selfObject.SetActive(false);

        }

        public void Update()
        {
        }

        public void Dispose()
        {
        }

        #region Button Handlers
        private void CloseButtonOnClickHandler()
        {
            _uiManager.LoadPreviousPage();
            _uiManager.LoadPreviousPopup();
        }

        private void AboutUSButtonOnClickHandler()
        {
            _uiManager.DrawPopup<AboutUSPopup>();
        }

        private void TutorialButtonOnClickHandler()
        {
            _uiManager.SetPage<TutorialPage>();
        }
        #endregion

        private void LanguageWasChangedEvent(Enumerators.Language obj)
        {
            UpdateLocalization();
            _selectLanguageHideButtonText.text = _selectLanguageShowButtonText.text = _selectLanguageShowButtonShadowText.text = _selectLanguageHideButtonShadowText.text = _localizationManager.GetUITranslation("KEY_SETTINGS_SELECTED_LANGUAGE");
            foreach (var item in _languagesElements)
            {
                item.ShowElement();
                if (item.Language == obj)
                {
                    item.HideElement();
                }
            }
        }

        private void UpdateLocalization()
        {
            _selfObject.transform.Find("Container/Container_Title/Text_TitleLight").GetComponent<TextMeshProUGUI>().text= _selfObject.transform.Find("Container/Container_Title/Text_TitleDark").GetComponent<TextMeshProUGUI>().text= _selfObject.transform.Find("Container/Container_Title/Text_TitleMain").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_SETTINGS_TITLE");
            _selfObject.transform.Find("Container/Image_Background/Container_Settings/Container_Sounds/Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_SETTINGS_SOUNDS");
            _selfObject.transform.Find("Container/Image_Background/Container_Settings/Container_Music/Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_SETTINGS_MUSIC");
            _selfObject.transform.Find("Container/Image_Background/Container_Settings/Container_Language/Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_SETTINGS_LANGUAGE");
            _selfObject.transform.Find("Container/Image_Background/Container_Settings/Container_Joystick/Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_SETTINGS_JOYSTICK");
            _aboutUSButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_ABOUT_US_TITLE");
            _tutorialButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_TUTORIAL_TITLE");

        }
        
        private class LanguageElement 
        {
            private GameObject _selfObject;
            public Action<Enumerators.Language> OnSelfButtonClickEvent;
            public Enumerators.Language Language { get; private set; }

            public LanguageElement(GameObject prefab, Enumerators.Language language, string text) 
            {
                _selfObject = prefab;
                _selfObject.GetComponent<Button>().onClick.AddListener(OnSelfButtonClickHandler);
                _selfObject.transform.Find("Text_Shadow").GetComponent<TextMeshProUGUI>().text = text;
                _selfObject.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
                Language = language;
            }

            public void HideElement() 
            {
                _selfObject.SetActive(false);
            }

            public void ShowElement()
            {
                _selfObject.SetActive(true);
            }

            public void Dispose() 
            {
                MonoBehaviour.Destroy(_selfObject);
            }

            private void OnSelfButtonClickHandler() 
            {
                OnSelfButtonClickEvent?.Invoke(Language);
            }
        }
    }
}