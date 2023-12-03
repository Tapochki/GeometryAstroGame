using System;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public class PlayerController : IController
    {
        public Action<int> EarnCoinEvent;

        private IGameplayManager _gameplayManager;
        private IInputManager _inputManager;
        private IUIManager _uiManager;
        private IDataManager _dataManager;

        private VFXController _vfxController;

        private GameplayData _gameData;
        private ShopData _shopData;

        private List<Weapon> _allWeapons;

        public PlayerObject Player { get; private set; }

        private ObjectsController _objectsController;
        private EnemyController _enemyController;

        private event Action<object[]> _gameOverEvent;

        public event Action<Enumerators.ActiveButtonType> ActiveButtonEvent;

        private WeaponLine _weaponLine;

        public DroneParent Drones { get; private set; }

        public event Action<float, float> XpUpdateEvent;

        public event Action<float, float> HealthUpdateEvent;

        public event Action<int> LevelUpdateEvent;

        public event Action<int> ScoreUpdateEvent;

        public int CriticalChanceProcent { get; private set; }
        public float CriticalDamageMultiplier { get; private set; }
        public Action<Enumerators.ActiveButtonType, float> SetTimerForButton;
        public Action<int, int> UpdateRocketCount;

        private INetworkManager _networkManager;

        private float _dashRecoverTime;
        public int DashDamage;

        private float _mascRecoverTime;
        public float MaskTime;

        private float _rocketRecoverTime;
        private int _rocketMaxCount;
        private int _rocketCurrentCount;
        public int RocketBlowDamage;

        private float _laserRecoverTime;
        private float _laserShotSize;

        private bool _isRestorePlayerByTime;
        private int _restoreHealthCount;
        private float _restoreHealthTimer;
        private float _restoreHealthTime;

        private float _dodgePower;
        private float _dodgeTimer;
        private float _dodgeTime;
        private bool _isCanDodge;

        private float _xpMultiplier;

        public int BombDamage;
        public float DamageMultiplier;

        private float _recoverTimerMultiplier;
        private float _dashTime;

        public int EarnedMoney;

        private float _moneyMultiplier;

        private const float _damageMultiplierFromBonus = 2f;
        private const float _damageMultiplierTime = 30f;
        private float _damageMultiplierTimer;
        private bool _isStartDamageBonusMultiplier;

        private MeshRenderer _back_0Material,
            _back_1Material,
            _back_2Material;

        public PlayerController()
        {
        }

        public void Dispose()
        {
        }

        public void IncreaseCriticalChanceProcent(int value)
        {
            CriticalChanceProcent += value;
        }

        public void IncreaseCriticalDamageMultiplier(float value)
        {
            CriticalDamageMultiplier += value;
        }

        public void UpgradeDashTime(float value)
        {
            _dashTime += value;
        }

        public void Init()
        {
            _gameplayManager = GameClient.Get<IGameplayManager>();
            _inputManager = GameClient.Get<IInputManager>();
            _uiManager = GameClient.Get<IUIManager>();
            _networkManager = GameClient.Get<INetworkManager>();
            _dataManager = GameClient.Get<IDataManager>();
            _vfxController = _gameplayManager.GetController<VFXController>();
            _enemyController = _gameplayManager.GetController<EnemyController>();
            _objectsController = _gameplayManager.GetController<ObjectsController>();
            _gameplayManager.GameplayStartedEvent += GameplayStartedEventHandler;
            _inputManager.OnDashClickHandler += PlayerDash;
            _inputManager.OnMaskClickHandler += PlayerMask;
            _inputManager.OnRocketShootClickHandler += OnRocketClickHandler;
            _inputManager.OnLaserShootClickHandler += OnLaserClickHandler;
            _inputManager.OnSwipeEvent += PlayerOnSwipeEventHandler;
        }

        public void PlayerOnSwipeEventHandler(Vector2 swipeDirection)
        {
            if (_isCanDodge)
            {
                Debug.LogError(12);
                Player.StartDodge(swipeDirection, _dodgePower);
                _dodgeTime = _dodgeTimer;
                _isCanDodge = false;
            }
        }

        public void UpgradeWeaponDoubleShot()
        {
            GetWeapon<DefaultWeapon>().UpgradeDoubleShoot();
        }

        public void UpgradeWeaponShotAfterShot()
        {
            GetWeapon<DefaultWeapon>().UpgradeShootAfterShoot();
        }

        public void IncreaseDefaultBulletSpeed<T>(int value) where T : Weapon
        {
            GetWeapon<T>().IncreaseBulletSpeed(value);
        }

        public void IncreaseDamage(float value)
        {
            DamageMultiplier += value;
        }

        public void ActiveButton(Enumerators.ActiveButtonType type)
        {
            ActiveButtonEvent?.Invoke(type);
        }

        public void UpgradeDronesCount()
        {
            Drones.RegisterNewDrone();
        }

        public void UpgradeDronesDamage(int value)
        {
            Drones.UpgradeDamage(value);
        }

        public void UpgradeDroneSpeed(float value)
        {
            Drones.UpgradeDroneSpeed(value);
        }

        private void GameplayStartedEventHandler()
        {
            _allWeapons = new List<Weapon>();
            Transform parent = _gameplayManager.GameplayObject.transform.Find("[Player]");
            _gameData = _gameplayManager.GameplayData;
            _shopData = _gameplayManager.ShopData;
            GameObject playerObject = MonoBehaviour.Instantiate(_gameData.playerData.playerPrefab, parent, false);
            Player = new PlayerObject(playerObject, _gameData.playerData, _inputManager.CurrentJoystick, _inputManager.RotationJoystick,
                _shopData.GetUpgradeValueByType(Enumerators.UpgradeType.Health), _shopData.GetUpgradeValueByType(Enumerators.UpgradeType.Speed), (int)_shopData.GetUpgradeValueByType(Enumerators.UpgradeType.Armor),
                _shopData.GetUpgradeValueByType(Enumerators.UpgradeType.PickUpRadius), _gameplayManager.GetSelectedProduct(Enumerators.CustomisationType.Player));
            CriticalChanceProcent = (int)_shopData.GetUpgradeValueByType(Enumerators.UpgradeType.CriticalChance);
            CriticalDamageMultiplier = _shopData.GetUpgradeValueByType(Enumerators.UpgradeType.CriticalDamageMultiplier);

            _back_0Material = _gameplayManager.GameplayCamera.transform.Find("BasicLevel").GetComponent<MeshRenderer>();
            _back_1Material = _gameplayManager.GameplayCamera.transform.Find("BasicLevel/Back_0").GetComponent<MeshRenderer>();
            _back_2Material = _gameplayManager.GameplayCamera.transform.Find("BasicLevel/Back_1").GetComponent<MeshRenderer>();

            _dodgeTimer = _gameData.playerData.StartDodgeRecoverTimer;
            _dodgePower = _gameData.playerData.StartDodgePower;
            _isCanDodge = true;

            Player.HealthUpdateEvent += UpdateHealth;
            Player.XpUpdateEvent += UpdateXp;
            Player.LevelUpdateEvent += UpdateLevel;
            Player.OnPlayerDiedEvent += PlayerDieHandler;
            Player.StartGameEvent();
            BombDamage = _gameData.playerData.BombDamage;
            Drones = new DroneParent(MonoBehaviour.Instantiate(_gameData.playerData.DroneData.prefab, parent), _gameData.playerData.DroneData.StartDroneSpeed, _gameData.playerData.DroneData.StartDroneDamage, playerObject.transform, _gameplayManager.GetSelectedProduct(Enumerators.CustomisationType.Drones));
            Drones.OnDroneHandlerEnter += OnDroneGiveDamageHandler;
            TakeDefaultWeaponWeapons();
            _isRestorePlayerByTime = false;
            _xpMultiplier = 1f;
            DamageMultiplier = _shopData.GetUpgradeValueByType(Enumerators.UpgradeType.DamageMultiplier);
            _recoverTimerMultiplier = _shopData.GetUpgradeValueByType(Enumerators.UpgradeType.RecoverTimerMultiplier);
            _moneyMultiplier = _shopData.GetUpgradeValueByType(Enumerators.UpgradeType.MoneyMultiplier);
            _dashTime = _gameData.playerData.StartDashTime;
            EarnedMoney = 0;
        }

        private void OnDroneGiveDamageHandler(GameObject enemy, GameObject drone, float damage)
        {
            _enemyController.HitEnemy(enemy, damage, _gameData.DropChance.DronChance);
        }

        public void ActivateLaserGun()
        {
            ActiveWeapon(Enumerators.WeaponType.LaserGun);
            _laserShotSize = _gameData.playerData.StartLaserShotSize;
            _laserRecoverTime = _gameData.playerData.StartLaserShotTime;
            ActiveButtonEvent(Enumerators.ActiveButtonType.LaserButton);
            SetLaserShotSize(_laserShotSize);
        }

        public void UpgradeLaserGunSize(float value)
        {
            _laserShotSize += value;
            SetLaserShotSize(_laserShotSize);
        }

        public void DecreaseReloadTimer(float value)
        {
            _recoverTimerMultiplier -= value;
            foreach (var weapon in _allWeapons)
            {
                weapon.SetRecoverTimeMultiplier(_recoverTimerMultiplier);
            }
        }

        private void SetLaserShotSize(float size)
        {
            GameObject laserShotObject = _gameData.GetBulletByType(Enumerators.WeaponType.LaserGun).ButlletObject;
            laserShotObject.transform.localScale = new Vector2(size, laserShotObject.transform.localScale.y);
        }

        public void IncreseXpMultiplier(float value)
        {
            _xpMultiplier += value;
        }

        private void GameOverHandler(object data = null)
        {
            GameClient.Instance.GetService<ITimerManager>().StopTimer(_gameOverEvent);
            _gameOverEvent -= GameOverHandler;
            _uiManager.DrawPopup<GameOverPopup>();
        }

        private void PlayerDieHandler()
        {
            Drones.HideAll();
            _enemyController.StopStartEnemy(false);
            Player.OnPlayerDiedEvent -= PlayerDieHandler;
            _vfxController.SpawnDeathParticles(Player.SelfTransform.position);
            _gameOverEvent += GameOverHandler;
            GameClient.Instance.GetService<ITimerManager>().AddTimer(_gameOverEvent, null, 2f);
        }

        private void UpdateHealth(float health, float maxHealth)
        {
            HealthUpdateEvent?.Invoke(health, Player.GetMaxHealthValue());
        }

        private void UpdateLevel(int level)
        {
            LevelUpdateEvent?.Invoke(level);
        }

        private void UpdateXp(float xpCount)
        {
            XpUpdateEvent?.Invoke(xpCount, Player.GetMaxExperianceValue());
        }

        public void AddXpToPlayer(int xpCount)
        {
            Player.AddXp((int)(xpCount * _xpMultiplier));
        }

        public void RestoreHelathPlayer(int amount)
        {
            _vfxController.SpawnDamagePointVFX(Player.SelfObject.transform.position, amount, Color.green);
            Player.RestoreHealth(amount);
        }

        public void FullRestorePlayerHealth()
        {
            Player.RestoreFullHealth();
        }

        public void ActiveMaskSkill()
        {
            ActiveButtonEvent?.Invoke(Enumerators.ActiveButtonType.MaskButton);
            _mascRecoverTime = _gameData.playerData.StartMaskRecoverTime;
            MaskTime = _gameData.playerData.StartMaskActiveTime;
        }

        public void MaskTimeActiveIncrease(float value)
        {
            MaskTime += value;
        }

        public void PlayerMask()
        {
            SetTimerForButton?.Invoke(Enumerators.ActiveButtonType.MaskButton, _mascRecoverTime * _recoverTimerMultiplier);
            Player.StartMask(MaskTime);
        }

        public void UpgradeRocketMaxCount(int value)
        {
            _rocketMaxCount += value;
            UpdateRocketCount?.Invoke(_rocketCurrentCount, _rocketMaxCount);
        }

        public bool OnGetRocketBox()
        {
            if (IfRocketIsMax())
            {
                return false;
            }
            _rocketCurrentCount++;
            UpdateRocketCount?.Invoke(_rocketCurrentCount, _rocketMaxCount);
            return true;
        }

        public bool IfRocketIsMax()
        {
            if (_rocketCurrentCount == _rocketMaxCount)
            {
                return true;
            }
            return false;
        }

        public void BackPlayerToCenter(int value)
        {
            Player.TakeDamage(value);
            Player.OnAnimationBackToCenterEnd();
        }

        public void BackPlayerToCenterStart()
        {
            Player.StartAnimationBackToCenter();
        }

        private void ShootHandler(Vector2 shotPosition, Vector2 direction, float damage, int dropChance, BulletData bulletData, int bulletLife = 1)
        {
            _objectsController.WeaponShoot((PlayerBulletData)bulletData, shotPosition, direction, damage, dropChance, bulletLife);
        }

        public void IncreasePlayerPickUpRadius(float value)
        {
            Player.IncreasePickUpRadius(value);
        }

        public void GetDash()
        {
            ActiveButtonEvent?.Invoke(Enumerators.ActiveButtonType.DashButton);
            _dashRecoverTime = _gameData.playerData.StartDashTimeRecover;
            DashDamage = _gameData.playerData.StartDashDamage;
        }

        private void PlayerDash()
        {
            SetTimerForButton?.Invoke(Enumerators.ActiveButtonType.DashButton, _dashRecoverTime * _recoverTimerMultiplier);
            Player.StartDash(_dashTime);
        }

        public void IncreaseDashDamage(int damage)
        {
            DashDamage += damage;
        }

        private void OnRocketClickHandler()
        {
            if (_rocketCurrentCount <= 0)
            {
                return;
            }
            _rocketCurrentCount--;
            UpdateRocketCount?.Invoke(_rocketCurrentCount, _rocketMaxCount);
            OnClickWeapon(Enumerators.WeaponType.RocketLauncer, Enumerators.ActiveButtonType.RocketButton, _rocketRecoverTime * _recoverTimerMultiplier);
        }

        private void OnLaserClickHandler()
        {
            OnClickWeapon(Enumerators.WeaponType.LaserGun, Enumerators.ActiveButtonType.LaserButton, _laserRecoverTime * _recoverTimerMultiplier);
        }

        public void ActivatePlayerRestoreHealth()
        {
            _isRestorePlayerByTime = true;
            _restoreHealthTime = _gameData.playerData.StartHealthRestoreTime;
            _restoreHealthCount = _gameData.playerData.StartHealthCountRestoreByTime;
            _restoreHealthTimer = _restoreHealthTime;
        }

        public void RestoreHealthByTime()
        {
            RestoreHelathPlayer(_restoreHealthCount);
            _restoreHealthTimer = _restoreHealthTime;
        }

        public void DecreaseRestoreHelathTimer(float value)
        {
            _restoreHealthTime -= value;
        }

        public void IncreseRestoreCountHealth(int value)
        {
            _restoreHealthCount += value;
        }

        private void OnClickWeapon(Enumerators.WeaponType type, Enumerators.ActiveButtonType buttonType, float recoverTime)
        {
            SetTimerForButton?.Invoke(buttonType, recoverTime * _recoverTimerMultiplier);
            GetWeaponByType(type).ClickShoot();
        }

        public void AddsPlayerAnotherChance()
        {
            Player.AddAnotherChance();
        }

        public void RecievePlayer()
        {
            FullRestorePlayerHealth();
            Player.StartMask();
            Player.PlayerRecieve();
            Player.OnPlayerDiedEvent += PlayerDieHandler;
            _objectsController.FreezeEnemies(Player.SelfObject.transform.position);
            _enemyController.StopStartEnemy(true);
        }

        public void ActivateRocket()
        {
            _rocketMaxCount = _gameData.playerData.StartRocketCount;
            _rocketCurrentCount = _gameData.playerData.StartRocketCount;
            _rocketRecoverTime = _gameData.playerData.StartRocketRecoverTime;
            RocketBlowDamage = _gameData.playerData.StartRocketDamage;
            UpdateRocketCount?.Invoke(_rocketCurrentCount, _rocketMaxCount);
            ActiveButtonEvent?.Invoke(Enumerators.ActiveButtonType.RocketButton);
        }

        public void EarnMoney(int value)
        {
            EarnedMoney += (int)(value * _moneyMultiplier);
            EarnCoinEvent?.Invoke(EarnedMoney);
        }

        public void UpgradeRocketBlow(int value)
        {
            RocketBlowDamage += value;
        }

        public void TakeWeapon(PlayerWeaponData data)
        {
            Weapon weapon;
            bool isButtonWeapon = false;
            switch (data.type)
            {
                case Enumerators.WeaponType.Standart:
                    _weaponLine = new WeaponLine(Player.SelfObject.transform.Find("Body/[Weapon]/ShootLine").gameObject);
                    weapon = new DefaultWeapon();
                    break;

                case Enumerators.WeaponType.RocketLauncer:
                    isButtonWeapon = true;
                    weapon = new RocketWeapon();
                    break;

                case Enumerators.WeaponType.LaserGun:
                    isButtonWeapon = true;
                    weapon = new LaserWeapon();
                    break;

                case Enumerators.WeaponType.AutoGun:
                    weapon = new AutoWeapon();
                    break;

                case Enumerators.WeaponType.Minigun:
                    weapon = new MinigunWeapon();
                    break;

                case Enumerators.WeaponType.EnergyGun:
                    weapon = new EnergyWeapon();
                    break;

                default:
                    weapon = new DefaultWeapon();
                    break;
            }
            weapon.Init(Player.SelfObject.transform.Find($"Body/[Weapon]/{data.weaponName}").gameObject, data, _gameData.GetBulletByType(data.type), _gameData.DropChance.StandartShotChance, Player.GetShootDetecrot(), isButtonWeapon);
            _allWeapons.Add(weapon);
        }

        public Weapon GetWeaponByType(Enumerators.WeaponType type)
        {
            foreach (Weapon weapon in _allWeapons)
            {
                if (weapon.WeaponType == type)
                {
                    return weapon;
                }
            }
            return null;
        }

        public T GetWeapon<T>() where T : Weapon
        {
            foreach (var weapon in _allWeapons)
            {
                if (weapon is T)
                {
                    return (T)weapon;
                }
            }

            throw new Exception("Weapon " + typeof(T).ToString() + " have not implemented");
        }

        public void ActiveWeapon(Enumerators.WeaponType type)
        {
            var weapon = GetWeaponByType(type);
            if (weapon != null)
            {
                if (weapon != null)
                {
                    weapon.OnShootEventHandler -= ShootHandler;
                    weapon.ActiveWeapon(false);
                }
                weapon.OnShootEventHandler += ShootHandler;
                weapon.ActiveWeapon(true);
            }
            else
            {
                throw new System.Exception();
            }
            weapon.ActiveWeapon();
        }

        private void TakeDefaultWeaponWeapons()
        {
            foreach (var weapon in _gameData.weaponData)
            {
                TakeWeapon(weapon);
            }
            ActiveWeapon(_gameData.playerData.StartWeaponType);
        }

        public void ResetAll()
        {
            foreach (var weapon in _allWeapons)
            {
                weapon.OnShootEventHandler -= ShootHandler;
            }
            _allWeapons.Clear();
            Player.HealthUpdateEvent -= UpdateHealth;
            Player.XpUpdateEvent -= UpdateXp;
            Player.LevelUpdateEvent -= UpdateLevel;
            Player = null;
        }

        public VFXBase GetSkillOnPlayer(Enumerators.SkillType type)
        {
            return _vfxController.GetSkillVFXByType(type);
        }

        public void ImpulsePlayer(int damage)
        {
            if (_vfxController.GetSkillVFXByType(Enumerators.SkillType.Shield) != null)
            {
                Player.ShieldImpulse(_vfxController.GetSkillVFXByType(Enumerators.SkillType.Shield) as ShieldSkillVFX, damage);
            }
            foreach (var weapon in _allWeapons)
            {
                if (weapon.IsActive)
                {
                    weapon.SetImpulseToWeapon(damage);
                }
            }
            _vfxController.SpawnImpulseHitPlayerParticles(Player.SelfObject.transform.position);
            SetTimerForButton?.Invoke(Enumerators.ActiveButtonType.RocketButton, _rocketRecoverTime * _recoverTimerMultiplier * damage);
            SetTimerForButton?.Invoke(Enumerators.ActiveButtonType.MaskButton, _mascRecoverTime * _recoverTimerMultiplier * damage);
            SetTimerForButton?.Invoke(Enumerators.ActiveButtonType.DashButton, _dashRecoverTime * _recoverTimerMultiplier * damage);
        }

        public void AddArmor(int amount)
        {
            Player.AddArmor(amount);
        }

        public void IncreaseMaxHealth(float amount)
        {
            Player.IncreaseMaxHealth(amount);
        }

        public void IncreaseMovementSpeed(float amount)
        {
            Player.IncreaseMovementSpeed(amount);
        }

        public void PlayerGotShieldSkill(bool value)
        {
            Player.GotShieldSkill(value);
        }

        public void StartDamageBonus()
        {
            DamageMultiplier += _damageMultiplierFromBonus;
            _damageMultiplierTimer = _damageMultiplierTime;
            (_uiManager.GetPage<GamePage>() as GamePage).DamageIndicatorShow(true);
            _isStartDamageBonusMultiplier = true;
        }

        private void EndDamageBonus()
        {
            _isStartDamageBonusMultiplier = false;
            (_uiManager.GetPage<GamePage>() as GamePage).DamageIndicatorShow(false);
            DamageMultiplier -= _damageMultiplierFromBonus;
        }

        public void Update()
        {
            if (!_gameplayManager.IsGameplayStarted)
            {
                return;
            }

            if (Player != null)
            {
                _gameplayManager.GameplayCamera.transform.position = Player.SelfObject.transform.position;
                Player.Update();
            }
            if (!_gameplayManager.IsGameplayStarted || !Player.IsAlive)
            {
                return;
            }

            if (_isRestorePlayerByTime)
            {
                _restoreHealthTimer -= Time.deltaTime;
                if (_restoreHealthTimer <= 0)
                {
                    RestoreHealthByTime();
                }
            }
            if (!_isCanDodge)
            {
                _dodgeTime -= Time.deltaTime;
                (_uiManager.GetPage<GamePage>() as GamePage).UpdateDodgePanel(_dodgeTime, _dodgeTimer);
                if (_dodgeTime <= 0)
                {
                    _isCanDodge = true;
                }
            }
            if (_isStartDamageBonusMultiplier)
            {
                _damageMultiplierTimer -= Time.deltaTime;
                if (_damageMultiplierTimer <= 0)
                {
                    EndDamageBonus();
                }
            }
            Drones.Update();
            foreach (var weapon in _allWeapons)
            {
                if (!weapon.IsActive)
                {
                    continue;
                }
                if (weapon != null)
                {
                    if (Player.IsMaskActive || Player.IsDashActive)
                    {
                        return;
                    }
                    if (weapon.WeaponType == Enumerators.WeaponType.Standart)
                    {
                        if (_weaponLine != null && _weaponLine.IsEnemyOnLine)
                        {
                            weapon.CanShoot();
                        }
                    }
                    else
                    {
                        weapon.CanShoot();
                    }
                }
                weapon.Update();
            }
        }

        public void FixedUpdate()
        {
            if (!_gameplayManager.IsGameplayStarted)
            {
                return;
            }

            if (Player != null)
            {
                Player.FixedUpdate();

                _back_0Material.material.mainTextureOffset = Player.SelfTransform.position / 15 * Time.deltaTime;
                _back_1Material.material.mainTextureOffset = Player.SelfTransform.position / 10 * Time.deltaTime;
                _back_2Material.material.mainTextureOffset = Player.SelfTransform.position / 8 * Time.deltaTime;
            }
        }
    }
}