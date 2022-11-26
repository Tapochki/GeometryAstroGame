using System.Collections;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TandC.RunIfYouWantToLive
{
    public class TutorialPage : IUIElement
    {
        private GameObject _selfObject,
                            _pageContainerObject;

        private ILoadObjectsManager _loadObjectsManager;
        private IUIManager _uiManager;
        private ILocalizationManager _localizationManager;
        private IDataManager _dataManager;

        private Button _buttonClose,
                        _buttonNextPage,
                        _buttonPreviousPage;

        private List<PageItem> _pages;
        private TextMeshProUGUI _descriptionText;
        private int _pageIndex;
        public bool StartGameAfterTutorial;
        public void Init()
        {
            _loadObjectsManager = GameClient.Get<ILoadObjectsManager>();
            _uiManager = GameClient.Get<IUIManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();
            _dataManager = GameClient.Get<IDataManager>();
            _selfObject = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/TutorialPage"), _uiManager.Canvas.transform, false);

            _buttonClose = _selfObject.transform.Find("Container/Image_Background/Image_ClosePanel/Button_Close").GetComponent<Button>();
            _buttonNextPage = _selfObject.transform.Find("Container/Image_Background/Button_Next").GetComponent<Button>();
            _buttonPreviousPage = _selfObject.transform.Find("Container/Image_Background/Button_Previous").GetComponent<Button>();

            _pageContainerObject = _selfObject.transform.Find("Container/Image_Background/Container_Info/Container_Pages").gameObject;
            _descriptionText = _selfObject.transform.Find("Container/Image_Background/Container_Info/Image_DesctiptionBG/Text_Description").GetComponent<TextMeshProUGUI>();
            _buttonClose.onClick.AddListener(CloseButtonOnClickHandler);
            _buttonNextPage.onClick.AddListener(NextButtonOnClickHandler);
            _buttonPreviousPage.onClick.AddListener(PreviousButtonOnClickHandler);
            _localizationManager.LanguageWasChangedEvent += LanguageWasChangedEvent;
            StartGameAfterTutorial = false;
            InitializePages();
            UpdateLocalization();
            Hide();
        }

        private void InitializePages()
        {
            _pages = new List<PageItem>();

            for (int i = 0; i < _pageContainerObject.transform.childCount; i++)
            {
                _pages.Add(new PageItem(_pageContainerObject.transform.Find($"Screen_{i}").gameObject));
                _pages[i].SetPageActive(false);
            }
        }

        public void Show()
        {
            _selfObject.SetActive(true);

            foreach (var item in _pages)
            {
                item.SetPageActive(false);
            }
            _pageIndex = 0;
            _pages[_pageIndex].SetPageActive(true);
            _buttonPreviousPage.interactable = false;
            SetText();
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

        #region Button Handlers
        private void CloseButtonOnClickHandler()
        {
            _dataManager.CachedUserLocalData.IstutorialComplete = true;
            _dataManager.SaveCache(Enumerators.CacheDataType.USER_LOCAL_DATA);

            if (StartGameAfterTutorial)
            {
                Hide();
                GameClient.Get<IGameplayManager>().StartGameplay();
                _uiManager.SetPage<GamePage>();
                StartGameAfterTutorial = false;
                return;
            }
            _uiManager.SetPage<SettingsPage>();
        }

        private void NextButtonOnClickHandler()
        {
            _pages[_pageIndex].SetPageActive(false);
            _pageIndex++;

            if (_pageIndex >= _pages.Count)
            {
                _dataManager.CachedUserLocalData.IstutorialComplete = true;
                _dataManager.SaveCache(Enumerators.CacheDataType.USER_LOCAL_DATA);
                if (StartGameAfterTutorial) 
                {
                    Hide();
                    GameClient.Get<IGameplayManager>().StartGameplay();
                    _uiManager.SetPage<GamePage>();
                }
                else 
                {
                    _uiManager.SetPage<SettingsPage>();
                }
                StartGameAfterTutorial = false;
                return;
            }
            else _buttonPreviousPage.interactable = true;
            SetText();
            _pages[_pageIndex].SetPageActive(true);
        }

        private void SetText() 
        {
            _descriptionText.text = _localizationManager.GetUITranslation($"KEY_TUTORIAL_PAGE_{_pageIndex}");
        }

        private void PreviousButtonOnClickHandler()
        {
            _pages[_pageIndex].SetPageActive(false);
            _pageIndex--;

            if (_pageIndex <= 0) _buttonPreviousPage.interactable = false;

            else _buttonPreviousPage.interactable = true;
            SetText();
            _pages[_pageIndex].SetPageActive(true);
        }
        #endregion

        private void LanguageWasChangedEvent(Enumerators.Language obj)
        {
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            _selfObject.transform.Find("Container/Container_Title/Text_TitleLight").GetComponent<TextMeshProUGUI>().text = _selfObject.transform.Find("Container/Container_Title/Text_TitleDark").GetComponent<TextMeshProUGUI>().text = _selfObject.transform.Find("Container/Container_Title/Text_TitleMain").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_TUTORIAL_TITLE");
        }
    }

    public class PageItem
    {
        private GameObject _selfObject;

        public PageItem(GameObject selfObject)
        {
            _selfObject = selfObject;
        }

        public void SetPageActive(bool active) => _selfObject.SetActive(active);
    }
}