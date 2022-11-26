using TandC.RunIfYouWantToLive.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TandC.RunIfYouWantToLive
{
    public class SelectShopPage : IUIElement
    {
        private GameObject _selfPage;

        private IUIManager _uiManager;
        private ILoadObjectsManager _loadObjectsManager;
        private ILocalizationManager _localizationManager;
        private IDataManager _dataManager;
        private IAdvarismetnManager _advarismetnManager;

        private Button _customisationButton, 
                       _upgradeButton, 
                       _closeButton,
                       _buttonBonusMoney;

        private TextMeshProUGUI _removeText,
                                _moneyCountText;

        public void Init()
        {
            _uiManager = GameClient.Get<IUIManager>();
            _loadObjectsManager = GameClient.Get<ILoadObjectsManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();
            _advarismetnManager = GameClient.Get<IAdvarismetnManager>();
            _dataManager = GameClient.Get<IDataManager>();

            _selfPage = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/SelectShopPage"), _uiManager.Canvas.transform, false);

            _customisationButton = _selfPage.transform.Find("Container/Image_Background/Container_Buttons/Button_Castomisation").GetComponent<Button>();
            _upgradeButton = _selfPage.transform.Find("Container/Image_Background/Container_Buttons/Button_Upgrades").GetComponent<Button>();
            _closeButton = _selfPage.transform.Find("Container/Image_Background/Image_Close/Button_Close").GetComponent<Button>();
            _buttonBonusMoney = _selfPage.transform.Find("Container/Image_Background/Container_Buttons/Button_BonusMoney").GetComponent<Button>();

            _removeText = _selfPage.transform.Find("Container/Image_Background/Container_Buttons/Button_RemoveAd/Text_Title").GetComponent<TextMeshProUGUI>();
            _moneyCountText = _selfPage.transform.Find("Container/Image_CoinContainer/Text_CoinValue").GetComponent<TextMeshProUGUI>();

            _localizationManager.LanguageWasChangedEvent += LanguageWasChangedEvent;

            _customisationButton.onClick.AddListener(OnCustomizationClickHandler);
            _upgradeButton.onClick.AddListener(OnUpgradeClickHandler);
            _closeButton.onClick.AddListener(OnCloseButtonClickHandler);
            _buttonBonusMoney.onClick.AddListener(BonusMoneyButtonOnClickHandler);

            UpdateLocalization();

            Hide();
        }

        public void Hide()
        {
            _selfPage.SetActive(false);
        }

        public void Show()
        {
            _selfPage.SetActive(true);
            _moneyCountText.text = _dataManager.CachedUserLocalData.MoneyCount.ToString();
            if (_advarismetnManager.IsLoadVideo)
                _buttonBonusMoney.interactable = true;
            else
                _buttonBonusMoney.interactable = false;
        }

        public void Update()
        {

        }

        private void LanguageWasChangedEvent(Enumerators.Language obj)
        {
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            _customisationButton.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_CUSTOMISATION");
            _upgradeButton.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_UPGRADE");
            _removeText.text = _localizationManager.GetUITranslation("KEY_REMOVE_ADS");
        }

        public void Dispose()
        {

        }

        #region Button Handlers
        private void BonusMoneyButtonOnClickHandler()
        {
            _advarismetnManager.ShowAd();
            _dataManager.CachedUserLocalData.MoneyCount += 150;
            _dataManager.SaveCache(Enumerators.CacheDataType.USER_LOCAL_DATA);
            _moneyCountText.text = _dataManager.CachedUserLocalData.MoneyCount.ToString();
        }

        private void OnCloseButtonClickHandler()
        {
            _uiManager.SetPage<MainPage>();
        }

        private void OnCustomizationClickHandler()
        {
            _uiManager.SetPage<CustomisationPage>();
        }

        private void OnUpgradeClickHandler()
        {
            _uiManager.SetPage<UpgradesPage>();
        }
        #endregion
    }
}

