using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;

namespace TandC.RunIfYouWantToLive
{
    public class LeaderBoardPage : IUIElement
    {
        private GameObject _selfPage;

        private IUIManager _uiManager;
        private ILoadObjectsManager _loadObjectsManager;
        private IDataManager _dataManager;
        private INetworkManager _networkManager;
        private ILocalizationManager _localizationManager;

        private Button _backToMenuButton;
        private Button _reloadButton;

        private Toggle _localRecordToggle,
                       _globalRecordToggle;

        private GameObject _localRecordsContent, 
                           _globalRecordsContent,
                           _localRecordsPanel,
                           _globalRecordsPanel,
                           _wrongPanel,
                           _loadingPanel;

        private TextMeshProUGUI _textLoading,
                                _textReloadButton;

        private GameObject _userEntryPrefab;

        private List<UserEntry> _localUserEntry,
                                _globalUserEntry;

        private List<GlobalRecordItem> _globalRecordItems;

        public void Init()
        {
            _uiManager = GameClient.Get<IUIManager>();
            _loadObjectsManager = GameClient.Get<ILoadObjectsManager>();
            _dataManager = GameClient.Get<IDataManager>();
            _networkManager = GameClient.Get<INetworkManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();

            _selfPage = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/LeaderBoardPage"), _uiManager.Canvas.transform, false);
            _userEntryPrefab = _loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/UIObjects/Leaderboard_Presset");
            _backToMenuButton = _selfPage.transform.Find("Container/Image_Background/Image_PanelClose/Button_Close").GetComponent<Button>();

            _localRecordToggle = _selfPage.transform.Find("Container/Image_Background/Container_Toggles/Toggle_SwitchToLocal").GetComponent<Toggle>();
            _globalRecordToggle = _selfPage.transform.Find("Container/Image_Background/Container_Toggles/Toggle_SwitchToGlobal").GetComponent<Toggle>();

            _localRecordsPanel = _selfPage.transform.Find("Container/Image_Background/Scroll_Local").gameObject;
            _globalRecordsPanel = _selfPage.transform.Find("Container/Image_Background/Scroll_Global").gameObject;

            _textLoading = _selfPage.transform.Find("Container/Image_Background/Panel_Loading/Text_Title").GetComponent<TextMeshProUGUI>();
            _textReloadButton = _selfPage.transform.Find("Container/Image_Background/Panel_LoadingError/Button_Reload/Text_Title").GetComponent<TextMeshProUGUI>();

            _localRecordsContent = _localRecordsPanel.transform.Find("Viewport/Content").gameObject;
            _globalRecordsContent = _globalRecordsPanel.transform.Find("Viewport/Content").gameObject;

            _wrongPanel = _selfPage.transform.Find("Container/Image_Background/Panel_LoadingError").gameObject;
            _loadingPanel = _selfPage.transform.Find("Container/Image_Background/Panel_Loading").gameObject;

            _reloadButton = _wrongPanel.transform.Find("Button_Reload").GetComponent<Button>();
            _localUserEntry = new List<UserEntry>();
            _globalUserEntry = new List<UserEntry>();

            _localRecordToggle.onValueChanged.AddListener(OnToggleValueChanged);
            _globalRecordToggle.onValueChanged.AddListener(OnToggleValueChanged);
            _backToMenuButton.onClick.AddListener(BackToMenuClickHandler);
            _reloadButton.onClick.AddListener(OnReloadButtonClickHandler);
            _wrongPanel.gameObject.SetActive(false);
            _loadingPanel.gameObject.SetActive(false);
            _localizationManager.LanguageWasChangedEvent += LanguageWasChangedEvent;
            UpdateLocalization();
            Hide();
        }

        private void OnToggleValueChanged(bool isOn) 
        {
            UpdatePanelShow();
        }

        private void UpdatePanelShow() 
        {
            _globalRecordsPanel.SetActive(_globalRecordToggle.isOn);
            _localRecordsPanel.SetActive(_localRecordToggle.isOn);
            if (_globalRecordToggle.isOn) 
            {
                GetGlobalRecords();
            }
            if (_localRecordToggle.isOn) 
            {
                _wrongPanel.gameObject.SetActive(false);
            }
        }

        public void Hide()
        {
            foreach(var item in _localUserEntry) 
            {
                item.Dispose();
            }
            foreach(var item in _globalUserEntry) 
            {
                item.Dispose();
            }
            _localUserEntry.Clear();
            _globalUserEntry.Clear();
            _selfPage.SetActive(false);
        }

