using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TandC.RunIfYouWantToLive
{
    public class AboutUSPopup : IUIPopup
    {
        private GameObject _selfObject;

        private ILoadObjectsManager _loadObjectsManager;
        private IUIManager _uiManager;
        private ILocalizationManager _localizationManager;

        private Button _buttonClose;

        private Button _danilInstagramButton;
        private Button _romaInstagramButton;
        private Button _patreonButton;

        public void Init()
        {
            _loadObjectsManager = GameClient.Get<ILoadObjectsManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();
            _uiManager = GameClient.Get<IUIManager>();

            _selfObject = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/Popups/AboutUSPopup"), _uiManager.Canvas.transform, false);
            _danilInstagramButton = _selfObject.transform.Find("Container/Image_Background/Container/LeadDeveloper/Button_Instagramm").GetComponent<Button>();
            _romaInstagramButton = _selfObject.transform.Find("Container/Image_Background/Container/LeadArtist/Button_Instagramm").GetComponent<Button>();
            _patreonButton = _selfObject.transform.Find("Container/Image_Background/Container/SupportUs/Button_Instagramm").GetComponent<Button>();
            _buttonClose = _selfObject.transform.Find("Container/Image_Background/Image_ClosePanel/Button_Close").GetComponent<Button>();

            _buttonClose.onClick.AddListener(CloseButtonOnCliskHandler);
            _danilInstagramButton.onClick.AddListener(OnDanilInstagramButtonClickHandler);
            _romaInstagramButton.onClick.AddListener(OnRomanInstagramButtonClickHandler);

            _localizationManager.LanguageWasChangedEvent += LanguageWasChangedEvent;
            UpdateLocalization();
            Hide();
        }

        private void OnPatreonButtonClickHandler()
        {

        }

        private void OnDanilInstagramButtonClickHandler() 
        {
            Application.OpenURL("https://www.instagram.com/tapochk1/");
        }

        private void OnRomanInstagramButtonClickHandler()
        {
            Application.OpenURL("https://www.instagram.com/balthazariy/");
        }

        public void Show()
        {
            _selfObject.SetActive(true);
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

        private void CloseButtonOnCliskHandler()
        {
            Hide();
        }

        private void LanguageWasChangedEvent(Enumerators.Language obj)
        {
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            _selfObject.transform.Find("Container/Image_Background/Container_Title/Text_TitleLight").GetComponent<TextMeshProUGUI>().text = _selfObject.transform.Find("Container/Image_Background/Container_Title/Text_TitleDark").GetComponent<TextMeshProUGUI>().text = _selfObject.transform.Find("Container/Image_Background/Container_Title/Text_TitleMain").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_ABOUT_US_TITLE");
            _selfObject.transform.Find("Container/Image_Background/Container/LeadDeveloper/Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_ABOUT_US_LEAD_PROGRAMME");
            _selfObject.transform.Find("Container/Image_Background/Container/LeadArtist/Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_ABOUT_US_LEAD_ARTIST");
            _selfObject.transform.Find("Container/Image_Background/Container/SupportUs/Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_SUPPORT_US_TITLE");
            _danilInstagramButton.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_ABOUT_US_INSTAGRAMM");
            _romaInstagramButton.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_ABOUT_US_INSTAGRAMM");
            _patreonButton.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_ABOUT_US_PATREON");
        }
    }
}