using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Helpers;
using UnityEngine;
using static TandC.RunIfYouWantToLive.Common.Enumerators;

namespace TandC.RunIfYouWantToLive
{
    public class ObjectsController : IController
    {
        private IGameplayManager _gameplayManager;
        private IUIManager _UIManager;
        public Action<bool> OnPlayerInBorderHandler;
        private VFXController _vfxController;
        private PlayerController _playerController;
        private EnemyController _enemyController;
        private SkillsController _skillsController;
        private GameplayData _gameplayData;
        public Transform BulletContainer { get; private set; }
        private Transform _itemContainer;
        private List<PlayerBullet> _bulletList = new List<PlayerBullet>();
        private List<PlayerBullet> _invokedBulletsList = new List<PlayerBullet>();
        private List<Item> _items;
        //private GamePage _gamePage;

        public List<PlayerBullet> InvokedBulletsList
        {
            get { return _invokedBulletsList; }
        }
        private const float _playerLeaveBorderTime = 5f;
        private float _playerLeaveBorderTimer;
        private bool _isPlayerInBorder;
        public GameObject LaserShot;
        private float _rocketBlowSize;
        private RandomDrop _randomDrop;

        public ObjectsController()
        {

        }

        public void Dispose()
        {
          
        }

        public void UpgradeRocketBlowSize(float value) 
        {
            _rocketBlowSize += value;
        }

        private void BulletInvoked(PlayerBullet bullet, GameObject collider) 
        {
            bool takeShot = _enemyController.HitEnemy(collider, bullet.Damage, bullet.DropChance);
            if (takeShot) 
            {
               
                if (bullet.BulletType == WeaponType.RocketLauncer) 
                {
                    var _rocketBlow = new Blow(_vfxController.SpawnRocketBlow(bullet.SelfObject.transform.position, _rocketBlowSize), _playerController.RocketBlowDamage, _gameplayData.DropChance.RocketBlowChance);
                    _rocketBlow.OnGetEnemy += HitBlowEnemy;
                }
                else 
                {
                    _vfxController.SpawnHitParticles(bullet.SelfObject.transform.position, bullet.SelfObject.transform.eulerAngles);
                }
                if (bullet.BulletType == WeaponType.LaserGun) 
                {
                    return;
                }
                bullet.Dispose();
            }
        }
        private void HitBlowEnemy(GameObject collider, float damage, int dropChance) 
        {
            _enemyController.HitEnemy(collider, damage, dropChance);   
        }
        public void Init()
        {
            _gameplayManager = GameClient.Get<IGameplayManager>();
            _UIManager = GameClient.Get<IUIManager>();
            _vfxController = _gameplayManager.GetController<VFXController>();
            _gameplayManager.GameplayStartedEvent += GameplayStartedEventHandler;
        }

        private void GameplayStartedEventHandler()
        {
            Transform parent = _gameplayManager.GameplayObject.transform.Find("[Objects]");
            _playerController = _gameplayManager.GetController<PlayerController>();
            _enemyController = _gameplayManager.GetController<EnemyController>();
            _skillsController = _gameplayManager.GetController<SkillsController>();
            BulletContainer = parent.Find("[Bullets]");
            _itemContainer = parent.Find("[Items]");
            _items = new List<Item>();
            _bulletList = new List<PlayerBullet>();
            _invokedBulletsList = new List<PlayerBullet>();
            _gameplayData = _gameplayManager.GameplayData;
            _randomDrop = new RandomDrop(_gameplayData);
            _isPlayerInBorder = true;
            var border = new Border(parent.transform.Find("Border").gameObject, _playerController.Player.ModelObject);
            border.PlayerBackGameLocationEvent += BackToBorderHandler;
            border.PlayerLeaveBorderEvent += PlayerLeaveBorderHandler;
            //_starsParticle = parent.transform.Find("Particle_Star").gameObject;
            _rocketBlowSize = _gameplayManager.GameplayData.playerData.StartRocketBlowSize;
            //_gamePage = GameClient.Get<IUIManager>().GetPage<GamePage>() as GamePage;

        }
        private void OnItemDestory(Item item) 
        {
            switch (item.ItemType)
            {
                case ItemType.SmallXp:
                case ItemType.BigXp:
                case ItemType.MeduimXp:
                    _playerController.AddXpToPlayer(item.ItemValue);
                    break;
                case ItemType.Medecine:
                    _playerController.RestoreHelathPlayer(item.ItemValue);
                    break;
                case ItemType.SmallMoney:
                    _playerController.EarnMoney(item.ItemValue);
                    break;
                case ItemType.FrozenBomb:
                    FreezeEnemies(item.SelfObject.transform.position);
                    break;
                case ItemType.Bomb:
                    BlowupAllEnemies(item.SelfObject.transform.position);
                    break;
                case ItemType.Magnet:
                    foreach (var dropItem in _items)
                    {
                        if (dropItem.ItemType == ItemType.SmallXp || dropItem.ItemType == ItemType.MeduimXp || dropItem.ItemType == ItemType.BigXp || dropItem.ItemType == ItemType.SmallMoney)
                        dropItem.StartMoving();
                    }
                    break;
                case ItemType.Chest:
                    GameClient.Get<IUIManager>().DrawPopup<ChestPopup>(_skillsController.FillUpgradeList(_randomDrop.GetChestDrop(), true));
                    //_chestMarker.isChestActive = false;
                    //_gamePage.SetMarkerActive(false);
                    break;
                case ItemType.RocketBox:
                    if (!_playerController.OnGetRocketBox()) 
                    {
                        return;
                    }
                    break;
            }
            item.Dispose();
            _items.Remove(item);
        }
        //public bool IsLaserShot(GameObject gameObject, out float damage) 
        //{
        //    damage = 0;
        //    if(LaserShot != null) 
        //    {
        //        if(gameObject == LaserShot) 
        //        {
        //            damage = _laserGunDamage;
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public void FreezeEnemies(Vector2 position)
        {
            _vfxController.SpawnFrozeBombVFX(position);
            _enemyController.FrozeAllEnemy();
        }

