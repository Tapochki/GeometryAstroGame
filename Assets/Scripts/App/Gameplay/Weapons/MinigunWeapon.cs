using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Helpers;
using UnityEngine;


namespace TandC.RunIfYouWantToLive 
{
    public class MinigunWeapon : Weapon
    {
        private int _shootCount;
        private float _directionAngle;

        private const int _startShotCount = 10;
        private const float _startDirectionAngle = 60;

        private Transform _weaponPivotTransform;
        private Transform _weaponPivotDirection;

        private bool _startShoot;
        private float _shootMinigunTimer;
        private int _gunNumber;

        public MinigunWeapon()
        {

        }
        protected override void LocalInit() 
        {
            _shootCount = _startShotCount;
            _directionAngle = _startDirectionAngle;
        }
        public void UpgradeShotCount(int value)
        {
            _shootCount += value;
        }
        public void UpdateDirectionAngle(float value) 
        {
            _directionAngle += value;
        }
        private void ShootAfterShoot()
        {
            _startShoot = true;
            _shootMinigunTimer = 0.12f;
            _gunNumber = 0;
        }

        private void StopShoot() 
        {
            _startShoot = false;
        }

        private void ReloadShot() 
        {
            _shootMinigunTimer = 0.12f;
            _gunNumber++;
            if(_gunNumber == _shootCount) 
            {
                StopShoot();
            }
        }

        public override void Update()
        {
            base.Update();

            if (_startShoot) 
            {
                _shootMinigunTimer -= Time.deltaTime;
                if(_shootMinigunTimer <= 0) 
                {
                    ReloadShot();
                    Shoot();
                }
            }
        }

        protected override void RegisterNewWeapon()
        {
            _weaponTransform = _selfObject.transform;
            _weaponDirection = _selfObject.transform.Find("ShootDirection");
            _weaponPivotTransform = _selfObject.transform.parent.Find("MiniGunPivot");
            _weaponPivotDirection = _weaponPivotTransform.Find("ShootDirectionPivot");
        }

        protected override void ShotGetReady()
        {

            _shootTempDeley = _shootDeley;
            if (_canShoot) 
            {
                _canShoot = false;
                ShootAfterShoot();
            }

        }
        protected override void Shoot(object[] data = null)
        {
            var shotDirection = Random.Range(_directionAngle * -1, _directionAngle + 1);
            _weaponPivotTransform.localPosition = new Vector2(_weaponTransform.localPosition.x + shotDirection, _weaponTransform.localPosition.y);
            Shoot(_weaponPivotTransform.position, _weaponPivotDirection.position);
        }


    }
}

