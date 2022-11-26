using System;
using System.Collections;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TandC.RunIfYouWantToLive.Common.Enumerators;
using static TandC.RunIfYouWantToLive.ShopData;

namespace TandC.RunIfYouWantToLive
{
    public class CustomisationPage : IUIElement
    {
        private GameObject _selfPage;

        private IUIManager _uiManager;
        private ILoadObjectsManager _loadObjectsManager;
        private ILocalizationManager _localizationManager;
        private IDataManager _dataManager;

        private Button _closeButton;

        private ShopData _shopData;

        private List<CustomisationTypeSelectionItem> _selectionItems;

        private GameObject _customisationPrefab;
        private GameObject _selectionPrefab;

        private Transform _selectionTypeParent;
        private Transform _customisationParent;

        private Dictionary<CustomisationType, List<CustomisationItem>> _customisationItems;

        private TextMeshProUGUI _moneyCountText;


        //private ButtonItem _customizationButton;

        public void Init()
        {
            _uiManager = GameClient.Get<IUIManager>();
            _loadObjectsManager = GameClient.Get<ILoadObjectsManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();
            _dataManager = GameClient.Get<IDataManager>();
            _selfPage = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/CustomisationPage"), _uiManager.Canvas.transform, false);
            _customisationPrefab = _loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/UIObjects/Customisation_Item");
            _selectionPrefab = _loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/UIObjects/SelectCustomization_Item");
            _shopData = _loadObjectsManager.GetObjectByPath<ShopData>("Data/ShopData");

            _selectionTypeParent = _selfPage.transform.Find("Container/Image_Background/Scroll_Types/Viewport/Content");
            _customisationParent = _selfPage.transform.Find("Container/Image_Background/Scroll_Items/Viewport/Content");

            _closeButton = _selfPage.transform.Find("Container/Image_Background/Image_PanelClose/Button_Close").GetComponent<Button>();

            _moneyCountText = _selfPage.transform.Find("Container/Image_CoinContainer/Text_CoinValue").GetComponent<TextMeshProUGUI>();

            _customisationItems = new Dictionary<CustomisationType, List<CustomisationItem>>();
            _selectionItems = new List<CustomisationTypeSelectionItem>();

            foreach(var type in Enum.GetValues(typeof(Enumerators.CustomisationType)))
            {
                CustomisationType selectedType = (CustomisationType)type;
                _customisationItems.Add(selectedType, BuildCustomisationItems(selectedType, _shopData.GetProductsByType(selectedType).Products));
                _selectionItems.Add(AddType(selectedType));
            }

            _localizationManager.LanguageWasChangedEvent += LanguageWasChangedEvent;
            _closeButton.onClick.AddListener(OnCloseButtonClickHandler);

            UpdateLocalization();
            Hide();
        }

        private CustomisationTypeSelectionItem AddType(CustomisationType type) 
        {
            CustomisationTypeSelectionItem item = new CustomisationTypeSelectionItem(MonoBehaviour.Instantiate(_selectionPrefab, _selectionTypeParent), type);
            item.OnSelectEvent += OnSelectType;
            return item;
        }

        private void OnSelectType(CustomisationType type)
        {
            foreach(CustomisationTypeSelectionItem selectionItem in _selectionItems) 
            {
                if(selectionItem.Type == type) 
                {
                    selectionItem.SetButtonInteractable(false);
                }
                selectionItem.SetButtonInteractable(true);
            }

            foreach (var customisationCollection in _customisationItems)
            {
                foreach(var customisation in customisationCollection.Value) 
                {
                    customisation.ShowHide(false);
                }
            }

            foreach (var customisation in _customisationItems[type])
            {
                customisation.ShowHide(true);
            }
        }

        private List<CustomisationItem> BuildCustomisationItems(CustomisationType type, ProductData[] data) 
        {
            List<CustomisationItem> list = new List<CustomisationItem>();
            foreach (var item in data) 
            {
                CustomisationItem customisationItem = new CustomisationItem(MonoBehaviour.Instantiate(_customisationPrefab, _customisationParent), type, item);
                customisationItem.OnBuyAction += OnBuyClickHandler;
                customisationItem.OnSelectedAction += OnSelectItemClickHandler;
                list.Add(customisationItem);
            }
            return list;
        }

        private void OnBuyClickHandler(CustomisationItem item) 
        {
            var playerMoney = _dataManager.CachedUserLocalData.MoneyCount;
            if(playerMoney >= item.ShopData.CostValue) 
            {
                _dataManager.CachedUserLocalData.MoneyCount -= item.ShopData.CostValue;
                _dataManager.CachedUserLocalData.GetProducts(item.Type).AviableId.Add(item.ShopData.Id);
                _dataManager.SaveCache(Enumerators.CacheDataType.USER_LOCAL_DATA);
                _moneyCountText.text = _dataManager.CachedUserLocalData.MoneyCount.ToString();
                UpdateList(item.Type);
            }
        }

        private void OnSelectItemClickHandler(CustomisationItem item) 
        {
            _dataManager.CachedUserLocalData.GetProducts(item.Type).SelectedId = item.ShopData.Id;
            UpdateList(item.Type);
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
            foreach (var customisationCollection in _customisationItems) 
            {
                UpdateList(customisationCollection.Key);
            }
            OnSelectType(CustomisationType.Player);
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
            _selfPage.transform.Find("Container/Container_Title/Text_TitleLight").GetComponent<TextMeshProUGUI>().text = _selfPage.transform.Find("Container/Container_Title/Text_TitleDark").GetComponent<TextMeshProUGUI>().text = _selfPage.transform.Find("Container/Container_Title/Text_TitleMain").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_CUSTOMISATION");
            foreach(var customisation in _customisationItems) 
            {
                foreach(var item in customisation.Value) 
                {
                    item.UpdateLocalization();
                }
            }
            foreach (var item in _selectionItems)
            {
                item.UpdateLocalization();
            }
        }

