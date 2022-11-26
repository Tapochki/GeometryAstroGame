using System;
using System.Collections;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using TandC.RunIfYouWantToLive.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TandC.RunIfYouWantToLive.ShopData;

namespace TandC.RunIfYouWantToLive
{
    public class UpgradesPage : IUIElement
    {
        private GameObject _selfPage;

        private IUIManager _uiManager;
        private ILoadObjectsManager _loadObjectsManager;
        private ILocalizationManager _localizationManager;
        private IDataManager _dataManager;

        private Button _closeButton;

        private TextMeshProUGUI _moneyCountText;

        private List<UpgradeItem> _upgradeItems;
        private Transform _itemsParent;
        private GameObject _upgradeItemPrefab;

        private ShopData _shopData;
        private GameObject _selectItemPanel;
        private Image _selectItemImage;
        private TextMeshProUGUI _titleText, 
                                _descriptionText, 
                                _costText;
        private Button _upgradeButton;

        private UpgradeData _selectedUpgradeData;
        private UpgradeItem _selectedItem;

        private float _costMultiplier;
        private int _upgradeCost;

        //private ButtonItem _customizationButton;

        public void Init()
        {
            _uiManager = GameClient.Get<IUIManager>();
            _loadObjectsManager = GameClient.Get<ILoadObjectsManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();
            _dataManager = GameClient.Get<IDataManager>();
            _selfPage = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/UpgradesPage"), _uiManager.Canvas.transform, false);
            _shopData = _loadObjectsManager.GetObjectByPath<ShopData>("Data/ShopData");
            _upgradeItemPrefab = _loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/UIObjects/UpgradeSkill_Item");
            _itemsParent = _selfPage.transform.Find("Container/Image_Background/ScrollView/Viewport/Content");
            _upgradeItems = new List<UpgradeItem>();
            foreach (var data in _shopData.UpgradeDataList)
            {
                UpgradeItem item = new UpgradeItem(MonoBehaviour.Instantiate(_upgradeItemPrefab, _itemsParent), data.UpgradeType, data);
                item.OnSelfSelectEvent += OnSelectItemHandler;
                _upgradeItems.Add(item);
            }

            _selectItemPanel = _selfPage.transform.Find("Container/Image_Background/Panel_UpgradeInfo").gameObject;
            _selectItemImage = _selectItemPanel.transform.Find("Image_SkillIcon").GetComponent<Image>();
            _titleText = _selectItemPanel.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>();
            _descriptionText = _selectItemPanel.transform.Find("TextDescription").GetComponent<TextMeshProUGUI>();
            _costText = _selectItemPanel.transform.Find("Panel_Cost/Text_Cost").GetComponent<TextMeshProUGUI>();
            _upgradeButton = _selectItemPanel.transform.Find("Button_Upgrade").GetComponent<Button>();

            _closeButton = _selfPage.transform.Find("Image_Close/Button_Close").GetComponent<Button>();
            _moneyCountText = _selfPage.transform.Find("Container/Image_CoinContainer/Text_CoinValue").GetComponent<TextMeshProUGUI>();
            _localizationManager.LanguageWasChangedEvent += LanguageWasChangedEvent;
            _closeButton.onClick.AddListener(OnCloseButtonClickHandler);
            _upgradeButton.onClick.AddListener(OnUpgradeButtonClickHandler);
            

            UpdateLocalization();
            Hide();
        }

        private void UpdateItems() 
        {
            foreach(var item in _upgradeItems) 
            {
                item.UpdateItem(_shopData.GetUpgradeByType(item.UpgradeType));
            }
        }

        private void OnUpgradeButtonClickHandler() 
        {
            _dataManager.CachedUserLocalData.MoneyCount -= _upgradeCost;
            _moneyCountText.text = _dataManager.CachedUserLocalData.MoneyCount.ToString();
            _dataManager.CachedUserLocalData.PlayerCharacteristicsData[_selectedUpgradeData.UpgradeType]++;
            _costMultiplier = InternalTools.GetIncrementalFloatValue(1, 1.05f, _shopData.GetSumOfUpgradeLevel());
            _shopData.GetUpgradeByType(_selectedUpgradeData.UpgradeType).CurrentLevel = _dataManager.CachedUserLocalData.PlayerCharacteristicsData[_selectedUpgradeData.UpgradeType];
            _selectedItem.UpdateItem(_shopData.GetUpgradeByType(_selectedUpgradeData.UpgradeType));
            _selectedItem.SelectItem();

            _dataManager.SaveCache(Enumerators.CacheDataType.USER_LOCAL_DATA);
        }

