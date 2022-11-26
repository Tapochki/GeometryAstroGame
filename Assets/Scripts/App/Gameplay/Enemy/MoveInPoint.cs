using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TandC.RunIfYouWantToLive 
{
    public partial class Enemy
    {
        public class MoveInPoint : IEnemyMove
        {
            private Enemy _enemy;
            public bool CanAction { get; private set; }
            private bool _isInit = false;
            private Vector3 _movePoint;

            public MoveInPoint(Transform movePoint) 
            {
                _movePoint = movePoint.position;
            }

            public void Init(Enemy enemy)
            {
                _enemy = enemy;
                CanAction = true;
                Vector2 direction = _movePoint - _enemy.SelfObject.transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
                _enemy.SelfObject.transform.localEulerAngles = new Vector3(0, 0, angle);
                _enemy._rigidbody2d.freezeRotation = true;
                _isInit = true;
            }

            public void Update() 
            {
                Move();
            }

            public void Move()
            {
                if (_isInit)
                {
                    _enemy.SelfObject.transform.Translate(Vector2.up * _enemy.MovementSpeed * 5 * Time.deltaTime);
                }
            }
        }
    }

}

