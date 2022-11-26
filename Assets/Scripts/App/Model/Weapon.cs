using System;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public abstract class Weapon
    {
        public event Action<Vector2, Vector2, float, int, BulletData, int> OnShootEventHandler;
        public event Action OnReloadEndHandler;
        public event Action OnReloadStartHandler;
        protected GameObject _selfObject;
        protected float _baseDamage;
        protected float _shootDeley = 0f;
        protected float _shootTempDeley;

        protected Transform _shotPlace;
        protected bool _canShoot = true;
        public int BulletId;
        private Transform _shootDirection;
        protected Vector2 _direction => _shootDirection.position;
        public bool IsActive { get; private set; }
        public bool IsButtonClick;

        protected int _dropChance;

        protected BulletData _bulletData;

        protected Transform _weaponTransform;
        protected Transform _weaponDirection;

        public Enumerators.WeaponType WeaponType { get; private set; }
        private float _deleyShotTimeMultiplier = 1f;

        protected int _bulletLife = 1;

        private GameObject _shootDetectorObject;
        public Weapon()
        {

        }
        public void Init(GameObject selfObject, PlayerWeaponData weaponData, BulletData bulletData, int dropChance, GameObject detector, bool isButtonClick = false) 
        {
            _shootDetectorObject = detector;
            _selfObject = selfObject;
            _shotPlace = selfObject.transform;
            _baseDamage = weaponData.baseDamage;
            _shootDeley = weaponData.shootDeley;
            _bulletData = bulletData;
            WeaponType = weaponData.type;
            _bulletLife = 1;
            IsActive = false;
            _shootTempDeley = 1f;
            _dropChance = dropChance;
            IsButtonClick = isButtonClick;
            LocalInit();

            _selfObject.gameObject.SetActive(false);
        }
        public void Init(GameObject selfObject, EnemyWeaponData weaponData, BulletData bulletData, int dropChance, GameObject detector, bool isButtonClick = false)
        {
            _shootDetectorObject = detector;
            _selfObject = selfObject;
            _shotPlace = selfObject.transform;
            _baseDamage = weaponData.baseDamage;
            _shootDeley = weaponData.shootDeley;
            _bulletData = bulletData;
            _bulletLife = 1;
            IsActive = false;
            _shootTempDeley = 1f;
            _dropChance = dropChance;
            IsButtonClick = isButtonClick;
            LocalInit();

            _selfObject.gameObject.SetActive(false);
        }

        public void SetImpulseToWeapon(int damage) 
        {
            _canShoot = false;
            _shootTempDeley = (_shootDeley * _deleyShotTimeMultiplier) * damage;
        } 

        protected virtual void LocalInit() 
        {

        }
        public void ActiveWeapon()
        {
            RegisterNewWeapon();
            _selfObject.gameObject.SetActive(true);
            IsActive = true;

        }
        protected abstract void RegisterNewWeapon();

        public void SetRecoverTimeMultiplier(float value)
        {
            _deleyShotTimeMultiplier = value;
        }
        public void IncreaseBulletSpeed(int value)
        {
            _bulletData.BulletSpeed += value;
        }
        public void IncreaseBulletDamage(int value)
        {
            _baseDamage += value;
        }

        public virtual void Update()
        {
            if (IsActive) 
            {
                if (!_canShoot)
                {
                    if (_shootDetectorObject.activeInHierarchy && this is DefaultWeapon)
                        _shootDetectorObject.SetActive(false);
                    _shootTempDeley -= Time.deltaTime;
                    if (_shootTempDeley <= 0)
                    {
                        _canShoot = true;
                    }

                }
                else
                {
                    if (!_shootDetectorObject.activeInHierarchy && this is DefaultWeapon)
                        _shootDetectorObject.SetActive(true);
                }
            }
        }

        public void ClickShoot()
        {
            ShotGetReady();
        }

        public void CanShoot()
        {
            if (!_canShoot || IsButtonClick)
            {
                return;
            }
            ShotGetReady();
        }

        protected virtual void ShotGetReady()
        {
            Shoot(_weaponTransform.position, _weaponDirection.position);
        }

        protected virtual void Shoot(Vector2 weaponPosition, Vector2 direction, object[] data = null) 
        {
            _canShoot = false;
            _shootTempDeley = _shootDeley * _deleyShotTimeMultiplier;
            OnShootEventHandler?.Invoke(weaponPosition, direction, _baseDamage, _dropChance, _bulletData, _bulletLife);
        }

        protected virtual void Shoot(object[] data = null) 
        {
            Shoot(_weaponTransform.position, _weaponDirection.position);
        }

        public virtual void Dispose()
        {
            MonoBehaviour.Destroy(_selfObject);
        }

        public void ActiveWeapon(bool value)
        {
            _selfObject.gameObject.SetActive(value);
        }
    }
}

