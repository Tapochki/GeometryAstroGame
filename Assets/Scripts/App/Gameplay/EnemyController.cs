using System.Collections.Generic;
using UnityEngine;
using TandC.RunIfYouWantToLive.Common;
using System;
using static TandC.RunIfYouWantToLive.Enemy;

namespace TandC.RunIfYouWantToLive
{
    public class EnemyController : IController
    {
        private IGameplayManager _gameplayManager;
        private IUIManager _UIManager;
        private VFXController _vfxController;
        private PlayerController _playerController;
        public int ScoreCount => _scoreCount;
        private int _scoreCount;
        public Action<int> ScoreUpdateEvent;

        private List<Enemy> _enemies;

        private float _cooldownToSpawnEnemy;
        private GameplayData _gameplayData;
        private Transform _parent;
        private ObjectsController _objectController;
        private List<EnemySpawnData> _enemysInPhase;
        // private float _phaseTimer;
        private Phase _currentPhase;

        public int CurrentPhaseIndex { get; private set; }
        public int DashDamage;

        private float _increaseEnemyParam;

        private float _camHeight;
        private float _camWidth;

        private Dictionary<Enumerators.SpawnType, Transform> _spawnPositions;
        private List<Enumerators.SpawnType> _spawnsType;

        private List<EnemyBullet> _enemyBullets;
        public List<Enemy> ClosetEnemyObjects;
        public GameObject ClosetRadiusObject;

        public EnemyController()
        {

        }

        public void Dispose()
        {
        }
        public bool HitEnemy(GameObject enemyObject, float damage, int dropChace)
        {
            Enemy enemy = null;
            foreach (Enemy enemyEnemy in _enemies)
            {
                if (enemyObject == enemyEnemy.SelfObject)
                {
                    enemy = enemyEnemy;
                    break;
                }
            }
            if (enemy == null)
            {
                return false;
            }
            enemy.DeathEvent += OnEnemyDeath;
            enemy.DropChance = dropChace;
            enemy.TakeDamage(DamageHandler(damage * _playerController.DamageMultiplier, enemy.SelfObject));
            enemy.DeathEvent -= OnEnemyDeath;
            return true;
        }

