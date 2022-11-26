using System;
using TandC.RunIfYouWantToLive.Common;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public class ShieldSkillVFX : VFXBase
    {
        public Action ShieldTakeDamageEvent;
        public Action<bool> ShieldIsActiveEvent;

        private bool _shieldCooldownStart,
                        _shieldActive;

        private float _shieldCooldown = 5f,
                        _shieldCurrentCooldown;
        public int ShieldDefaultHealth,
                        ShieldCurrentHealth;

        private GameObject _shieldObject;

        private Animator _animator;

        public ShieldSkillVFX(Enumerators.SkillType type) : base(type)
        {
            _playerController.PlayerGotShieldSkill(true);
            _shieldCooldownStart = true;

            _shieldObject = _playerController.Player.SelfObject.transform.Find("Body/[Skills]/ShieldSkill").gameObject;
            _shieldObject.SetActive(false);
            _animator = _shieldObject.GetComponent<Animator>();
            ShieldDefaultHealth = 0;

            _shieldCurrentCooldown = 0;

            ShieldCurrentHealth = ShieldDefaultHealth;

            ShieldTakeDamageEvent += ShieldTakeDamageEventHandler;
        }

        public override void Update()
        {
            base.Update();
            if (_shieldCooldownStart)
            {
                _shieldCurrentCooldown -= Time.deltaTime;
                if (_shieldCurrentCooldown <= 0)
                {
                    _shieldCurrentCooldown = _shieldCooldown;
                    ShieldTurnOn();
                    if(ShieldDefaultHealth == ShieldCurrentHealth) 
                    {
                        _shieldCooldownStart = false;
                    }
                }
            }
        }

        public void DecreaseShieldRecovery(float value) => _shieldCooldown -= value;
        public void IncreaseShieldHealth() => ShieldDefaultHealth++;

        private void ShieldTurnOn()
        {
            //Debug.Log("<color=#4EFFEE>SHIELD TURN ON</color>");
            ShieldCurrentHealth++;
            _animator.Play("Appear", -1, 0);
            _shieldObject.SetActive(true);
            ShieldIsActiveEvent?.Invoke(true);
            _shieldActive = true;
            ShieldDefaultSettings();
        }

        private void ShieldDefaultSettings()
        {
            _shieldCurrentCooldown = _shieldCooldown;
            _shieldCooldownStart = true;
        }

        private void ShieldTakeDamageEventHandler()
        {
            //Debug.Log("<color=#E01010>SHIELD TAKE DAMAGE</color>");
            if (ShieldCurrentHealth > 0)
                ShieldCurrentHealth--;
            if (_shieldActive)
            {
                _animator.Play("Damage", -1, 0);
                if (ShieldCurrentHealth <= 0)
                    ShieldTurnOff();
            }
        }

        public void ShieldOffByImpulse(int damage)
        {
            //Debug.Log("<color=#10E03A>SHIELD TAKE DAMAGE BY IMPULSE</color>");
            ShieldCurrentHealth = 0;
            ShieldTurnOff();
            _shieldCurrentCooldown = _shieldCooldown * damage;
        }

        public void ShieldTurnOff()
        {
            //Debug.Log("<color=#E010E0>SHIELD TURN OFF</color>");
            _animator.Play("Break", -1, 0);
            //_shieldObject.SetActive(false);
            _shieldCurrentCooldown = _shieldCooldown;
            _shieldActive = false;
            ShieldIsActiveEvent?.Invoke(false);
            _shieldCooldownStart = true;

        }
    }
}