        public void Dispose()
        {
            
        }

        private void UpdateList(CustomisationType type) 
        {
            List<int> _aviableId = _dataManager.CachedUserLocalData.GetProducts(type).AviableId;
            int selectedId = _dataManager.CachedUserLocalData.GetProducts(type).SelectedId;
            var playerMoney = _dataManager.CachedUserLocalData.MoneyCount;
            foreach (var item in _customisationItems[type])
            {
                item.SetPanelBlocked();
                foreach (var shipId in _aviableId)
                {
                    if (item.ShopData.Id == shipId)
                    {
                        item.SetPanelBought();
                        break;
                    }
                }
                if (item.ShopData.Id == selectedId)
                {
                    item.SetSelectedPanel();
                }
                item.BuyButtonInteractable(item.ShopData.CostValue < playerMoney);
            }
            foreach(var item in _selectionItems) 
            {
                if(item.Type == type) 
                {
                    item.SetImage(_shopData.GetProductsByType(type).GetProductById(selectedId).Sprite);
                }
            }
        }

        private class CustomisationItem 
        {
            private GameObject _selfObject;
            public CustomisationType Type;
            public ProductData ShopData;

            private ILocalizationManager _localizationManager;

            private GameObject _conteinerBought;
            private GameObject _conteinerBlocket;
            private GameObject _conteinerSelected;

            public Action<CustomisationItem> OnSelectedAction;
            public Action<CustomisationItem> OnBuyAction;

            private Button _buyButton;
            private Button _selectButton;

            private TextMeshProUGUI _selectedText;

            public CustomisationItem(GameObject _prefab, CustomisationType type, ProductData data) 
            {
                _localizationManager = GameClient.Get<ILocalizationManager>();
                _selfObject = _prefab;

                _conteinerBought = _selfObject.transform.Find("Container_Bought").gameObject;
                _conteinerBlocket = _selfObject.transform.Find("Container_Blocked").gameObject;
                _conteinerSelected = _selfObject.transform.Find("Container_Selected").gameObject;

                _buyButton = _conteinerBlocket.transform.Find("Button_Buy").GetComponent<Button>();
                _selectButton = _conteinerBought.transform.Find("Button_Select").GetComponent<Button>();
                _selectedText = _conteinerSelected.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>();
                _selfObject.transform.Find("Image_Item").GetComponent<Image>().sprite = data.Sprite;
                ShopData = data;
                Type = type;
                _buyButton.onClick.AddListener(OnBuyClickHandler);
                _selectButton.onClick.AddListener(OnSelectClickHandler);
            }

            public void ShowHide(bool value) 
            {
                _selfObject.SetActive(value);
            }

            public void SetPanelBlocked() 
            {
                _conteinerBlocket.SetActive(true);
                _conteinerSelected.SetActive(false);
                _conteinerBought.SetActive(false);
            }

            public void SetPanelBought()
            {
                _conteinerBlocket.SetActive(false);
                _conteinerSelected.SetActive(false);
                _conteinerBought.SetActive(true);
            }

            public void SetSelectedPanel() 
            {
                _conteinerBlocket.SetActive(false);
                _conteinerSelected.SetActive(true);
                _conteinerBought.SetActive(false);
            }

            public void UpdateLocalization() 
            {
                _buyButton.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>().text =  $"{_localizationManager.GetUITranslation("KEY_COST")} {ShopData.CostValue}";
                _selectButton.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_SELECT");
                _selectedText.text = _localizationManager.GetUITranslation("KEY_SELECTED");
            }

            public void BuyButtonInteractable(bool value) 
            {
                _buyButton.interactable = value;
            }

            public void BuyFailed() 
            {

            }

            private void OnBuyClickHandler()
            {
                OnBuyAction?.Invoke(this);
            }

            private void OnSelectClickHandler()
            {
                OnSelectedAction?.Invoke(this);
            }
        }

        private class CustomisationTypeSelectionItem 
        {
            private GameObject _selfObject;
            private Button _selfButton;
            public Action<CustomisationType> OnSelectEvent;
            public CustomisationType Type;
            private ILocalizationManager _localizationManager;
            private Image _selectedImage;
            private TextMeshProUGUI _text;

            public CustomisationTypeSelectionItem(GameObject prefab, CustomisationType type) 
            {
                _localizationManager = GameClient.Get<ILocalizationManager>();
                _selfObject = prefab;
                Type = type;
                _selfButton = _selfObject.transform.GetComponent<Button>();
                _selfButton.onClick.AddListener(OnSelfButtonClickHandler);
                _selectedImage = _selfObject.transform.Find("Image").GetComponent<Image>();
                _text = _selfObject.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            }

            public void SetButtonInteractable(bool value) 
            {
                _selfButton.interactable = value;
            }

            private void OnSelfButtonClickHandler() 
            {
                OnSelectEvent?.Invoke(Type);
            }

            public void SetImage(Sprite sprite) 
            {
                _selectedImage.sprite = sprite;
            }

            public void UpdateLocalization() 
            {
                _text.text = _localizationManager.GetUITranslation($"KEY_SELECTION_{Type.ToString().ToUpper()}");
            }

        }
    }
}

