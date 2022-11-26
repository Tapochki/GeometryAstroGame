using System;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Helpers;
using UnityEngine;


namespace TandC.RunIfYouWantToLive 
{
    public class AutoWeapon : Weapon
    {
        private EnemyController _enemyController;

        public AutoWeapon()
        {

        }

        public void UpgradeBulletLife(int value) 
        {
            _bulletLife += value;
        }

        protected override void RegisterNewWeapon()
        {
            _enemyController = GameClient.Get<IGameplayManager>().GetController<EnemyController>();
            _weaponTransform = _selfObject.transform;
        }

        private GameObject FindClosetEnemy()
        {
            GameObject enemy = _enemyController.GetClosetEnemy(_weaponTransform);
            
            if(enemy == null) 
            {
                Debug.LogError($"{enemy} {_enemyController.ClosetEnemyObjects.Count}");
                _shootTempDeley = 1f;
                _canShoot = false;
            }
            
            return enemy;
        }

        protected override void ShotGetReady()
        {
            if(_enemyController.ClosetEnemyObjects.Count <= 0) 
            {
                return;
            }
            GameObject closetEnemy = FindClosetEnemy();
            if(closetEnemy == null) 
            {
                return;
            }
            _weaponDirection = closetEnemy.transform;
            Shoot();
        }

        
    }
}

