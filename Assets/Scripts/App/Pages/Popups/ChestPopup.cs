using System;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using TandC.RunIfYouWantToLive.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TandC.RunIfYouWantToLive
{
    public class ChestPopup : IUIPopup
    {
        public event Action<Enumerators.SkillType> OnSkillChoiceEvent;

        private ILoadObjectsManager _loadObjectManager;
        private IGameplayManager _gameplayManager;
        private IUIManager _uIManager;
        private ILocalizationManager _localizationManager;
        private IDataManager _dataManager;

        private GameObject _selfObject,
                           _container,
                           _skillsContainer,
                           _flashLightContainer,
                           _tapToOpenObject;

        private Button _buttonOk,
                       _buttonTapToOpenChest;

        private Animator _animator;

        private List<Skill> _skills;
        private List<SkillItem> _skillItems;

        private TextMeshProUGUI _textConfirmButton,
                                _textOpenChest,
                                _textCoinValue;

        private CounterAnimation _coinCounter;

        public void Init()
        {
            _loadObjectManager = GameClient.Get<ILoadObjectsManager>();
            _gameplayManager = GameClient.Get<IGameplayManager>();
            _uIManager = GameClient.Get<IUIManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();
            _dataManager = GameClient.Get<IDataManager>();

            _selfObject = MonoBehaviour.Instantiate(_loadObjectManager.GetObjectByPath<GameObject>("Prefabs/UI/Popups/ChestPopup"), _uIManager.Canvas.transform);

            _container = _selfObject.transform.Find("Container").gameObject;
            _skillsContainer = _container.transform.Find("Container_Skills").gameObject;
            _flashLightContainer = _selfObject.transform.Find("Flashlight").gameObject;
            _tapToOpenObject = _flashLightContainer.transform.Find("Text_OpenChest").gameObject;

            _buttonOk = _selfObject.transform.Find("Container/Button_Confirm").GetComponent<Button>();
            _buttonTapToOpenChest = _selfObject.transform.Find("Flashlight").GetComponent<Button>();

            _animator = _flashLightContainer.GetComponent<Animator>();

            _textConfirmButton = _selfObject.transform.Find("Container/Button_Confirm/Text_Title").GetComponent<TextMeshProUGUI>();
            _textOpenChest = _selfObject.transform.Find("Flashlight/Text_OpenChest").GetComponent<TextMeshProUGUI>();
            _textCoinValue = _selfObject.transform.Find("Container_Coins/Text_Coins").GetComponent<TextMeshProUGUI>();

            _buttonOk.onClick.AddListener(OkButtonOnClickHandler);
            _buttonTapToOpenChest.onClick.AddListener(TapToOpenChestButtonOnClickHandler);

            _container.SetActive(false);
            _flashLightContainer.SetActive(true);
            _tapToOpenObject.SetActive(true);

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
            _textConfirmButton.text = _localizationManager.GetUITranslation("KEY_CONFIRM");
            _textOpenChest.text = _localizationManager.GetUITranslation("KEY_TAP_TO_OPEN_CHEST");
        }

        public void Show()
        {
            _selfObject.SetActive(true);
            _container.SetActive(false);
            _flashLightContainer.SetActive(true);
            _tapToOpenObject.SetActive(true);
            _gameplayManager.PauseGame(true);
            _animator.Play("ChestIdle", -1, 0);
            _buttonTapToOpenChest.interactable = true;
            _coinCounter = new CounterAnimation();

        }

        public void Show(object data)
        {
            Show();
            _skills = (List<Skill>)data;
            _skillItems = new List<SkillItem>();
        }

        public void Hide()
        {
            _selfObject.SetActive(false);
        }

        public void Update()
        {
            if (_coinCounter != null)
                _coinCounter.Update();
        }

        public void Dispose()
        {
        }

        public void SetMainPriority()
        {
        }

        public void FillChestSkills()
        {

        }

        #region Button handlers
        private void OkButtonOnClickHandler()
        {
            for (int i = 0; i < _skills.Count; i++)
            {
                OnSkillChoiceEvent?.Invoke(_skills[i].SkillType);
            }
            for (int i = 0; i < _skillItems.Count; i++)
            {
                SkillItem skillItem = _skillItems[i];
                skillItem.selfObject.SetActive(false);
            }
            _skillItems.Clear();
            _skills.Clear();
            _gameplayManager.PauseGame(false);
            _coinCounter = null;
            Hide();
        }

        private void TapToOpenChestButtonOnClickHandler()
        {
            _animator.Play("ChestOpen", -1, 0);
            _buttonTapToOpenChest.interactable = false;

            float coinCount = 0;

            switch (_skills.Count)
            {
                case 1:
                    coinCount = UnityEngine.Random.Range(25, 50);
                    break;
                case 2:
                    coinCount = UnityEngine.Random.Range(50, 100);
                    break;
                case 3:
                    coinCount = UnityEngine.Random.Range(100, 200);
                    break;
                case 4:
                    coinCount = UnityEngine.Random.Range(200, 500);
                    break;
                case 5:
                    coinCount = UnityEngine.Random.Range(500, 1000);
                    break;
            }


            _animator.transform.GetComponent<OnBehaviourHandler>().OnAnimationStringEvent += (string value) =>
                        {
                            for (int i = 0; i < _skills.Count; i++)
                            {
                                _skillItems.Add(new SkillItem(_skillsContainer.transform.Find($"Skill_Presset_{i}").gameObject, _skills[i].SkillData, false, true));
                                _skillItems[i].selfObject.SetActive(true);
                            }

                            _gameplayManager.GetController<PlayerController>().EarnMoney((int)coinCount);

                            _container.SetActive(true);
                            _flashLightContainer.SetActive(false);
                        };
            _coinCounter.Animate(0f, coinCount, 1f, () => UpdateText(_coinCounter.CurrentValue.ToString("###")));
        }

        private void UpdateText(string text)
        {
            _textCoinValue.text = text;
        }
        #endregion
    }
}