        private void OnSelectItemHandler(UpgradeItem item) 
        {
            foreach(var upgradeItem in _upgradeItems) 
            {
                upgradeItem.SetHighlight(false);
            }
            item.SetHighlight(true);
            _selectedItem = item;
            _selectedUpgradeData = _shopData.GetUpgradeByType(_selectedItem.UpgradeType);

            _selectItemImage.sprite = _selectedUpgradeData.UpgradeSprite;
            _titleText.text = _localizationManager.GetUITranslation(_selectedUpgradeData.Title);

            string descriptionText = _localizationManager.GetUITranslation(_selectedUpgradeData.DescriptionKey);
            descriptionText = descriptionText.Replace("%n%", $"{_selectedUpgradeData.GetProcentFromUpgrade().ToString("###")}%");
            _descriptionText.text = descriptionText;

            if (_selectedUpgradeData.CurrentLevel+1 >= _selectedUpgradeData.UpgradeList.Count) 
            {
                _costText.text = _localizationManager.GetUITranslation("KEY_MAX_VALUE");
                _upgradeButton.interactable = false;
            }
            else 
            {
                _upgradeCost = (int)(_selectedUpgradeData.GetCostValue() * _costMultiplier);
                _upgradeButton.interactable = _dataManager.CachedUserLocalData.MoneyCount > _upgradeCost; 
                _costText.text = _upgradeCost.ToString();
            }
        }

        private void OnCloseButtonClickHandler() 
        {
            _uiManager.SetPage<SelectShopPage>();
        }

        public void Hide()
        {
            _selfPage.SetActive(false);
        }

        public void Show()
        {
            _selfPage.SetActive(true);
            _moneyCountText.text = _dataManager.CachedUserLocalData.MoneyCount.ToString();
            _costMultiplier = InternalTools.GetIncrementalFloatValue(1, 1.05f, _shopData.GetSumOfUpgradeLevel());
            UpdateItems();
            _upgradeItems[0].SelectItem();
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
            _selfPage.transform.Find("Container/Container_Title/Text_TitleLight").GetComponent<TextMeshProUGUI>().text = _selfPage.transform.Find("Container/Container_Title/Text_TitleDark").GetComponent<TextMeshProUGUI>().text = _selfPage.transform.Find("Container/Container_Title/Text_TitleMain").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_UPGRADE");
        }

        public void Dispose()
        {
            
        }

        private class UpgradeItem 
        {
            private GameObject _selfObject;
            public Action<UpgradeItem> OnSelfSelectEvent;
            public Enumerators.UpgradeType UpgradeType;
            private TextMeshProUGUI _levelText;
            private GameObject _hightlight;
            private ILocalizationManager _localizationManager;

            public UpgradeItem(GameObject prefab, Enumerators.UpgradeType type, UpgradeData data) 
            {
                _localizationManager = GameClient.Get<ILocalizationManager>(); 
                _selfObject = prefab;
                UpgradeType = type;
                _selfObject.GetComponent<Button>().onClick.AddListener(SelectItem);

                _hightlight = _selfObject.transform.Find("Image_Highlight").gameObject;
                _selfObject.transform.Find("Image_Icon").GetComponent<Image>().sprite = data.UpgradeSprite;
                _levelText = _selfObject.transform.Find("Image_Icon/Text_Level").GetComponent<TextMeshProUGUI>();

                SetHighlight(false);
                _hightlight.SetActive(false);
            }

            public void SelectItem() 
            {
                OnSelfSelectEvent?.Invoke(this);
            }

            public void SetHighlight(bool value) 
            {
                _hightlight.SetActive(value);
            }

            public void UpdateItem(UpgradeData data) 
            {
                if (data.CurrentLevel+1 >= data.UpgradeList.Count) 
                {
                    _levelText.text = _localizationManager.GetUITranslation("KEY_MAX_VALUE");
                    return;
                }
                _levelText.text = $"{_localizationManager.GetUITranslation("KEY_GAME_LEVEL")} {data.CurrentLevel+1}";
            }
            
            public void Dispose() 
            {
                MonoBehaviour.Destroy(_selfObject);
            }
        }
    }
}

