using UnityEngine;
using TandC.RunIfYouWantToLive.Common;
using System;


namespace TandC.RunIfYouWantToLive
{
    public partial class Enemy
    {
        public event Action<Enemy> DestroyEvent;
        public event Action<Enemy> DeathEvent;
        public event Action<GameObject, Enemy> OnCollderEvent;
        public event Action<Enemy> OnExitCollderEvent;
        public event Action<Enemy> OnEndEnemyLifeTime;
        public GameObject SelfObject { get => _selfObject; }
        private GameObject _selfObject;
        private Animator _animator;
        protected Rigidbody2D _rigidbody2d;
        private Enemies _enemies;
        protected OnBehaviourHandler _onBehaviourHandler;
        private SpriteRenderer _enemySprite;

        private EnemyController _enemyController;

        protected float _lifeTimer;

        public int EnemyId { get; private set; }
        public float Health { get; private set; }
        public int Damage { get; private set; }
        public int GainedExperience { get; private set; }
        public float MovementSpeed { get; set; }
        public Enumerators.EnemyType EnemyType { get; set; }
        private bool _isAlive;
        protected float _attackCooldown = 1f;
        protected float _actualCooldown;
        public Rigidbody2D Rigidbody2D;
        public bool IsCanAction { get; set; }

        public Transform EnemyTransform { get; set; }

        private bool _isInitialize;
        protected Transform _playerTransform;

        private bool _isDamaged;
        private float _blinkTimer;
        private const float _blinkTime = 0.15f;

        public bool IsBoss = false;

        public bool IsCanMove;

        private float _frozenStartenTime = 5f;
        private float _frozenTimer;
        private bool _isFroze = false;

        public int DropChance;
        public bool IsEndLifeTime = false;


        private IEnemyMove _movePart;
        private IEnemyAction _actionPart;
        public bool IsTargetAlready;

        private Material _defaultMaterial,
                         _flashMaterial;

        public Enemy(Transform parent, Enemies data, Transform playerTransform, Vector2 position, Material @default, Material flash, bool isBoss = false, bool init = true)
        {
            _enemyController = GameClient.Get<IGameplayManager>().GetController<EnemyController>();
            if (init)
            {
                _enemies = data;

                EnemyId = _enemies.enemyId;
                MovementSpeed = _enemies.movementSpeed;
                Damage = _enemies.damage;
                _attackCooldown = 2f;
                _playerTransform = playerTransform;
                _selfObject = MonoBehaviour.Instantiate(_enemies.enemyPrefab, parent);
                _selfObject.transform.position = position;
                _selfObject.name = $"{_enemies.type}";
                _animator = _selfObject.GetComponent<Animator>();
                _rigidbody2d = _selfObject.GetComponent<Rigidbody2D>();
                this.Rigidbody2D = _rigidbody2d;
                GainedExperience = data.experience;
                EnemyTransform = _selfObject.transform;
                Health = data.health;
                _enemySprite = _selfObject.GetComponent<SpriteRenderer>();
                _isAlive = true;
                _isInitialize = true;
                IsCanAction = true;
                IsBoss = isBoss;
                EnemyType = data.type;
                _lifeTimer = data.lifeTime;
                _actualCooldown = 2f;
                IsCanMove = true;

                _defaultMaterial = @default;
                _flashMaterial = flash;

            }
            else
                _isInitialize = false;

            RegisterBehaviorHandler();
        }

        public void BuildParts(IEnemyMove movePart, IEnemyAction actionPart) 
        {
            _movePart = movePart;
            _movePart.Init(this);
            _actionPart = actionPart;
            _actionPart.Init(this);
        }

        protected virtual void RegisterBehaviorHandler() 
        {
            _onBehaviourHandler = _selfObject.GetComponent<OnBehaviourHandler>();
            
            _onBehaviourHandler.Trigger2DEntered += OnBehaviorHandler;
            _onBehaviourHandler.Trigger2DExited += OnColliderExit;
        }
        public void FrozeEnemy()
        {
            _rigidbody2d.constraints = RigidbodyConstraints2D.FreezeAll;
            _frozenTimer = _frozenStartenTime;
            _isFroze = true;
        }

        public void Action() 
        {
            if (_isFroze) 
            {
                return;
            }

            if (_movePart.CanAction) 
            {
                _actionPart.Action();
            }
        }

        public void AddLifeTime(float time)
        {
            _lifeTimer = time;
        }

        public void IncreaseParam(float mulriplier) 
        {
            Health += mulriplier;
            Damage = (int)(Damage * mulriplier);
        }

        public virtual void Update()
        {
            if (IsEndLifeTime) 
            {
                return;
            }
            if (!_isAlive)
                return;
            if (!_isInitialize)
                return;

            //if (!IsCanAction)
            //{
            //    _actualCooldown -= Time.deltaTime;
            //    if (_actualCooldown <= 0)
            //    {
            //        Action();
            //    }
            //}
           
            if (_isDamaged) 
            {
                _blinkTimer -= Time.deltaTime;
                if(_blinkTimer <= 0) 
                {
                    SetNormalColor();
                }
            }
            _lifeTimer -= Time.deltaTime;
            if (_lifeTimer <= 0)
            {
                OnEndEnemyLifeTime?.Invoke(this);
                return;
            }
            _actionPart.Update();
        }


        public void FixedUpdate()
        {
            if (IsEndLifeTime) 
                return;
            if (!_isAlive)
                return;
            if (!_isInitialize)
                return;
            if (!IsCanMove)
                return;
            if (_isFroze) 
            {
                _frozenTimer -= Time.deltaTime;
                if(_frozenTimer <= 0) 
                {
                    _rigidbody2d.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
                    _isFroze = false;
                }
                return;
            }
            _movePart.Update();
        }

        public void TakeDamage(float damage)
        {
            if (!_isAlive)
                return;
            if (!_isInitialize)
                return;
            Health -= damage;
            SetDamageColor();
            if (Health > 0) 
            {
                _blinkTimer = _blinkTime;
                _isDamaged = true;
            }
            if (Health <= 0) 
            {
                Dispose();
                DeathEvent?.Invoke(this);
            }
        }
        private void SetNormalColor() 
        {
            _enemySprite.material = _defaultMaterial;
        }
        private void SetDamageColor() 
        {
            _enemySprite.material = _flashMaterial;
            
        }

        protected void OnBehaviorHandler(GameObject collider) 
        {
            if(collider.tag == _selfObject.tag || collider.tag == "Border") 
            {
                return;
            }
            OnCollderEvent?.Invoke(collider.gameObject, this);
        }

        protected void OnColliderExit(GameObject collider) 
        {
            if (collider != _enemyController.ClosetRadiusObject)
            {
                return;
            }
            OnExitCollderEvent?.Invoke(this);
        }

        public void Dispose()
        {
            if (!_isAlive)
                return;
            if (!_isInitialize)
                return;

            _isAlive = false;

            Destroy();
        }

        protected void PlayAnimation(string name, int layer = -1, float normalizedTime = 0)
        {
            _animator.Play(name, layer, normalizedTime);
        }

        public void Destroy()
        {
            IsTargetAlready = true;
            _isInitialize = false;
            _isAlive = false;
            MonoBehaviour.Destroy(_selfObject);

        }
    }
}