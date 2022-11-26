using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TandC.RunIfYouWantToLive 
{
    public partial class Enemy
    {
        public class WeaponAction : IEnemyAction
        {
            public event Action<Vector2, Vector2, float, EnemyBulletData> OnShootEventHandler;
            private EnemyWeapon _weapon;
            private Enemy _enemy;
            

            public void Init(Enemy enemy)
            {
                _enemy = enemy;
                _weapon = new EnemyWeapon(_enemy._playerTransform);
                GameObject weaponPoint = _enemy.SelfObject.transform.Find("LongRangeWeapon").gameObject;
                _weapon.Init(weaponPoint, GameClient.Get<IGameplayManager>().GameplayData.GetEnemyWeaponByType(_enemy.EnemyType), GameClient.Get<IGameplayManager>().GameplayData.GetEnemyBulletByType(_enemy.EnemyType), 0, weaponPoint.transform.Find("ShootDetector").gameObject);
                _weapon.ActiveWeapon();
                _weapon.OnShootEventHandler += OnEnemyShootHandler;
            }

            private void OnEnemyShootHandler(Vector2 shotPosition, Vector2 direction, float damage, int dropChance, BulletData bulletData, int bulletLife = 1)
            {
                OnShootEventHandler?.Invoke(shotPosition, direction, damage, (EnemyBulletData)bulletData);
            }

            public void Action()
            {
                _weapon.CanShoot();
            }

            public void Update() 
            {
                _weapon.Update();
                _enemy.Action();
            }
        }
    }

}

