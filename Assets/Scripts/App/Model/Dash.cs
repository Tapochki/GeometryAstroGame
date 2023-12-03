using System;
using UnityEngine;

namespace TandC.RunIfYouWantToLive 
{
    public class Dash
    {
        public Action EndDashEvent;
        private Transform _objectTransform;
        private GameObject _dashContainer;

        private float _dashTimer;
        private bool _isDash;
        private float _speed;

        private const float _dashPower = 500f;

        public Dash(Transform transform, GameObject dashContainer) 
        {
            _objectTransform = transform;
            _dashContainer = dashContainer;
        }
        public void Update() 
        {
            if (_isDash)
            {
                Move();
            }
        }
        public void Move()
        {
            _objectTransform.transform.Translate(Vector2.up * _dashPower * Time.fixedDeltaTime);
            _dashTimer -= Time.deltaTime;
            if (_dashTimer < 0)
            {
                EndMove();
            }
        }

        private void EndMove() 
        {
            EndDashEvent?.Invoke();
            EndDashEvent = null;
            if (_dashTimer <= -0.5f)
            {
                _isDash = false;
                _dashContainer.SetActive(false);
            }
        }

        public void StartDash(float speed, float dashTimer = 0.15f)
        {
            _speed = speed;
            _dashTimer = dashTimer;
            _dashContainer.SetActive(true);
            _isDash = true;
        }

    }
}

