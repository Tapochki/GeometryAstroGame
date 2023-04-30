using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using TandC.RunIfYouWantToLive.Common;

namespace TandC.RunIfYouWantToLive
{
    public class GameOverPopup : IUIPopup
    {
        private GameObject _selfObject;

        private IUIManager _uiManager;
        private ILoadObjectsManager _loadObjectsManager;
        private IGameplayManager _gameplayManager;
        private IDataManager _dataManager;
        private INetworkManager _networkManager;
        private IAdvarismetnManager _advarismetnManager;
        private ILocalizationManager _localizationManager;

        private Button _backToMenuButton;
        private Button _recieveButton;

        private bool _isRecieveOneTime;

        private TextMeshProUGUI _scoreValueText;

        private TMP_InputField _nameInputField;
        private int _scoreValue;

        private IEnemyMove _movePart;
        private IEnemyAction _actionPart;
        

        private TextMeshProUGUI _textTitle1, 
                                _textTitle2, 
                                _textTitle3, 
                                _textRecieveButton, 
                                _textContinueButton, 
                                _textEnterNickNameField,
                                _textCoinValue;

        public void Init()
        {
            _uiManager = GameClient.Get<IUIManager>();
            _loadObjectsManager = GameClient.Get<ILoadObjectsManager>();
            _gameplayManager = GameClient.Get<IGameplayManager>();
            _dataManager = GameClient.Get<IDataManager>();
            _networkManager = GameClient.Get<INetworkManager>();
            _advarismetnManager = GameClient.Get<IAdvarismetnManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();
            _selfObject = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/Popups/GameOverPopup"), _uiManager.Canvas.transform, false);

            _backToMenuButton = _selfObject.transform.Find("Container/Image_Background/Button_Continue").GetComponent<Button>();
            _recieveButton = _selfObject.transform.Find("Container/Image_Background/Button_ViewAdd").GetComponent<Button>();
            _scoreValueText = _selfObject.transform.Find("Container/Record_Panel/Text_ScoreValue").GetComponent<TextMeshProUGUI>();
            _nameInputField = _selfObject.transform.Find("Container/Image_Background/InputField_Nickname").GetComponent<TMP_InputField>();
            _textCoinValue = _selfObject.transform.Find("Container/Coin_Panel/Text_CoinValue").GetComponent<TextMeshProUGUI>();

            _textTitle1 = _selfObject.transform.Find("Container/Image_Background/Container_Title/Text_TitleLight").GetComponent<TextMeshProUGUI>();
            _textTitle2 = _selfObject.transform.Find("Container/Image_Background/Container_Title/Text_TitleDark").GetComponent<TextMeshProUGUI>();
            _textTitle3 = _selfObject.transform.Find("Container/Image_Background/Container_Title/Text_TitleMain").GetComponent<TextMeshProUGUI>();
            _textRecieveButton = _selfObject.transform.Find("Container/Image_Background/Button_ViewAdd/Text_Title").GetComponent<TextMeshProUGUI>();
            _textContinueButton = _selfObject.transform.Find("Container/Image_Background/Button_Continue/Text_Title").GetComponent<TextMeshProUGUI>();
            _textEnterNickNameField = _selfObject.transform.Find("Container/Image_Background/InputField_Nickname/Text_Area/Placeholder").GetComponent<TextMeshProUGUI>();

            _backToMenuButton.onClick.AddListener(BackToMenuClickHandler);
            _recieveButton.onClick.AddListener(OnRecieveButtonClickHandler);
            _gameplayManager.GameplayStartedEvent += StartGameInit;
            _nameInputField.onValueChanged.AddListener(UpdateNameField);

            _localizationManager.LanguageWasChangedEvent += LanguageWasChangedEvent;
            UpdateLocalization();
            Hide();
        }

        private void LanguageWasChangedEvent(Enumerators.Language obj)
        {
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            _textTitle1.text = _localizationManager.GetUITranslation("KEY_GAMEOVER_TITLE");
            _textTitle2.text = _localizationManager.GetUITranslation("KEY_GAMEOVER_TITLE");
            _textTitle3.text = _localizationManager.GetUITranslation("KEY_GAMEOVER_TITLE");
            _textRecieveButton.text = _localizationManager.GetUITranslation("KEY_GAMEOVER_ONE_MORE_CHANCE");
            _textContinueButton.text = _localizationManager.GetUITranslation("KEY_CONTINUE");
            _textEnterNickNameField.text = _localizationManager.GetUITranslation("KEY_GAMEOVER_ENTER_NICKNAME");
        }

        private void StartGameInit() 
        {
            _isRecieveOneTime = false;
            _recieveButton.interactable = true;
        }

        public void Hide()
        {
            _selfObject.SetActive(false);
        }

        public void Show()
        {
            _selfObject.SetActive(true);
            _gameplayManager.PauseGame(true);
            _scoreValueText.text = string.Empty;
            _scoreValue = _gameplayManager.GetController<EnemyController>().ScoreCount;
            _textCoinValue.text = _gameplayManager.GetController<PlayerController>().EarnedMoney.ToString();
            _scoreValueText.text = _scoreValue.ToString();
            UpdateNameField(_nameInputField.text);
        }

        private void UpdateNameField(string value)
        {
            if (value != string.Empty)
            {
                _backToMenuButton.interactable = true;
            }
            else
            {
                _backToMenuButton.interactable = false;
            }

        }

        public void Update()
        {

        }

        public void Dispose()
        {

        }

        public void Show(object data)
        {
        }

        public void SetMainPriority()
        {
        }

        private void OnRecieveButtonClickHandler() 
        {
            if (_isRecieveOneTime) 
            {
                return;
            }
            _advarismetnManager.ShowAdsVideo(OnRecieveComplete, OnRecieveFailed);


        }
        private void OnRecieveComplete() 
        {
            _gameplayManager.GetController<PlayerController>().RecievePlayer();
            _isRecieveOneTime = false;
            _recieveButton.interactable = false;
            Hide();
            _gameplayManager.PauseGame(false);
            _uiManager.SetPage<GamePage>();
        }
        private void OnRecieveFailed() 
        {
            Debug.Log("Failed");
        }

        private void RegisterNameInLeaderBoard() 
        {
            if (_nameInputField.text == String.Empty)
            {
                return;
            }
            var recordItem = new RecordItem
            {
                Name = _nameInputField.text,
                Score = _scoreValue,
                EndTime = DateTime.Now.ToString()
            };
            //if (_networkManager.IsHasInternetConnection()) 
            //{
            //    _networkManager.StartSend(recordItem.Name, recordItem.Score, recordItem.EndTime);
            //}
            _dataManager.AddRecord(recordItem);
            _dataManager.SaveAllCache();
        }

        #region Button handlers
        private void BackToMenuClickHandler()
        {
            Hide();
            RegisterNameInLeaderBoard();
            GameClient.Get<IGameplayManager>().StopGameplay();

            if (_advarismetnManager.IsLoadRewardVideo && _gameplayManager.GetController<PlayerController>().EarnedMoney > 0)
                _uiManager.DrawPopup<DoubleMoneyPopup>();
            else 
            {
                _gameplayManager.PlayerGetMoney(_gameplayManager.GetController<PlayerController>().EarnedMoney);
                _uiManager.SetPage<MainPage>();
            }

        }
        #endregion
    }
    [Serializable]
    public class RecordItem 
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public string EndTime {get; set; }
    }
}