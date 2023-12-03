using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TandC.RunIfYouWantToLive 
{
    public class Dodge
    {
        private Transform _objectTransform;

        private float _dodgeTimer = 0.2f;
        private bool _isDodge;
        private float _dodgePower;
        private Vector2 _dodgeDirection;
        public Dodge(Transform transform) 
        {
            _objectTransform = transform;
        }
        public void Update() 
        {
            if (_isDodge)
            {
                Move();
            }
        }
        public void Move()
        {
            int playerDirection = _objectTransform.transform.rotation.eulerAngles.z > 90f || _objectTransform.transform.rotation.eulerAngles.z < -90f ? -1 : 1;
            _objectTransform.Translate(_dodgeDirection * _dodgePower * playerDirection * Time.deltaTime);
            _dodgeTimer -= Time.deltaTime;
            if (_dodgeTimer < 0)
            {
                _isDodge = false;
            }
        }
        public void StartDodge(Vector2 direction, float dodgePower)
        {
            _dodgeTimer = 0.1f;
            _dodgeDirection = direction;
            _dodgePower = dodgePower;
            _isDodge = true;
        }

    }
}

