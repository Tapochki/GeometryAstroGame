using System;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TandC.RunIfYouWantToLive
{
    public class LevelUpPopup : IUIPopup
    {
        public event Action<Enumerators.SkillType> OnSkillChoiceEvent;

        public GameObject Self
        {
            get { return _selfPage; }
        }

        private ILoadObjectsManager _loadObjectManager;
        private IGameplayManager _gameplayManager;
        private IUIManager _uIManager;
        private IAdvarismetnManager _advarismetnManager;
        private ILocalizationManager _localizationManager;
        private GameObject _selfPage;

        private Transform _skillContainer;

        private Button _continueButton,
                        _viewADButton;

        private TextMeshProUGUI _contratulationText;

        public List<SkillItem> _skillItemsList;
        private Enumerators.SkillType _skillType;

        private GameObject _levelUpPrefab;
        private SkillsController _skillsController;
        private PlayerController _playerController;
        private List<Skill> _skills;

        private bool _isReloadOneTime;
        private string _reachedText;

        public void Init()
        {
            _loadObjectManager = GameClient.Get<ILoadObjectsManager>();
            _gameplayManager = GameClient.Get<IGameplayManager>();
            _uIManager = GameClient.Get<IUIManager>();
            _advarismetnManager = GameClient.Get<IAdvarismetnManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();
            _selfPage = MonoBehaviour.Instantiate(_loadObjectManager.GetObjectByPath<GameObject>("Prefabs/UI/Popups/LevelUpPopup"), _uIManager.Canvas.transform);
            _levelUpPrefab = _loadObjectManager.GetObjectByPath<GameObject>("Prefabs/UI/UIObjects/Skill_Presset");
            _skillContainer = _selfPage.transform.Find("Container/Image_Background/Container_Skills");

            _continueButton = _selfPage.transform.Find("Container/Image_Background/Container_Buttons/Button_Confirm").GetComponent<Button>();
            _viewADButton = _selfPage.transform.Find("Container/Image_Background/Container_Buttons/Button_ResetSkill").GetComponent<Button>();

            _contratulationText = _selfPage.transform.Find("Container/Image_Background/Text_Info").GetComponent<TextMeshProUGUI>();

            _continueButton.onClick.AddListener(ContinueButtonOnClickHandler);
            _viewADButton.onClick.AddListener(ViewADButtonOnClickHandler);
            _continueButton.interactable = false;
            _gameplayManager.ControllerInitEvent += InitGameplay;
            _localizationManager.LanguageWasChangedEvent += LanguageWasChangedEvent;
            UpdateLocalization();
            Hide();
        }

        private void InitGameplay()
        {
            _skillsController = _gameplayManager.GetController<SkillsController>();
            _playerController = _gameplayManager.GetController<PlayerController>();
        }

        public void Show()
        { }

        public void Show(object data)
        {
            _isReloadOneTime = false;
            _viewADButton.interactable = true;
            _selfPage.gameObject.SetActive(true);
            _skills = (List<Skill>)data;
            _contratulationText.text = _reachedText + " " + _playerController.Player.CurrentLevel;
            FillSkillList();
            _gameplayManager.PauseGame(true);
        }

        public void Hide()
        {
            _selfPage.gameObject.SetActive(false);
            _continueButton.interactable = false;
        }

        public void Update()
        {
        }

        public void Dispose()
        {
            MonoBehaviour.Destroy(Self);
        }

        public void SetMainPriority()
        {
        }

        private void ContinueButtonOnClickHandler()
        {
            ContinueAfterSelectionSkill();
        }

        private void ItemConfirmSelectionEventHandler(Enumerators.SkillType skillType)
        {
            ContinueAfterSelectionSkill();
        }

        private void ContinueAfterSelectionSkill()
        {
            Hide();
            OnSkillChoiceEvent?.Invoke(_skillType);
            ResetSkillList();
            _gameplayManager.PauseGame(false);
            _playerController.AddXpToPlayer(0);
        }

        private void OnCompleteAds()
        {
            ResetSkillList();
            _skills = _gameplayManager.GetController<SkillsController>().FillUpgradeList();
            FillSkillList();
        }

        private void OnFailedAds()
        {
            Debug.LogError("Fail");
        }

        private void ViewADButtonOnClickHandler()
        {
            if (_isReloadOneTime)
            {
                return;
            }
            _isReloadOneTime = true;
            _viewADButton.interactable = false;
            _advarismetnManager.ShowAdsVideo(OnCompleteAds, OnFailedAds);
        }

        public void FillSkillList()
        {
            _skillItemsList = new List<SkillItem>();

            SkillItem skillItem;

            for (int i = 0; i < _skills.Count; i++)
            {
                Skill skill = _skills[i];
                skillItem = new SkillItem(MonoBehaviour.Instantiate(_levelUpPrefab, _skillContainer), skill.SkillData, skill.SkillUseType != Enumerators.SkillUseType.Additional);
                skillItem.ItemSelectionChangedEvent += ItemSelectEventHandler;
                skillItem.ItemConfirmSelectionEvent += ItemConfirmSelectionEventHandler;
                _skillItemsList.Add(skillItem);
            }
            foreach (var button in _skillItemsList)
            {
                button.Deselect();
            }
        }

        private void ItemSelectEventHandler(Enumerators.SkillType skillType)
        {
            _skillType = skillType;
            _continueButton.interactable = true;
            foreach (var button in _skillItemsList)
            {
                button.Deselect();
            }
        }

        public void ResetSkillList()
        {
            if (_skillItemsList != null)
            {
                foreach (var item in _skillItemsList)
                {
                    item.Dispose();
                }

                _skillItemsList.Clear();
                _skillItemsList = null;
            }
        }

        private void LanguageWasChangedEvent(Enumerators.Language obj)
        {
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            _selfPage.transform.Find("Container/Container_Title/Text_TitleLight").GetComponent<TextMeshProUGUI>().text = _selfPage.transform.Find("Container/Container_Title/Text_TitleDark").GetComponent<TextMeshProUGUI>().text = _selfPage.transform.Find("Container/Container_Title/Text_TitleMain").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_LEVEL_UP_TITLE");
            _continueButton.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_CONFIRM");
            _viewADButton.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_RESET_SKILLS");
            _reachedText = _localizationManager.GetUITranslation("KEY_REACHED_LEVEL");
        }
    }

    public class SkillItem
    {
        public event Action<Enumerators.SkillType> ItemSelectionChangedEvent;

        public event Action<Enumerators.SkillType> ItemConfirmSelectionEvent;

        public GameObject selfObject;

        private Button _selectButton;
        private Button _buttonConfirmSelection;

        public bool isSelect { get; private set; }

        public bool isChestSkill;

        public Enumerators.SkillType skillType;
        public uint skillId;
        private ILocalizationManager _localizationManager;

        public SkillItem(GameObject prefab, SkillsData data, bool isNew, bool isChestSkill = false)
        {
            _localizationManager = GameClient.Get<ILocalizationManager>();
            selfObject = prefab;
            _selectButton = selfObject.GetComponent<Button>();
            _buttonConfirmSelection = _selectButton.transform.Find("Button_Confirm").GetComponent<Button>();
            _buttonConfirmSelection.gameObject.SetActive(false);
            selfObject.transform.Find("Image_IconBackground/Image_Icon").GetComponent<Image>().sprite = data.sprite;
            string text = _localizationManager.GetUITranslation(data.description);
            if (data.isProcent)
            {
                text = text.Replace("%n%", $"<color=#FFBF00>{data.procentIncrease}%</color>");
            }
            else
            {
                text = text.Replace("%n%", $"<color=#FFBF00>{data.Value}</color>");
            }
            selfObject.transform.Find("Text_SkillDescription").GetComponent<TextMeshProUGUI>().text = text;
            selfObject.transform.Find("Text_SkillName").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation(data.name);
            selfObject.transform.Find("Text_NewMark").gameObject.SetActive(isNew);
            skillId = data.id;
            skillType = data.type;
            SetSelection(false);
            this.isChestSkill = isChestSkill;
            _selectButton.onClick.AddListener(SelectButtonOnClickHandler);
            _buttonConfirmSelection.onClick.AddListener(ConfirmSelectionButtonOnClickHandler);

            if (isChestSkill)
            {
                _selectButton.interactable = false;
            }
        }

        private void SelectButtonOnClickHandler()
        {
            SetSelection(!isSelect);
        }

        public void SetSelection(bool state)
        {
            if (isSelect == state)
            {
                return;
            }

            ItemSelectionChangedEvent?.Invoke(skillType);
            isSelect = state;
            selfObject.GetComponent<Image>().color = new Color(0.0f, 0.4f, 0.6f, 1f);
            _buttonConfirmSelection.gameObject.SetActive(true);
        }

        public void Deselect()
        {
            selfObject.GetComponent<Image>().color = new Color(0.0f, 0.32f, 0.32f, 1f);
            isSelect = false;
            _buttonConfirmSelection.gameObject.SetActive(false);
        }

        private void ConfirmSelectionButtonOnClickHandler()
        {
            ItemConfirmSelectionEvent?.Invoke(skillType);
        }

        public void Dispose()
        {
            MonoBehaviour.Destroy(selfObject);
        }
    }
}