        private void EnemyBehaviorHandler(GameObject gameObject, Enemy enemy)
        {
            if (gameObject == _playerController.Player.ModelObject || gameObject == _playerController.Player.SelfObject)
            {
                if (_playerController.Player.IsMaskActive) 
                {
                    return;
                }
                if (_playerController.Player.IsDash)
                {
                    enemy.DeathEvent += OnEnemyDeath;
                    enemy.DropChance = _gameplayData.DropChance.DashChance;
                    enemy.TakeDamage(DamageHandler(_playerController.DashDamage, enemy.SelfObject));
                    enemy.DeathEvent -= OnEnemyDeath;
                    return;
                }
                if(enemy.EnemyType == Enumerators.EnemyType.Impulse || enemy.EnemyType == Enumerators.EnemyType.ImpulseSaw) 
                {
                    _playerController.ImpulsePlayer(enemy.Damage);
                    LostClosetEnemy(enemy);
                    _enemies.Remove(enemy);
                    enemy.Destroy();
                    return;
                }
                _playerController.Player.TakeDamage(enemy.Damage);
                _vfxController.SpawnHitPlayerParticles(enemy.EnemyTransform.position, _playerController.Player.ModelObject.transform.position);
                if (!enemy.IsBoss)
                {
                    LostClosetEnemy(enemy);
                    _enemies.Remove(enemy);
                    enemy.Destroy();
                }
                return;
            }
            if(gameObject == ClosetRadiusObject) 
            {
                RegisterClosetEnemy(enemy);
            }
        }
        private float DamageHandler(float damage, GameObject enemy)
        {
            int chance = UnityEngine.Random.Range(0, 101);
            bool isCriticalChance = _playerController.CriticalChanceProcent >= chance;
            if (isCriticalChance)
            {
                damage *= _playerController.CriticalDamageMultiplier;
                _vfxController.SpawnDamagePointVFX(enemy.transform.position, damage, Color.red);
            }
            else
            {
                _vfxController.SpawnDamagePointVFX(enemy.transform.position, damage, Color.yellow);
            }
            return damage;
        }
        private void OnEnemyDeath(Enemy enemy)
        {
            if (enemy.EnemyType == Enumerators.EnemyType.PiciesFull)
            {
                float size = enemy.EnemyTransform.localScale.x;
                var firstHalf = SpawnEnemy(_gameplayData.GetEnemiesByType(Enumerators.EnemyType.PiciesHalf), Enumerators.SpawnType.EnemySpawnPosition_0, false);
                firstHalf.EnemyTransform.position = new Vector2(enemy.EnemyTransform.position.x - 6 * size, enemy.EnemyTransform.position.y);
                firstHalf.EnemyTransform.localScale = new Vector2(firstHalf.EnemyTransform.localScale.x * -1, firstHalf.EnemyTransform.localScale.y);
                var secondHalf = SpawnEnemy(_gameplayData.GetEnemiesByType(Enumerators.EnemyType.PiciesHalf), Enumerators.SpawnType.EnemySpawnPosition_0, false);
                secondHalf.EnemyTransform.position = new Vector2(enemy.EnemyTransform.position.x + 6 * size, enemy.EnemyTransform.position.y);
                _enemies.Add(firstHalf);
                _enemies.Add(secondHalf);
            }
            if (enemy.EnemyType == Enumerators.EnemyType.PiciesHalf)
            {
                float size = enemy.EnemyTransform.localScale.x;
                var firstHalf = SpawnEnemy(_gameplayData.GetEnemiesByType(Enumerators.EnemyType.PiciesSmall), Enumerators.SpawnType.EnemySpawnPosition_0, false);
                firstHalf.EnemyTransform.position = new Vector2(enemy.EnemyTransform.position.x, enemy.EnemyTransform.position.y + 6 * size);
                firstHalf.EnemyTransform.localScale = new Vector2(size, size);
                var secondHalf = SpawnEnemy(_gameplayData.GetEnemiesByType(Enumerators.EnemyType.PiciesSmall), Enumerators.SpawnType.EnemySpawnPosition_0, false);
                secondHalf.EnemyTransform.position = new Vector2(enemy.EnemyTransform.position.x, enemy.EnemyTransform.position.y - 6 * size);
                secondHalf.EnemyTransform.localScale = new Vector2(size, size * -1);
                _enemies.Add(firstHalf);
                _enemies.Add(secondHalf);
            }

            LostClosetEnemy(enemy);
            _vfxController.SpawnDeathParticles(enemy.EnemyTransform.position);
            _objectController.OnEnemyDeath(enemy);
            _scoreCount += enemy.GainedExperience;
            ScoreUpdateEvent?.Invoke(_scoreCount);
            enemy.Destroy();
            _enemies.Remove(enemy);
        }

