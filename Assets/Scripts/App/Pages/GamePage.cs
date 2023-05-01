using System;
using TandC.RunIfYouWantToLive.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TandC.RunIfYouWantToLive
{
    public class GamePage : IUIElement
    {
        private GameObject _selfObject,
                           _bonusContainer,
                           _backToGameZoneContainer;

        private IUIManager _uiManager;
        private ILoadObjectsManager _loadObjectsManager;
        private IInputManager _inputManager;
        private IGameplayManager _gameplayManager;
        private ILocalizationManager _localizationManager;
        private IDataManager _dataManager;
        private IAdvarismetnManager _advarismetnManager;

        private PlayerController _playerController;
        private ObjectsController _objectsController;
        private EnemyController _enemyController;

        private Joystick _staticJoyStick,
                         _rotationJoyStick,
                         _dynamicJoyStick;

        private Button _rocketShootButton,
                       _laserShootButton,
                       _maskButton,
                       _dashButton,
                       _pauseButton,
                       _testPauseButton,
                       _buttonBonus;

        private Animator _maskReadyAnimator,
                         _dashReadyAnimator,
                         _laserReadyAnimator,
                         _rocketReadyAnimator,
                         _lowHealthIndicatorAnimator,
                         _damageIndicatorAnimator,
                         _bonusAnimator;

        private TextMeshProUGUI _textCurrentLevel,
                                _textExperianceValue,
                                _textHealthValue,
                                _textKillsCount,
                                _rocketCountText,
                                _backToGameZoneTimerText,
                                _textCoinValue,
                                _textBonusTitle;

        private Image _dashFillImage,
                      _maskFillImage,
                      _laserFillImage,
                      _rocketFillImage,
                      _imageHealthBar,
                      _imageExperianceBar,
                      _imageBonus;

        private float _dashTimer,
                      _maxDashTimer,
                      _maskTimer,
                      _maxMaskTimer,
                      _laserTimer,
                      _maxLaserTimer,
                      _rocketTimer,
                      _maxRocketTimer,
                      _backToGameZoneTimer = 5f,
                      _backToGameZoneCurrentTimer,
                      _minValueForAppearBonus = 60f,
                      _maxValueForAppearBonus = 90f,
                      _maxTimerForDesappearBonus = 60f,
                      _currentTimerOfBonus;

        private bool _isMaskActive,
                     _isRocketActive,
                     _isLaserActive,
                     _isDashActive,
                     _backToGameZoneEnabled,
                     _bonusIsActive,
                     _bonusIsInactive;

        private string _level;

        private Enumerators.BonusType _activeBonusType;

        public void Init()
        {
            _uiManager = GameClient.Get<IUIManager>();
            _loadObjectsManager = GameClient.Get<ILoadObjectsManager>();
            _inputManager = GameClient.Get<IInputManager>();
            _gameplayManager = GameClient.Get<IGameplayManager>();
            _localizationManager = GameClient.Get<ILocalizationManager>();
            _dataManager = GameClient.Get<IDataManager>();
            _advarismetnManager = GameClient.Get<IAdvarismetnManager>();

            _selfObject = MonoBehaviour.Instantiate(_loadObjectsManager.GetObjectByPath<GameObject>("Prefabs/UI/GamePage"), _uiManager.Canvas.transform, false);

            _backToGameZoneContainer = _selfObject.transform.Find("Container_BackToGameZone").gameObject;
            _bonusContainer = _selfObject.transform.Find("Container_Bonus").gameObject;

            _rocketShootButton = _selfObject.transform.Find("Rocket_Button").GetComponent<Button>();
            _laserShootButton = _selfObject.transform.Find("Laser_Button").GetComponent<Button>();
            _maskButton = _selfObject.transform.Find("Mask_Button").GetComponent<Button>();
            _dashButton = _selfObject.transform.Find("Dash_Button").GetComponent<Button>();
            _pauseButton = _selfObject.transform.Find("Container_TopPanel/Button_Pause").GetComponent<Button>();
            _testPauseButton = _selfObject.transform.Find("Container_TopPanel/Button_TestPauseGame").GetComponent<Button>();
            _buttonBonus = _bonusContainer.GetComponent<Button>();

            _dashFillImage = _dashButton.transform.Find("RestoreBar_Image").GetComponent<Image>();
            _maskFillImage = _maskButton.transform.Find("RestoreBar_Image").GetComponent<Image>();
            _laserFillImage = _laserShootButton.transform.Find("RestoreBar_Image").GetComponent<Image>();
            _rocketFillImage = _rocketShootButton.transform.Find("RestoreBar_Image").GetComponent<Image>();
            _imageHealthBar = _selfObject.transform.Find("Container_TopPanel/Container_HealthBar/Image_Fillbar").GetComponent<Image>();
            _imageExperianceBar = _selfObject.transform.Find("Container_TopPanel/Container_Experiance/Image_ExperianceFillBar").GetComponent<Image>();
            _imageBonus = _bonusContainer.transform.Find("Image_Icon").GetComponent<Image>();

            _maskReadyAnimator = _maskButton.transform.Find("Image_SkillReady").GetComponent<Animator>();
            _rocketReadyAnimator = _rocketShootButton.transform.Find("Image_SkillReady").GetComponent<Animator>();
            _laserReadyAnimator = _laserShootButton.transform.Find("Image_SkillReady").GetComponent<Animator>();
            _dashReadyAnimator = _dashButton.transform.Find("Image_SkillReady").GetComponent<Animator>();
            _lowHealthIndicatorAnimator = _selfObject.transform.Find("LowHealthIndicator").GetComponent<Animator>();
            _damageIndicatorAnimator = _selfObject.transform.Find("DamageIndicator").GetComponent<Animator>();
            _bonusAnimator = _bonusContainer.GetComponent<Animator>();

            _textCurrentLevel = _selfObject.transform.Find("Container_TopPanel/Container_Experiance/Text_LevelValue").GetComponent<TextMeshProUGUI>();
            _textHealthValue = _selfObject.transform.Find("Container_TopPanel/Container_HealthBar/Text_Value").GetComponent<TextMeshProUGUI>();
            _textKillsCount = _selfObject.transform.Find("Container_LeftPanel/Container_KIlls/Text_Value").GetComponent<TextMeshProUGUI>();
            _textCoinValue = _selfObject.transform.Find("Container_LeftPanel/Container_Coins/Text_Value").GetComponent<TextMeshProUGUI>();
            _rocketCountText = _rocketShootButton.transform.Find("Image_CountBG/Text_RocketValue").GetComponent<TextMeshProUGUI>();
            _backToGameZoneTimerText = _backToGameZoneContainer.transform.Find("Text_Timer").GetComponent<TextMeshProUGUI>();
            _textBonusTitle = _bonusContainer.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>();

            _staticJoyStick = _selfObject.transform.Find("JoyStickStatick").GetComponent<Joystick>();
            _dynamicJoyStick = _selfObject.transform.Find("JoyStickDynamic").GetComponent<Joystick>();
            _rotationJoyStick = _selfObject.transform.Find("JoyStickStatickRotation").GetComponent<Joystick>();

            _pauseButton.onClick.AddListener(PauseButtonOnClickHandler);
            _testPauseButton.onClick.AddListener(OnTestsButtonClickHandler);
            _rocketShootButton.onClick.AddListener(OnRocketShootButtonClickHandler);
            _laserShootButton.onClick.AddListener(OnLaserShootClickHandler);
            _maskButton.onClick.AddListener(OnMaskClickHandler);
            _dashButton.onClick.AddListener(OnDashClickHandler);
            _buttonBonus.onClick.AddListener(BonusButtonOnClickHandler);

            _gameplayManager.ControllerInitEvent += InitGameplay;
            _localizationManager.LanguageWasChangedEvent += LanguageWasChangedEvent;

            _backToGameZoneTimerText.text = "05:00";

            _lowHealthIndicatorAnimator.gameObject.SetActive(false);
            _damageIndicatorAnimator.gameObject.SetActive(false);
            _backToGameZoneContainer.SetActive(false);
            _bonusContainer.SetActive(false);

            ResetBackToGameTimerValue();
            UpdateLocalization();

            Hide();
        }

        private void OnTestsButtonClickHandler()
        {
            if (!_gameplayManager.IsGamePaused)
            {
                _gameplayManager.PauseGame(true);
            }
            else
            {
                _gameplayManager.PauseOff();
            }
        }

        private void BonusButtonOnClickHandler()
        {
            switch (_activeBonusType)
            {
                case Enumerators.BonusType.Damage:
                    _advarismetnManager.ShowAdsVideo(OnSuccessBonusDamageRecevie, OnFailedBonusRecevie);
                    break;
                case Enumerators.BonusType.Freeze:
                    _advarismetnManager.ShowAdsVideo(OnSuccessBonusFreezeRecevie, OnFailedBonusRecevie);
                    break;
                case Enumerators.BonusType.BlowUp:
                    _advarismetnManager.ShowAdsVideo(OnSuccessBonusBlowupRecevie, OnFailedBonusRecevie);
                    break;
            }
            _bonusAnimator.Play("Hide", -1, 0);
            _currentTimerOfBonus = _maxTimerForDesappearBonus;
            _bonusIsActive = false;
            _bonusIsInactive = true;
        }

        private void OnSuccessBonusDamageRecevie()
        {
            _playerController.StartDamageBonus();
        }

        private void OnSuccessBonusFreezeRecevie()
        {
            _objectsController.FreezeEnemies(_playerController.Player.SelfObject.transform.position);
        }

        private void OnSuccessBonusBlowupRecevie()
        {
            _objectsController.BlowupAllEnemies(_playerController.Player.SelfObject.transform.position);
        }

        private void OnFailedBonusRecevie()
        {
            Debug.Log($"<color=#ED0F0F>Reward does not received</color>");
        }

        public void InitializeBonus(Enumerators.BonusType type)
        {

            var bonus = _gameplayManager.GameplayData.GetBonusByType(type);

            if (bonus != null)
            {
                _textBonusTitle.text = _localizationManager.GetUITranslation($"KEY_BONUS_{type.ToString().ToUpper()}");
                _imageBonus.sprite = bonus.sprite;
                _activeBonusType = type;
                _currentTimerOfBonus = UnityEngine.Random.Range(_minValueForAppearBonus, _maxValueForAppearBonus);
                if (_advarismetnManager.IsLoadRewardVideo)
                {
                    _bonusIsActive = true;
                    _bonusContainer.SetActive(true);
                    _bonusAnimator.Play("Show", -1, 0);
                }
                else
                {
                    _currentTimerOfBonus = _maxTimerForDesappearBonus;
                    _bonusIsActive = false;
                    _bonusIsInactive = true;
                }
            }
            else
            {
                Debug.Log($"<color=#ED0F0F>Bonus with type - <color=#18ED0F>[{type}]</color> does not exist!</color>");
            }
        }

        public void CoinEarnEventHandler(int value) => _textCoinValue.text = value.ToString();

        private void InitGameplay()
        {
            _isMaskActive = false;
            _isRocketActive = false;
            _isLaserActive = false;
            _isDashActive = false;
            _rocketShootButton.gameObject.SetActive(false);
            _laserShootButton.gameObject.SetActive(false);
            _maskButton.gameObject.SetActive(false);
            _dashButton.gameObject.SetActive(false);
            _playerController = _gameplayManager.GetController<PlayerController>();
            _objectsController = _gameplayManager.GetController<ObjectsController>();
            _enemyController = _gameplayManager.GetController<EnemyController>();
            _enemyController.ScoreUpdateEvent += UpdateScoreText;
            _playerController.HealthUpdateEvent -= UpdateHealthPanel;
            _playerController.XpUpdateEvent -= UpdateExperianceValue;
            _playerController.LevelUpdateEvent -= UpdateLevelText;
            _playerController.ActiveButtonEvent -= ActiveButtonHandler;
            _playerController.SetTimerForButton -= SetTimers;
            _objectsController.OnPlayerInBorderHandler -= SetBackToGameObjectContainerActive;
            _enemyController.ScoreUpdateEvent -= UpdateScoreText;
            _playerController.UpdateRocketCount -= UpdateCurrentRocketCount;
            _playerController.HealthUpdateEvent += UpdateHealthPanel;
            _objectsController.OnPlayerInBorderHandler += SetBackToGameObjectContainerActive;
            _playerController.XpUpdateEvent += UpdateExperianceValue;
            _playerController.LevelUpdateEvent += UpdateLevelText;
            _playerController.ActiveButtonEvent += ActiveButtonHandler;
            _playerController.SetTimerForButton += SetTimers;
            _playerController.UpdateRocketCount += UpdateCurrentRocketCount;
            _enemyController.ScoreUpdateEvent += UpdateScoreText;
            _playerController.EarnCoinEvent += CoinEarnEventHandler;
            _staticJoyStick.gameObject.SetActive(true);
            _textCoinValue.text = "0";
            //_dynamicJoyStick.gameObject.SetActive(false);
            //Joystick currentJoyStick = _dataManager.CachedUserLocalData.IsStaticJoyStick ? _staticJoyStick : _dynamicJoyStick;
            //currentJoyStick.gameObject.SetActive(true);
            GameClient.Get<IInputManager>().CurrentJoystick = _staticJoyStick;
            GameClient.Get<IInputManager>().RotationJoystick = _rotationJoyStick;
            _currentTimerOfBonus = 30f;
            _bonusIsInactive = true;
        }

        private void ActiveButtonHandler(Enumerators.ActiveButtonType type)
        {
            switch (type)
            {
                case Enumerators.ActiveButtonType.DashButton:
                    _dashButton.gameObject.SetActive(true);
                    _isDashActive = true;
                    break;
                case Enumerators.ActiveButtonType.MaskButton:
                    _maskButton.gameObject.SetActive(true);
                    _isMaskActive = true;
                    break;
                case Enumerators.ActiveButtonType.RocketButton:
                    _rocketShootButton.gameObject.SetActive(true);
                    _isRocketActive = true;
                    break;
                case Enumerators.ActiveButtonType.LaserButton:
                    _laserShootButton.gameObject.SetActive(true);
                    _isLaserActive = true;
                    break;
            }
        }

        public void SetBackToGameObjectContainerActive(bool active)
        {
            if (active)
                ResetBackToGameTimerValue();

            _backToGameZoneContainer.SetActive(active);
            _backToGameZoneEnabled = active;
        }

        public void SetBackToGameTimerText(float value) => _backToGameZoneTimerText.text = string.Format(@"{00:00.00}", value).Replace(',', ':');
        public void ResetBackToGameTimerValue() => _backToGameZoneCurrentTimer = _backToGameZoneTimer;
        private void UpdateScoreText(int score) => _textKillsCount.text = score.ToString();
        private void UpdateLevelText(int level) => _textCurrentLevel.text = _level + " " + level.ToString();

        private void UpdateHealthPanel(float currentHealth, float maxHealth)
        {
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }
            _textHealthValue.text = ((int)currentHealth).ToString() + "/" + maxHealth;
            _imageHealthBar.fillAmount = currentHealth / maxHealth;
            if (currentHealth <= (maxHealth * 0.3f))
            {
                if (!_lowHealthIndicatorAnimator.gameObject.activeInHierarchy)
                {
                    _lowHealthIndicatorAnimator.gameObject.SetActive(true);
                    _lowHealthIndicatorAnimator.Play("LowHealthIndicatorAnimation", -1, 0);
                }
            }
            else
            {
                _lowHealthIndicatorAnimator.gameObject.SetActive(false);
            }
        }

        public void DamageIndicatorShow(bool value)
        {
            _damageIndicatorAnimator.gameObject.SetActive(value);
            if (value)
            {
                _damageIndicatorAnimator.Play("LowHealthIndicatorAnimation", -1, 0);
            }
        }

        private void UpdateExperianceValue(float value, float neededExperiance)
        {
            _imageExperianceBar.fillAmount = value / neededExperiance;
        }

        public void Hide()
        {
            _selfObject.SetActive(false);
        }

        public void Show()
        {
            _selfObject.SetActive(true);
        }

        public void Update()
        {
            if (_dashTimer >= 0)
            {
                _dashTimer -= Time.deltaTime;
                _dashFillImage.fillAmount = _dashTimer / _maxDashTimer;
            }

            if (_maskTimer >= 0)
            {
                _maskTimer -= Time.deltaTime;
                _maskFillImage.fillAmount = _maskTimer / _maxMaskTimer;
            }

            if (_rocketTimer >= 0)
            {
                _rocketTimer -= Time.deltaTime;
                _rocketFillImage.fillAmount = _rocketTimer / _maxRocketTimer;
            }

            if (_laserTimer >= 0)
            {
                _laserTimer -= Time.deltaTime;
                _laserFillImage.fillAmount = _laserTimer / _maxLaserTimer;
            }

            if (_isDashActive)
            {
                if (_dashTimer <= 0)
                {
                    if (!_dashReadyAnimator.gameObject.activeInHierarchy)
                    {
                        _dashReadyAnimator.gameObject.SetActive(true);
                        _dashReadyAnimator.Play("SkillReadyAnimation", -1, 0);
                    }
                }
            }

            if (_isMaskActive)
            {
                if (_maskTimer <= 0)
                {
                    if (!_maskReadyAnimator.gameObject.activeInHierarchy)
                    {
                        _maskReadyAnimator.gameObject.SetActive(true);
                        _maskReadyAnimator.Play("SkillReadyAnimation", -1, 0);
                    }
                }
            }

            if (_isLaserActive)
            {
                if (_laserTimer <= 0)
                {
                    if (!_laserReadyAnimator.gameObject.activeInHierarchy)
                    {
                        _laserReadyAnimator.gameObject.SetActive(true);
                        _laserReadyAnimator.Play("SkillReadyAnimation", -1, 0);
                    }
                }
            }

            if (_isRocketActive)
            {
                if (_rocketTimer <= 0)
                {
                    if (!_rocketReadyAnimator.gameObject.activeInHierarchy)
                    {
                        _rocketReadyAnimator.gameObject.SetActive(true);
                        _rocketReadyAnimator.Play("SkillReadyAnimation", -1, 0);
                    }
                }
            }

            if (_backToGameZoneEnabled)
            {
                _backToGameZoneCurrentTimer -= Time.deltaTime;
                SetBackToGameTimerText(_backToGameZoneCurrentTimer);
                if (_backToGameZoneCurrentTimer <= 0)
                {
                    _playerController.Player.Animator.Play("Start", -1, 0);
                    SetBackToGameTimerText(0f);
                    _backToGameZoneEnabled = false;
                }
            }

            if (_bonusIsActive)
            {
                _currentTimerOfBonus -= Time.deltaTime;
                if (_currentTimerOfBonus <= 0)
                {
                    _bonusAnimator.Play("Hide", -1, 0);
                    _currentTimerOfBonus = _maxTimerForDesappearBonus;
                    _bonusIsActive = false;
                    _bonusIsInactive = true;
                }
            }

            if (_bonusIsInactive)
            {
                _currentTimerOfBonus -= Time.deltaTime;

                if (_currentTimerOfBonus <= 0)
                {
                    _bonusIsInactive = false;
                    InitializeBonus((Enumerators.BonusType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Enumerators.BonusType)).Length));
                }
            }
        }

        private void UpdateCurrentRocketCount(int rocketCurrentCount, int maxRocketCount)
        {
            _rocketCountText.text = $"{rocketCurrentCount}/{maxRocketCount}";
        }

        private void SetTimers(Enumerators.ActiveButtonType type, float time)
        {
            switch (type)
            {
                case Enumerators.ActiveButtonType.DashButton:
                    _dashTimer = time;
                    _maxDashTimer = time;
                    break;
                case Enumerators.ActiveButtonType.MaskButton:
                    _maskTimer = time;
                    _maxMaskTimer = time;
                    float timerForAllButton = _gameplayManager.GetController<PlayerController>().MaskTime;
                    if (_dashTimer <= timerForAllButton)
                    {
                        SetTimers(Enumerators.ActiveButtonType.DashButton, timerForAllButton);
                    }
                    if (_rocketTimer <= timerForAllButton)
                    {
                        SetTimers(Enumerators.ActiveButtonType.RocketButton, timerForAllButton);
                    }
                    if (_laserTimer <= timerForAllButton)
                    {
                        SetTimers(Enumerators.ActiveButtonType.LaserButton, timerForAllButton);
                    }
                    break;
                case Enumerators.ActiveButtonType.RocketButton:
                    _rocketTimer = time;
                    _maxRocketTimer = time;
                    break;
                case Enumerators.ActiveButtonType.LaserButton:
                    _laserTimer = time;
                    _maxLaserTimer = time;
                    break;

            }
            _dashReadyAnimator.gameObject.SetActive(false);
            _laserReadyAnimator.gameObject.SetActive(false);
            _maskReadyAnimator.gameObject.SetActive(false);
            _rocketReadyAnimator.gameObject.SetActive(false);
        }

        public void Dispose()
        {

        }

        #region Button Handlers

        private void OnRocketShootButtonClickHandler()
        {
            if (_rocketTimer > 0)
            {
                return;
            }
            _inputManager.OnRocketClick();
        }
        private void OnLaserShootClickHandler()
        {
            if (_laserTimer > 0)
            {
                return;
            }
            _inputManager.OnLaserClick();
        }
        private void OnDashClickHandler()
        {
            if (_dashTimer > 0)
            {
                return;
            }
            _inputManager.OnDashClick();
        }
        private void OnMaskClickHandler()
        {
            if (_maskTimer > 0)
            {
                return;
            }
            _inputManager.OnMaskClick();
        }
        private void PauseButtonOnClickHandler()
        {
            _uiManager.DrawPopup<PausePopup>();
        }
        #endregion

        private void LanguageWasChangedEvent(Enumerators.Language obj)
        {
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            _level = _localizationManager.GetUITranslation("KEY_GAME_LEVEL");
            _selfObject.transform.Find("Container_BackToGameZone/Image/Text_Warning").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_WARNING");
            _selfObject.transform.Find("Container_BackToGameZone/Image/Text_Warning (1)").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_WARNING");
            _selfObject.transform.Find("Container_BackToGameZone/Text_Info").GetComponent<TextMeshProUGUI>().text = _localizationManager.GetUITranslation("KEY_BACK_TO_GAME_ZONE");
        }
    }
}