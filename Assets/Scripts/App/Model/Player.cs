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

        private GameObject //_healthBar,
                                _bodyObject,
                                _selfObject,
                                _dashSkillContainer,
                                _shoowDetectorObject;

        private SpriteRenderer _spriteRenderer;
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

        private int _rotateSpeed = 1000;

        public Transform SelfTransform { get; private set; }

        private bool _playerGotShieldSkill,
                        _shieldActive;

        private Joystick _variableJoystick;
        private Joystick _rotationJoystick;
        public bool IsAlive;

        public bool IsDashActive;
        private float _dashTimer;
        private bool _isDash;

        private float _dodgeTimer = 0.1f;
        private bool _isDodge;
        private float _dodgePower;
        private Vector2 _dodgeDirection;

        public bool IsMaskActive;
        private float _maskTimer;

        private bool _setNormalRotation;

        private float _maxTimerToTeleportPlayer = 5f,
                        _currentTimerToTeleportPlayer;

        private bool _teleportPlayerStart;

        public GameObject ModelObject;

        private bool _isHaveAnotherChance;

        public PlayerObject(GameObject selfObject, PlayerData data, Joystick variableJoystick, Joystick rotationJoystick, float health, float speed, int armor, float startPickUpRadius, GameObject model)
        {
            _selfObject = selfObject;
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
            _dashSkillContainer = _bodyObject.transform.Find("[Skills]/DashSkill").gameObject;
            _shoowDetectorObject = ModelObject.transform.Find("ShootDetector").gameObject;
            _spriteRenderer = ModelObject.gameObject.GetComponent<SpriteRenderer>();
            _maxHealth = health;
            _currentHealth = _maxHealth;
            _movementSpeed = speed;
            CurrentLevel = data.startedLevel;
            _rotateSpeed = data.rotateSpeed;
            _armorAmount = armor;
            _maxXp = data.startNeedXp;
            _currentXp = 0;
            _isHaveAnotherChance = false;
            IsAlive = true;
            _setNormalRotation = true;
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
            _isHaveAnotherChance = false;
            IsAlive = true;
            _bodyObject.gameObject.SetActive(true);
        }

        private void PlayerDie()
        {
            if (!IsAlive)
            {
                return;
            }

            if (_isHaveAnotherChance)
            {
                PlayerRecieve();
                RestoreHealth(10);
            }
            else
            {
                IsAlive = false;
                _bodyObject.gameObject.SetActive(false);
                OnPlayerDiedEvent?.Invoke();
            }
        }

        public void RestoreFullHealth()
        {
            _currentHealth = _maxHealth;
            //Debug.LogError($"Current Hp: {_currentHealth} Max Hp {_maxHealth}");
            HealthUpdateEvent?.Invoke(_currentHealth, _maxHealth);
        }

        public bool IsPlayerHaveAnotherChance() => _isHaveAnotherChance;

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
            //Start Animation
            _teleportPlayerStart = true;
            _currentTimerToTeleportPlayer = _maxTimerToTeleportPlayer;
        }

        public void OnAnimationBackToCenterEnd()
        {
            // On Animation end
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
            if (_isDash)
            {
                Dash();
                //return;
            }
            if (_isDodge)
            {
                Dodge();
            }
            if (IsMaskActive)
            {
                _maskTimer -= Time.deltaTime;
                if (_maskTimer <= 0)
                {
                    EndMask();
                }
            }
            Vector2 movementDirection;
            movementDirection = new Vector2(_variableJoystick.Horizontal, _variableJoystick.Vertical);
            float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
            movementDirection.Normalize();

            Vector2 rotationDirection;
            rotationDirection = new Vector2(_rotationJoystick.Horizontal, _rotationJoystick.Vertical);
            float rotationMagnitude = Mathf.Clamp01(rotationDirection.magnitude);
            rotationDirection.Normalize();

            _selfObject.transform.Translate(movementDirection * _movementSpeed * (inputMagnitude) * Time.deltaTime, Space.World);

            if (_rotationJoystick.Vertical != 0 && _rotationJoystick.Horizontal != 0)
            {
                Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, rotationDirection);
                _bodyObject.transform.rotation = Quaternion.RotateTowards(_bodyObject.transform.rotation, toRotation, _rotateSpeed * Time.deltaTime);
                _setNormalRotation = false;
            }
            if (_variableJoystick.Vertical != 0 && _variableJoystick.Horizontal != 0)
            {
                Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
                _selfObject.transform.rotation = Quaternion.RotateTowards(_selfObject.transform.rotation, toRotation, _rotateSpeed * Time.deltaTime);

                if (_rotationJoystick.Vertical == 0 && _rotationJoystick.Horizontal == 0)
                {
                    //if (!_setNormalRotation)
                    //{
                    _bodyObject.transform.rotation = Quaternion.RotateTowards(_bodyObject.transform.rotation, toRotation, _rotateSpeed * Time.deltaTime);
                    //_modelObject.transform.localRotation = new Quaternion(0,0,0,0);
                    //_setNormalRotation = true;
                    // }
                }
            }
        }

        private void Dash()
        {
            //if (_rotationJoystick.Vertical != 0 && _rotationJoystick.Horizontal != 0)
            //{
            //    //Vector2 rotationDirection;
            //   // rotationDirection = new Vector2(_rotationJoystick.Horizontal, _rotationJoystick.Vertical);
            //    //Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, rotationDirection);

            //    //Debug.LogError($" _selfObject {_selfObject.transform.rotation}  _modelObject {_modelObject.transform.rotation} toRotate {toRotation}");
            //}
            _selfObject.transform.rotation = _bodyObject.transform.rotation;
            _bodyObject.transform.localRotation = Quaternion.identity;
            _bodyObject.transform.localRotation = Quaternion.identity;
            if (IsDashActive)
            {
                _selfObject.transform.Translate(Vector2.up * _movementSpeed * 10 * Time.deltaTime);
            }
            _dashTimer -= Time.deltaTime;
            if (_dashTimer < 0)
            {
                IsDashActive = false;
                _variableJoystick.enabled = true;
                if (_dashTimer <= -0.5f)
                {
                    _isDash = false;
                    _dashSkillContainer.SetActive(false);
                }
            }
        }

        public void Dodge()
        {
            int playerDirection = _selfObject.transform.rotation.eulerAngles.z > 90f || _selfObject.transform.rotation.eulerAngles.z < -90f ? -1 : 1;
            _selfObject.transform.Translate(_dodgeDirection * _dodgePower * _movementSpeed * playerDirection * Time.deltaTime);
            _dodgeTimer -= Time.deltaTime;
            if (_dodgeTimer < 0)
            {
                _variableJoystick.enabled = true;
                _isDodge = false;
            }
        }

        public void StartDodge(Vector2 direction, float dodgePower)
        {
            _dodgeTimer = 0.1f;
            _dodgeDirection = direction;
            _dodgePower = dodgePower;
            _isDodge = true;
            _variableJoystick.enabled = false;
        }

        public void StartDash(float dashTimer = 0.25f)
        {
            _dashTimer = dashTimer;
            IsDashActive = true;
            _variableJoystick.UpdateHandleCenter();
            _variableJoystick.enabled = false;
            _dashSkillContainer.SetActive(true);
            _isDash = true;
        }

        public void StartMask(float maskTimer = 3f)
        {
            IsMaskActive = true;
            _collider.enabled = false;
            _maskTimer = maskTimer;
            _spriteRenderer.color = new Color(1f, 1f, 1f, 0.7f);
        }

        private void EndMask()
        {
            _collider.enabled = true;
            _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            IsMaskActive = false;
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

        public void AddAnotherChance()
        {
            _isHaveAnotherChance = true;
        }
    }
}