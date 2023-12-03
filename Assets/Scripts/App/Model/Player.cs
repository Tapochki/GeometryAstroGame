using System;
using TandC.RunIfYouWantToLive.Helpers;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public class PlayerObject
    {
        public Action<bool> IsShieldTakeDamageEvent;
        public Action<float, float> HealthUpdateEvent;
        public Action<float> XpUpdateEvent;
        public Action<int> LevelUpdateEvent;
        public event Action OnPlayerDiedEvent;

        private GameObject
                                _bodyObject,
                                _selfObject,                               
                                _shoowDetectorObject;
        
        public GameObject SelfObject => _selfObject;
        public int CurrentLevel { get; private set; }
        private int _armorAmount,
                        _currentXp,
                        _maxXp;

        private float _currentHealth,
                      _maxHealth,
                      _movementSpeed;

        private Collider2D _collider;

        private CircleCollider2D _pickUpCollider;

        public Animator Animator;

        private Rigidbody2D _selfRigidbody2D;

        private int _rotateSpeed = 1000;

        public Transform SelfTransform { get; private set; }

        private bool _playerGotShieldSkill,
                        _shieldActive;

        private Joystick _variableJoystick;
        private Joystick _rotationJoystick;
        public bool IsAlive;

        private Dodge _playerDodge;
        private Dash _playerDash;
        private Mask _playerMask;

        public bool IsDash;
        public bool IsMaskActive => _playerMask.IsMaskActive;

        private float _maxTimerToTeleportPlayer = 5f,
                        _currentTimerToTeleportPlayer;
        private bool _teleportPlayerStart;

        public GameObject ModelObject;


        public PlayerObject(GameObject selfObject, PlayerData data, Joystick variableJoystick, Joystick rotationJoystick, float health, float speed, int armor, float startPickUpRadius, GameObject model)
        {
            _selfObject = selfObject;
            _selfRigidbody2D = _selfObject.GetComponent<Rigidbody2D>();
            SelfTransform = _selfObject.transform;
            _variableJoystick = variableJoystick;
            _variableJoystick.UpdateHandleCenter();
            _rotationJoystick = rotationJoystick;
            _rotationJoystick.UpdateHandleCenter();
            _bodyObject = _selfObject.transform.Find("Body").gameObject;
            ModelObject = MonoBehaviour.Instantiate<GameObject>(model, _bodyObject.transform);
            ModelObject.transform.localPosition = Vector3.zero;
            _collider = model.GetComponent<Collider2D>();
            _collider.enabled = true;
            _pickUpCollider = _bodyObject.transform.Find("PickUpCollider").GetComponent<CircleCollider2D>();
            _pickUpCollider.radius = startPickUpRadius;
            Animator = _bodyObject.GetComponent<Animator>();
            _playerDodge = new Dodge(SelfTransform);
            _playerDash = new Dash(SelfTransform, _bodyObject.transform.Find("[Skills]/DashSkill").gameObject);
            _playerMask = new Mask(_collider, ModelObject.gameObject.GetComponent<SpriteRenderer>());
            _shoowDetectorObject = ModelObject.transform.Find("ShootDetector").gameObject;
            _maxHealth = health;
            _currentHealth = _maxHealth;
            _movementSpeed = speed;
            CurrentLevel = data.startedLevel;
            _rotateSpeed = data.rotateSpeed;
            _armorAmount = armor;
            _maxXp = data.startNeedXp;
            _currentXp = 0;
            IsAlive = true;
        }

        public GameObject GetShootDetecrot()
        {
            return _shoowDetectorObject;
        }

        public void StartGameEvent()
        {
            Animator.Play("End", -1, 0);
            LevelUpdateEvent?.Invoke(CurrentLevel);
            HealthUpdateEvent?.Invoke(_currentHealth, _maxHealth);
            XpUpdateEvent?.Invoke(_currentXp);
        }

        public void ShieldImpulse(ShieldSkillVFX shieldVFX, int damage)
        {
            shieldVFX.ShieldOffByImpulse(damage);
        }

        public void SetupShieldSkill(ShieldSkillVFX shieldVFX)
        {
            shieldVFX.IncreaseShieldHealth();
            shieldVFX.ShieldIsActiveEvent += ShieldIsActiveEventHandler;
            IsShieldTakeDamageEvent += (input) =>
            {
                shieldVFX.ShieldTakeDamageEvent?.Invoke();
            };
        }

        public void IncreasePickUpRadius(float value)
        {
            _pickUpCollider.radius += value;
        }

        public void DecreaseShieldCooldownHandler(ShieldSkillVFX shieldVFX, float value) => shieldVFX.DecreaseShieldRecovery(value);
        public void IncreaseShieldHealthHandler(ShieldSkillVFX shieldVFX) => shieldVFX.IncreaseShieldHealth();

        public void AddXp(int addedXp)
        {
            _currentXp += addedXp;
            if (_currentXp >= _maxXp)
            {
                _currentXp -= _maxXp;
                LevelUp();
            }
            XpUpdateEvent?.Invoke(_currentXp);
        }

        public float GetMaxHealthValue() => _maxHealth;
        public float GetMaxExperianceValue() => _maxXp;

        public void TakeDamage(int damage)
        {
            if (IsMaskActive) 
            {
                return;
            }
            if (_playerGotShieldSkill && _shieldActive)
            {
                IsShieldTakeDamageEvent?.Invoke(true);
            }
            else
            {
                IsShieldTakeDamageEvent?.Invoke(false);
                PlayerTakeDamage(damage);
            }
        }

        private void PlayerTakeDamage(int damage)
        {
            float tempDamage = damage - ((float)damage / 100 * _armorAmount);
            _currentHealth -= tempDamage;
            HealthUpdateEvent?.Invoke(_currentHealth, _maxHealth);
            if (_currentHealth <= 0)
            {
                PlayerDie();
            }
        }

        public void PlayerRecieve()
        {
            _selfRigidbody2D.constraints = RigidbodyConstraints2D.None;
            _selfRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            IsAlive = true;
            _bodyObject.gameObject.SetActive(true);
        }

        private void PlayerDie()
        {
            _selfRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            IsAlive = false;
            _bodyObject.gameObject.SetActive(false);
            OnPlayerDiedEvent?.Invoke();
        }

        public void RestoreFullHealth()
        {
            _currentHealth = _maxHealth;
            HealthUpdateEvent?.Invoke(_currentHealth, _maxHealth);
        }
        public void RestoreHealth(int healthValue)
        {
            _currentHealth += healthValue;
            if (_currentHealth >= _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
            HealthUpdateEvent?.Invoke(_currentHealth, _maxHealth);
        }

        public void Update()
        {
            if (_teleportPlayerStart)
            {
                _currentTimerToTeleportPlayer -= Time.deltaTime;

                if (_currentTimerToTeleportPlayer <= 0)
                {
                    _teleportPlayerStart = false;
                    Animator.Play("Start", -1, 0);
                    GameClient.Get<IGameplayManager>().PauseGame(true);
                }
            }
        }

        public void AddArmor(int amount)
        {
            _armorAmount += amount;
        }

        public void IncreaseMaxHealth(float amount)
        {
            _maxHealth += amount;
            _currentHealth += amount;
            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
            HealthUpdateEvent?.Invoke(_currentHealth, _maxHealth);
        }

        public void IncreaseMovementSpeed(float amount)
        {
            _movementSpeed += amount;
        }

        public void GotShieldSkill(bool value)
        {
            _playerGotShieldSkill = value;
        }

        private void ShieldIsActiveEventHandler(bool active)
        {
            _shieldActive = active;
        }

        public void StartAnimationBackToCenter()
        {
            _teleportPlayerStart = true;
            _currentTimerToTeleportPlayer = _maxTimerToTeleportPlayer;
        }

        public void OnAnimationBackToCenterEnd()
        {
            GameClient.Get<IGameplayManager>().PauseGame(false);
            Animator.Play("End", -1, 0);
            SelfTransform.position = new Vector2(0, 0);
            _teleportPlayerStart = false;
        }

        public void FixedUpdate()
        {
            if (_selfObject == null || !IsAlive)
            {
                return;
            }
            _playerDodge.Update();
            _playerDash.Update();
            _playerMask.Update();

            Vector2 movementDirection;
            movementDirection = new Vector2(_variableJoystick.Horizontal, _variableJoystick.Vertical);
            movementDirection.Normalize();
            float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
            Vector2 rotationDirection;
            rotationDirection = new Vector2(_rotationJoystick.Horizontal, _rotationJoystick.Vertical);
            rotationDirection.Normalize();

            _selfObject.transform.Translate(movementDirection * _movementSpeed * (inputMagnitude) * Time.deltaTime, Space.World);
            if (_rotationJoystick.Vertical != 0 && _rotationJoystick.Horizontal != 0)
            {
                Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, rotationDirection);
                _bodyObject.transform.rotation = Quaternion.RotateTowards(_bodyObject.transform.rotation, toRotation, _rotateSpeed * Time.deltaTime);
            }
            if (_variableJoystick.Vertical != 0 && _variableJoystick.Horizontal != 0)
            {
                Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
                _selfObject.transform.rotation = Quaternion.RotateTowards(_selfObject.transform.rotation, toRotation, _rotateSpeed * Time.deltaTime);

                if (_rotationJoystick.Vertical == 0 && _rotationJoystick.Horizontal == 0)
                {
                    _bodyObject.transform.rotation = Quaternion.RotateTowards(_bodyObject.transform.rotation, toRotation, _rotateSpeed * Time.deltaTime);
                }
            }
        }

        public void StartDodge(Vector2 direction, float dodgePower) 
        {
            _playerDodge.StartDodge(direction, dodgePower);
        }

        public void StartDash(float dashTimer = 0.25f)
        {
            IsDash = true;
            _variableJoystick.UpdateHandleCenter();
            _variableJoystick.enabled = false;
            _selfObject.transform.rotation = _bodyObject.transform.rotation;
            _bodyObject.transform.localRotation = Quaternion.identity;
            _playerDash.StartDash(_movementSpeed, dashTimer);
            _playerDash.EndDashEvent += OnDashEndHandler;
        }
        private void OnDashEndHandler() 
        {
            IsDash = false;
            _variableJoystick.enabled = true;
        }

        public void StartMask(float maskTimer = 3f)
        {
            _playerMask.StartMask(maskTimer);
        }
        private void LevelUp()
        {
            _maxXp = (int)InternalTools.GetIncrementalFloatValue(100, 1.2f, CurrentLevel);
            CurrentLevel++;
            LevelUpdateEvent?.Invoke(CurrentLevel);
        }

        public void Dispose()
        {
            MonoBehaviour.Destroy(_selfObject);
        }
    }
}