        public void BlowupAllEnemies(Vector2 position)
        {
            var bombBlow = new Blow(_vfxController.SpawnBombBlow(position), _playerController.BombDamage, _gameplayData.DropChance.BombBlowChance);
            bombBlow.OnGetEnemy += HitBlowEnemy;
        }

        public void OnEnemyDeath(Enemy enemy) 
        {
            ItemData itemData = GetRandomItem(enemy.DropChance,enemy.IsBoss);
            if(itemData == null) 
            {
                return;
            }
            int itemValue = InternalTools.GetRandomNumberInteger(itemData.itemValueMin, itemData.itemValueMax);
            Item item = new Item(itemData.prefab, _itemContainer, enemy.EnemyTransform.position, itemData.type, itemValue);

            //if (item.ItemType == ItemType.Chest)
            //{
            //    _chestMarker = new ChestMarker(item.SelfObject.transform.position, _UIManager.UICamera, _playerController, _gamePage.GetMarkerObject());
            //    _chestMarker.isChestActive = true;
            //    _gamePage.SetMarkerActive(true);
            //}

            item.ItemDestroyHandler += OnItemDestory;
            _items.Add(item);
        }



        private ItemData GetRandomItem(int dropChance, bool isBoss = false) 
        {
            ItemData itemData;
            if (isBoss)
            {
                if (_skillsController.ActiveSkills.Count <= 0 && _skillsController.PassiveSkills.Count <= 0)
                {
                    itemData = _gameplayData.GetItemDataByType(ItemType.BigXp);
                    return itemData;
                }
                itemData = _randomDrop.GetBossDrop();
                return itemData;
            }
            var chance = InternalTools.GetRandomNumberInteger(0, 100);
            if (chance <= dropChance)
            {
                itemData = _randomDrop.GetDrop();
                if ((itemData.type == ItemType.RocketBox && !_playerController.GetWeaponByType(WeaponType.RocketLauncer).IsActive))
                {
                    itemData = _gameplayData.GetItemDataByType(ItemType.SmallXp);
                }
                return itemData;
            }
            else 
            {
                return null;
            }
        }

        public void WeaponShoot(PlayerBulletData bulletdata, Vector2 shotPosition, Vector2 direction, float damage, int dropChance, int bulletLife = 1) 
        {
            if(bulletdata != null) 
            {
                PlayerBullet bullet = new PlayerBullet(BulletContainer, bulletdata, direction, damage, dropChance, shotPosition, bulletLife);
                bullet.OnColliderEvent += BulletInvoked;
                if (bulletdata.type == Common.Enumerators.WeaponType.LaserGun)
                {
                    LaserShot = bullet.SelfObject;
                }

                _bulletList.Add(bullet);

            };
        }

        private void PlayerLeaveBorderHandler() 
        {
            if (_playerController.Player.IsMaskActive || !_playerController.Player.IsAlive) 
            {
                return;
            }
            OnPlayerInBorderHandler?.Invoke(true);
            _playerLeaveBorderTimer = _playerLeaveBorderTime;
            _isPlayerInBorder = false;
            _playerController.BackPlayerToCenterStart();
        }
        private void BackToBorderHandler() 
        {
            OnPlayerInBorderHandler?.Invoke(false);
            ResetTimer();
        }
        private void ResetTimer() 
        {
            _isPlayerInBorder = true;
        }

        public void ResetAll()
        {
            _bulletList.Clear();
            _invokedBulletsList.Clear();
        }

        public void Update()
        {
            if (!_gameplayManager.IsGameplayStarted)
                return;
            if (!_isPlayerInBorder) 
            {
                _playerLeaveBorderTimer -= Time.deltaTime;
                if(_playerLeaveBorderTimer <= 0) 
                {
                    ResetTimer();
                    _playerController.BackPlayerToCenter(40);
                }
            }
            //_starsParticle.transform.position = _gameplayManager.GameplayCamera.transform.position * -1.75f;
            for(int i = 0; i < _bulletList.Count; i++) 
            {
                PlayerBullet bullet = _bulletList[i];
                if (!bullet.IsLife) 
                {
                    bullet.Dispose();
                    _bulletList.Remove(bullet);
                }
                bullet.Update();
            }
            foreach(var item in _items) 
            {
                item.Update();
            }
            //if (_chestMarker != null)
            //    _chestMarker.Update();
        }

        public void FixedUpdate()
        {
           
        }
    }

}