        private void BuildLocalRecords() 
        {
            for(int i = 0; i < _dataManager.UserLocalRecords.Count; i++) 
            {
                var item = _dataManager.UserLocalRecords[i];
                _localUserEntry.Add(new UserEntry(MonoBehaviour.Instantiate(_userEntryPrefab, _localRecordsContent.transform) , i+1, item.Name, item.Score, item.EndTime));
            }
        }

        private void BuildGlobalRecords() 
        {
            _wrongPanel.gameObject.SetActive(false);
            _loadingPanel.SetActive(false);
            for (int i = 0; i < _globalRecordItems.Count; i++)
            {
                var item = _globalRecordItems[i];
                //DateTime time = DateTime.Parse(item.EndTime);
                _globalUserEntry.Add(new UserEntry(MonoBehaviour.Instantiate(_userEntryPrefab, _globalRecordsContent.transform), i + 1, item.Name, item.Score, item.EndTime));
            }
        }
        
        private void OnGetRecords(string json) 
        {
            try
            {
                foreach (var item in _globalUserEntry)
                {
                    item.Dispose();
                }
                _globalUserEntry = new List<UserEntry>();
                _globalRecordItems = JsonConvert.DeserializeObject<List<GlobalRecordItem>>(json);
                BuildGlobalRecords();
            }
            catch (Exception ex)
            {
                OnGetError(json);
            }

        }
        private void OnGetError(string json) 
        {
            _wrongPanel.gameObject.SetActive(true);
            _loadingPanel.SetActive(false);
             Debug.LogError(json);
        }

        private void GetGlobalRecords() 
        {
            _loadingPanel.SetActive(true);
            _networkManager.StartGetData(OnGetRecords, OnGetError);
        }

        public void Show()
        {
            UpdatePanelShow();
            BuildLocalRecords();
            _selfPage.SetActive(true);
        }

        public void Update()
        {
        }

        public void Dispose()
        {
        }

        private void LanguageWasChangedEvent(Enumerators.Language obj)
        {
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            _selfPage.transform.Find("Container/Container_Title/Text_TitleLight").GetComponent<TextMeshProUGUI>().text = _selfPage.transform.Find("Container/Container_Title/Text_TitleDark").GetComponent<TextMeshProUGUI>().text = _selfPage.transform.Find("Container/Container_Title/Text_TitleMain").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_MAIN_MENU_LEADERBOARD");
            _selfPage.transform.Find("Container/Image_Background/Image_TopPanel/Container_Info/Text_Order").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_LEADERBOARD_PLACE");
            _selfPage.transform.Find("Container/Image_Background/Image_TopPanel/Container_Info/Text_Nickname").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_LEADERBOARD_NICKNAME");
            _selfPage.transform.Find("Container/Image_Background/Image_TopPanel/Container_Info/Text_Score").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_SCORE");
            _selfPage.transform.Find("Container/Image_Background/Image_TopPanel/Container_Info/Text_RecordApearanceTIme").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_LEADERBOARD_TIME");
            _globalRecordToggle.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_LEADERBOARD_SWITCH_TO_GLOBAL");
            _localRecordToggle.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_LEADERBOARD_SWITCH_TO_LOCAL");
            _textLoading.text = _localizationManager.GetUITranslation("KEY_LOADING");
            _textReloadButton.text = _localizationManager.GetUITranslation("KEY_RELOAD");
        }


        #region Button handlers
        private void BackToMenuClickHandler()
        {
            Hide();
            _uiManager.SetPage<MainPage>();
        }

        private void OnReloadButtonClickHandler() 
        {
            GetGlobalRecords();
        }

        #endregion
        [Serializable]
        public class GlobalRecordItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Score { get; set; }
            public string EndTime { get; set; }
        }
        private class UserEntry
        {
            private GameObject _selfObject;

            public UserEntry(GameObject prefab, int number, string name, int score, string endTime) 
            {
                _selfObject = prefab;
                _selfObject.transform.Find("Icon_Order/Text_Value").GetComponent<TextMeshProUGUI>().text = number.ToString();
                _selfObject.transform.Find("Text_Nickname").GetComponent<TextMeshProUGUI>().text = name;
                _selfObject.transform.Find("Text_Score").GetComponent<TextMeshProUGUI>().text = score.ToString();
                _selfObject.transform.Find("Text_Time").GetComponent<TextMeshProUGUI>().text = endTime;
            }

            public void Dispose() 
            {
                MonoBehaviour.Destroy(_selfObject);
            }
        }
    }
}