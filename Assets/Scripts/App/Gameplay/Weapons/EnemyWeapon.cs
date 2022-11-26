using System;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Helpers;
using UnityEngine;


namespace TandC.RunIfYouWantToLive 
{
    public class EnemyWeapon : Weapon
    {
        private OnBehaviourHandler _behaviourHandler;
        private bool _isCanShot;

        public EnemyWeapon(Transform playerTransform)
        {
            _isCanShot = false;
            _weaponDirection = playerTransform;
        }

        public void UpgradeBulletLife(int value) 
        {
            _bulletLife += value;
        }

        private void OnHandlerEnter(GameObject collider) 
        {
            if(collider.tag == "Player") 
            {
                _isCanShot = true;
            }
        }
        private void OnHandlerExit(GameObject collider) 
        {
            if (collider.tag == "Player")
            {
                _isCanShot = false;
            }
        }
        
        protected override void RegisterNewWeapon()
        {
            _weaponTransform = _selfObject.transform;
            _behaviourHandler = _weaponTransform.GetComponent<OnBehaviourHandler>();
            _behaviourHandler.Trigger2DEntered += OnHandlerEnter;
            _behaviourHandler.Trigger2DExited += OnHandlerExit;
        }

        public override void Update()
        {
            base.Update();
        }

        protected override void ShotGetReady()
        {
            if (_isCanShot) 
            {
                Shoot();
            }
        }
        
    }
}