        public void FrozeAllEnemy()
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                var enemy = _enemies[i];
                _vfxController.SpawnFrozeEnemyVFX(enemy.SelfObject.transform.position, enemy.SelfObject.transform);
                enemy.FrozeEnemy();
            }
        }

        public void Init()
        {
            _gameplayManager = GameClient.Get<IGameplayManager>();
            _vfxController = _gameplayManager.GetController<VFXController>();
            _playerController = _gameplayManager.GetController<PlayerController>();
            _objectController = _gameplayManager.GetController<ObjectsController>();
            _gameplayManager.GameplayStartedEvent += GameplayStartedEventHandler;

        }

        private void EnemyWeaponShoot(Vector2 shotPosition, Vector2 direction, float damage, EnemyBulletData bulletdata)
        {
            if (bulletdata != null)
            {
                EnemyBullet bullet = new EnemyBullet(_objectController.BulletContainer, bulletdata, direction, damage, shotPosition);
                bullet.OnColliderEvent += BulletInvoked;

                _enemyBullets.Add(bullet);

            };
        }

        private void BulletInvoked(EnemyBullet bullet, GameObject collider) 
        {
            if(collider == _playerController.Player.SelfObject) 
            {
                _playerController.Player.TakeDamage((int)bullet.Damage);
                _vfxController.SpawnHitPlayerParticles(bullet.SelfObject.transform.position, _playerController.Player.SelfObject.transform.position);
                bullet.Dispose();
            }
        }

        private IEnemyMove BuildMove(Enumerators.EnemyMovementType type, Enumerators.SpawnType spawnType) 
        {
            switch (type)
            {
                case Enumerators.EnemyMovementType.DefaultMove:
                    return new DefaultMove();
                case Enumerators.EnemyMovementType.SawMove:
                    return new RotateSawMove();
                case Enumerators.EnemyMovementType.MoveInPoint:
                    return new MoveInPoint(PositionToMove(spawnType));
                case Enumerators.EnemyMovementType.DistanceMove:
                    return new MovingOnDistanceMove();
                default:
                    return new DefaultMove();
            }
        }

        private Transform PositionToMove(Enumerators.SpawnType type) 
        {
            Transform spawnPosition = null;

            switch (type) 
            {
                case Enumerators.SpawnType.SpawnFrontPlayer:
                    spawnPosition = _playerController.Player.SelfObject.transform;
                    break;
                case Enumerators.SpawnType.EnemySpawnPosition_0:
                    spawnPosition = _spawnPositions[Enumerators.SpawnType.EnemySpawnPosition_10];
                    break;
                case Enumerators.SpawnType.EnemySpawnPosition_1:
                    spawnPosition = _spawnPositions[Enumerators.SpawnType.EnemySpawnPosition_9];
                    break;
                case Enumerators.SpawnType.EnemySpawnPosition_2:
                    spawnPosition = _spawnPositions[Enumerators.SpawnType.EnemySpawnPosition_8];
                    break;
                case Enumerators.SpawnType.EnemySpawnPosition_3:
                    spawnPosition = _spawnPositions[Enumerators.SpawnType.EnemySpawnPosition_7];
                    break;
                case Enumerators.SpawnType.EnemySpawnPosition_4:
                    spawnPosition = _spawnPositions[Enumerators.SpawnType.EnemySpawnPosition_6];
                    break;
                case Enumerators.SpawnType.EnemySpawnPosition_5:
                    spawnPosition = _spawnPositions[Enumerators.SpawnType.EnemySpawnPosition_11];
                    break;
                case Enumerators.SpawnType.EnemySpawnPosition_6:
                    spawnPosition = _spawnPositions[Enumerators.SpawnType.EnemySpawnPosition_4];
                    break;
                case Enumerators.SpawnType.EnemySpawnPosition_7:
                    spawnPosition = _spawnPositions[Enumerators.SpawnType.EnemySpawnPosition_3];
                    break;
                case Enumerators.SpawnType.EnemySpawnPosition_8:
                    spawnPosition = _spawnPositions[Enumerators.SpawnType.EnemySpawnPosition_2];
                    break;
                case Enumerators.SpawnType.EnemySpawnPosition_9:
                    spawnPosition = _spawnPositions[Enumerators.SpawnType.EnemySpawnPosition_1];
                    break;
                case Enumerators.SpawnType.EnemySpawnPosition_10:
                    spawnPosition = _spawnPositions[Enumerators.SpawnType.EnemySpawnPosition_0];
                    break;
                case Enumerators.SpawnType.EnemySpawnPosition_11:
                    spawnPosition = _spawnPositions[Enumerators.SpawnType.EnemySpawnPosition_5];
                    break;
                default:
                                        spawnPosition = _playerController.Player.SelfObject.transform;
                    break;
            }

            return spawnPosition;
        }


        private IEnemyAction BuildAction(Enumerators.EnemyActionType type)
        {
            switch (type)
            {
                case Enumerators.EnemyActionType.DefaultAction:
                    return new DefaultAction();
                case Enumerators.EnemyActionType.WeaponAction:
                    var actionPart = new WeaponAction();
                    actionPart.OnShootEventHandler += EnemyWeaponShoot;
                    return actionPart;
                default:
                    return new DefaultAction();
            }
        }

        private Enemy SpawnEnemy(Enemies data, Enumerators.SpawnType spawnType, bool isBoss)
        {
            Enemy enemy;
            enemy = new Enemy(_parent, data, _playerController.Player.SelfTransform, SetSpawnPosition(spawnType), 
                _gameplayManager.GameplayData.GetMaterialByType(Enumerators.MaterialTypes.DefaultEnemyMaterial).material,
                _gameplayManager.GameplayData.GetMaterialByType(Enumerators.MaterialTypes.FlashEnemyMaterial).material, isBoss);
            IEnemyAction actionPart = BuildAction(data.actionType);
            IEnemyMove movePart = BuildMove(data.movementType, spawnType);

            enemy.BuildParts(movePart, actionPart);
            enemy.OnExitCollderEvent += LostClosetEnemy;
            enemy.IncreaseParam(_increaseEnemyParam);
            enemy.OnCollderEvent += EnemyBehaviorHandler;
            enemy.OnEndEnemyLifeTime += EnemyLifeTimeEnd;
            enemy.DestroyEvent += EnemyDestroyedEventHandler;
            return enemy;

        }

        public void StopStartEnemy(bool value) 
        {
            foreach(var enemy in _enemies) 
            {
                enemy.IsCanMove = value;
            }
        }

        private void EnemyLifeTimeEnd(Enemy enemy)
        {
            if (Vector2.Distance(enemy.SelfObject.transform.position, _playerController.Player.SelfTransform.position) >= 1500f)
            {
                enemy.IsEndLifeTime = true;
            }
            else
            {
                enemy.AddLifeTime(10f);
            }
        }

        public void StartSpawnEnemy()
        {
            if (_enemies.Count >= 200)
            {
                return;
            }
            int enemyInPhaseId = 0;
            if (_currentPhase.IsRandomEnemySpawn) 
            {
                enemyInPhaseId = UnityEngine.Random.Range(0, _enemysInPhase.Count);
            }

            EnemySpawnData enemySpawnData = _enemysInPhase[enemyInPhaseId];
            _enemysInPhase.Remove(enemySpawnData);
            switch (enemySpawnData.spawnType) 
            {
                case Enumerators.SpawnType.Circle:
                    SpecialSpawn(0, _spawnsType.Count-1, enemySpawnData.enemyType);
                    break;
                case Enumerators.SpawnType.UpperPosition:
                    SpecialSpawn(0, 4, enemySpawnData.enemyType);
                    break;
                case Enumerators.SpawnType.LeftPosition:
                    SpecialSpawn(4, 6, enemySpawnData.enemyType);
                    break;
                case Enumerators.SpawnType.DownPosition:
                    SpecialSpawn(6, 10, enemySpawnData.enemyType);
                    break;
                case Enumerators.SpawnType.RightPosition:
                    _enemies.Add(SpawnEnemy(_gameplayData.GetEnemiesByType(enemySpawnData.enemyType), Enumerators.SpawnType.EnemySpawnPosition_0, false));
                    SpecialSpawn(10, _spawnsType.Count-1, enemySpawnData.enemyType);
                    break;
                default:
                    _enemies.Add(SpawnEnemy(_gameplayData.GetEnemiesByType(enemySpawnData.enemyType), enemySpawnData.spawnType, false));
                    break;
            }

        }

        private void SpecialSpawn(int spawnPointCountStart, int spawnPointCountEnd, Enumerators.EnemyType enemyType) 
        {
            for (int i = spawnPointCountStart; i <= spawnPointCountEnd; i++)
            {
                _enemies.Add(SpawnEnemy(_gameplayData.GetEnemiesByType(enemyType), _spawnsType[i], false));
            }
        }

        private void SetNewPhase(int phaseId)
        {
            if (CurrentPhaseIndex >= _gameplayData.gamePhases.Length)
            {
                IncreasePhaseIndex();
            }
            var miniBoss = _gameplayData.GetMiniBossByPhaseId(phaseId);
            if (miniBoss != null)
            {
                _enemies.Add(SpawnEnemy(miniBoss.enemyData, Enumerators.SpawnType.Random, true));
            }
            CurrentPhaseIndex = phaseId;
            _currentPhase = _gameplayData.GetPhaseById(phaseId);
            _enemysInPhase = new List<EnemySpawnData>();
            if (_currentPhase != null)
            {
                for (int i = 0; i < _currentPhase.enemyInPhase.Length; i++)
                {
                    for (int j = 0; j < _currentPhase.enemyInPhase[i].EnemyCount; j++)
                    {
                        _enemysInPhase.Add(_currentPhase.enemyInPhase[i]);
                    }
                }
                _cooldownToSpawnEnemy = _currentPhase.timeBeforePhase;
            }
        }

        private Vector2 SetSpawnPosition(Enumerators.SpawnType spawnType)
        {
            Vector2 position;        

            if (spawnType == Enumerators.SpawnType.Random)
            {
                int index = UnityEngine.Random.Range(0, _spawnsType.Count);

                spawnType = _spawnsType[index];
            }
            Transform spawnPoint = _spawnPositions[spawnType];

            if (spawnType == Enumerators.SpawnType.SpawnFrontPlayer)
            {
                spawnPoint.transform.localPosition = new Vector2(0, spawnPoint.transform.localPosition.y);
                int range = UnityEngine.Random.Range(-200, 200);
                spawnPoint.transform.localPosition = new Vector2(spawnPoint.transform.localPosition.x + range, spawnPoint.transform.localPosition.y);
                position = new Vector2(spawnPoint.transform.position.x, spawnPoint.transform.position.y);
                return position;
            }
            position = spawnPoint.position;
            if (spawnPoint.localPosition.x == 0 || (spawnPoint.localPosition.x < _camWidth/2 && spawnPoint.position.x > (_camWidth/2)*-1) )
            {
                int range = UnityEngine.Random.Range(-100, 101);
                position = new Vector2(spawnPoint.position.x + range, spawnPoint.position.y);
            }
            if(spawnPoint.localPosition.y == 100 || spawnPoint.localPosition.y == -100) 
            {
                int range = UnityEngine.Random.Range(-100, 101);
                position = new Vector2(spawnPoint.position.x, spawnPoint.position.y + range);
            }
            //else 
            //{
            //    int range = UnityEngine.Random.Range(-100, 101);
            //    position = new Vector2(spawnPoint.position.x + range, spawnPoint.position.y + range);
            //}

            return position;
        }

        public void EnemyDestroyedEventHandler(Enemy enemy)
        {
            _enemies.Remove(enemy);
        }

        private void GameplayStartedEventHandler()
        {

            float screenAspect = (float)Screen.width / (float)Screen.height;
            _camHeight = _gameplayManager.GameplayCamera.orthographicSize;
            _camWidth = screenAspect * _camHeight;
            _parent = _gameplayManager.GameplayObject.transform.Find("[Enemy]");
            _enemies = new List<Enemy>();
            _gameplayData = _gameplayManager.GameplayData;
            _scoreCount = 0;
            _enemyBullets = new List<EnemyBullet>();
            ClosetEnemyObjects = new List<Enemy>();
            _spawnPositions = new Dictionary<Enumerators.SpawnType, Transform>();
            _spawnsType = new List<Enumerators.SpawnType>();
            foreach (var spawnType in Enum.GetValues(typeof(Enumerators.SpawnType)))
            {
                Transform spawnPosition = _gameplayManager.GameplayCamera.transform.Find(spawnType.ToString());
                if (spawnPosition != null)
                {
                    _spawnPositions.Add((Enumerators.SpawnType)spawnType, spawnPosition);
                    _spawnsType.Add((Enumerators.SpawnType)spawnType);
                }
            }
            ClosetRadiusObject = _playerController.Player.SelfObject.transform.Find("Body/AimRadius").gameObject;
            ClosetRadiusObject.SetActive(false);
            _spawnPositions.Add(Enumerators.SpawnType.SpawnFrontPlayer, _playerController.Player.SelfObject.transform.Find("EnemySpawnPosition"));
            _spawnsType.Add(Enumerators.SpawnType.SpawnFrontPlayer);
            CurrentPhaseIndex = 0;
            _increaseEnemyParam = 1f;
            SetNewPhase(CurrentPhaseIndex);
            ScoreUpdateEvent?.Invoke(_scoreCount);
        }
        private void RegisterClosetEnemy(Enemy enemy) 
        {
            ClosetEnemyObjects.Add(enemy);
        }
        private void LostClosetEnemy(Enemy enemy) 
        {
            for(int i = 0; i < ClosetEnemyObjects.Count; i++) 
            {
                if(ClosetEnemyObjects[i] == enemy) 
                {
                    enemy.IsTargetAlready = false;
                    ClosetEnemyObjects.Remove(ClosetEnemyObjects[i]);
                }
            }
        }
        public GameObject GetClosetEnemy(Transform weaponTransform) 
        {
            Enemy closetEnemy = ClosetEnemyObjects[0];
            for(int i = 0; i < ClosetEnemyObjects.Count; i++) 
            {
                if(ClosetEnemyObjects[i] != null) 
                {
                    closetEnemy = ClosetEnemyObjects[i];
                    break;
                }
            }
            for (int i = 0; i <= ClosetEnemyObjects.Count - 1; i++)
            {
                if (closetEnemy == null)
                {
                    if (i < ClosetEnemyObjects.Count - 1) 
                    {
                        closetEnemy = ClosetEnemyObjects[i+1];
                    }
                    continue;
                }
                float closetEnemyDistance =
                       Vector2.Distance(closetEnemy.SelfObject.transform.position, weaponTransform.position);
                var anotherEnemy = ClosetEnemyObjects[i];
                if(anotherEnemy == null) 
                {
                    if(closetEnemy != null) 
                    {
                        anotherEnemy = closetEnemy;
                    }
                    else 
                    {
                        anotherEnemy = ClosetEnemyObjects[ClosetEnemyObjects.Count - 1];
                    }
                }
                var enemyPlayerDistance = Vector2.Distance(anotherEnemy.SelfObject.transform.position,
                       weaponTransform.position);
                if (enemyPlayerDistance <= closetEnemyDistance)
                {
                    closetEnemy = ClosetEnemyObjects[i];
                }
            }
            if(closetEnemy == null) 
            {
                for (int i = 0; i < ClosetEnemyObjects.Count; i++)
                {
                    if (ClosetEnemyObjects[i] != null)
                    {
                        closetEnemy = ClosetEnemyObjects[i];
                        break;
                    }
                }
            }
            return closetEnemy.SelfObject;
        }
        public Enemy GetClosetEnemyForEnergy(Transform weaponTransform)
        {
            Enemy closetEnemy = ClosetEnemyObjects[0];
            if (closetEnemy == null)
            {
                for (int i = 0; i < ClosetEnemyObjects.Count; i++)
                {
                    if (ClosetEnemyObjects[i] != null && ClosetEnemyObjects[i].IsTargetAlready && ClosetEnemyObjects[i].SelfObject != null)
                    {
                        closetEnemy = ClosetEnemyObjects[i];
                        break;
                    }
                }
            }
            try 
            {
                float closetEnemyDistance = Vector2.Distance(closetEnemy.SelfObject.transform.position, weaponTransform.position);
                for (int i = 0; i < ClosetEnemyObjects.Count; i++)
                {
                    var anotherEnemy = ClosetEnemyObjects[i];
                    if (anotherEnemy == null && anotherEnemy.IsTargetAlready && anotherEnemy.SelfObject == null)
                    {
                        continue;
                    }
                    var enemyPlayerDistance = Vector2.Distance(anotherEnemy.SelfObject.transform.position,
                           weaponTransform.position);
                    if (enemyPlayerDistance <= closetEnemyDistance)
                    {
                        closetEnemy = ClosetEnemyObjects[i];
                        closetEnemyDistance = Vector2.Distance(closetEnemy.SelfObject.transform.position, weaponTransform.position);
                    }
                }
            }
            catch (MissingReferenceException ex) 
            {
                ClosetEnemyObjects.Clear();
                ClosetRadiusObject.gameObject.SetActive(false);
                ClosetRadiusObject.gameObject.SetActive(true);
                closetEnemy = _enemies[0];
            }
           

            return closetEnemy;
        }

        public void IncreasePhaseIndex() 
        {
            CurrentPhaseIndex++;
            if(CurrentPhaseIndex >= _gameplayData.gamePhases.Length - 1) 
            {
                _increaseEnemyParam += 0.25f;
                CurrentPhaseIndex = 0;
            }
            SetNewPhase(CurrentPhaseIndex);
        }

        public void ResetAll()
        {
            _enemies.Clear();
        }

        public void Update()
        {
            if (!_gameplayManager.IsGameplayStarted)
                return;
            //_phaseTimer -= Time.deltaTime;
            //if(_phaseTimer <= 0) 
            //{
            //    IncreasePhaseIndex();
            //}
            _cooldownToSpawnEnemy -= Time.deltaTime;
            if (_cooldownToSpawnEnemy <= 0)
            {
                _cooldownToSpawnEnemy = _currentPhase.spawnTime;
                StartSpawnEnemy();
                if(_enemysInPhase.Count <= 0) 
                {
                    // Debug.Log("Enemis count <= 0");
                    IncreasePhaseIndex();
                }
            }
            
            //foreach (var item in _enemies) 
            //{
            //    item.Update();
            //}
            for(int i = 0; i < _enemies.Count; i++) 
            {
                var enemy = _enemies[i];
                if(enemy.IsEndLifeTime) 
                {
                    LostClosetEnemy(enemy);
                    enemy.Destroy();
                    _enemies.Remove(enemy);
                }
                enemy.Update();
            }
                
        }

        public void FixedUpdate()
        {
            if (!_gameplayManager.IsGameplayStarted)
                return;

            for (int i = 0; i < _enemyBullets.Count; i++)
            {
                EnemyBullet bullet = _enemyBullets[i];
                if (!bullet.IsLife)
                {
                    bullet.Dispose();
                    _enemyBullets.Remove(bullet);
                }
                bullet.Update();
            }

            if (_enemies.Count > 0)
                foreach (var item in _enemies)
                    item.FixedUpdate();
        }
    }
}