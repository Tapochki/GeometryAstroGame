using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TandC.RunIfYouWantToLive 
{
    public partial class Enemy
    {
        public class DefaultMove : IEnemyMove
        {
            private Enemy _enemy;
            private OnBehaviourHandler _onDetectHandler;
            private bool _needMove;
            private bool _isNeedMove;
            public bool CanAction { get; private set; }
            private float _movingTime;
            private Vector2 _endPosition;

            public void Init(Enemy enemy)
            {
                _enemy = enemy;
                CanAction = true;
            }

            public void Update() 
            {
                Move();
            }

            public void Move()
            {
                var targetPosition = _enemy._playerTransform.position - _enemy.EnemyTransform.position;
                targetPosition.Normalize();
                _enemy._rigidbody2d.MovePosition(_enemy.EnemyTransform.position + (targetPosition * _enemy.MovementSpeed * Time.fixedDeltaTime));
            }
        }
    }

}

