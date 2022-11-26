using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TandC.RunIfYouWantToLive 
{
    public partial class Enemy
    {
        public class MovingOnDistanceMove : IEnemyMove
        {
            private Enemy _enemy;
            private OnBehaviourHandler _onDetectHandler;
            private bool _needMove;
            private bool _isNeedMove;
            public bool CanAction { get; private set; }
            private float _movingTime;
            private Vector2 _endPosition;
            private Vector2 _currentPosition;

            public void Init(Enemy enemy)
            {
                _enemy = enemy;
                _onDetectHandler = _enemy.SelfObject.transform.Find("DetectRadius").GetComponent<OnBehaviourHandler>();
                _onDetectHandler.Trigger2DEntered += OnPlayerDetect;
                _onDetectHandler.Trigger2DExited += OnPlayerLost;
                _isNeedMove = true;
                CanAction = false;
                _currentPosition = new Vector2();
                _movingTime = 1f;
            }

            private void OnGetLostPlayer(bool value)
            {
                _isNeedMove = !value;
                if (_isNeedMove)
                {
                    _enemy._rigidbody2d.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
                }

                _endPosition = _enemy._rigidbody2d.position;
                if (value)
                {
                    _movingTime = 1f;
                    _needMove = false;
                    _endPosition = _enemy._rigidbody2d.position;
                }
            }

            private void OnPlayerDetect(GameObject collider)
            {
                if (collider.tag == "Player")
                {
                    OnGetLostPlayer(true);
                    CanAction = true;
                }
            }
            private void OnPlayerLost(GameObject collider)
            {
                if (collider.tag == "Player")
                {
                    OnGetLostPlayer(false);
                }
            }

            public void Update() 
            {
                Move();
            }

            public void Move()
            {
                if (_needMove)
                {
                    _movingTime -= Time.deltaTime;
                    var targetPosition = _enemy._playerTransform.position - _enemy.EnemyTransform.position;
                    targetPosition.Normalize();
                    _enemy._rigidbody2d.MovePosition(_enemy.EnemyTransform.position + (targetPosition * _enemy.MovementSpeed * Time.fixedDeltaTime));
                    if (_movingTime <= 0)
                    {
                        _movingTime = 1f;
                        _needMove = false;
                        _endPosition = _enemy._rigidbody2d.position;
                    }
                }
                if (_isNeedMove)
                {
                    var targetPosition = _enemy._playerTransform.position - _enemy.EnemyTransform.position;
                    targetPosition.Normalize();

                    _enemy._rigidbody2d.MovePosition(_enemy.EnemyTransform.position + (targetPosition * _enemy.MovementSpeed * Time.fixedDeltaTime));

                    //if(_currentPosition == _enemy.Rigidbody2D.position) 
                    //{
                    //    _enemy._rigidbody2d.MovePosition(_enemy.EnemyTransform.position + (new Vector3(targetPosition.x + 20, targetPosition.y, 0) * _enemy.MovementSpeed * Time.fixedDeltaTime));
                    //}

                    _currentPosition = _enemy.Rigidbody2D.position;
                }
                else
                {
                    if (_needMove == false && _enemy._rigidbody2d.position != _endPosition)
                    {
                        _needMove = true;
                    }
                }
                if (!_isNeedMove)
                {
                    if (Vector2.Distance(_enemy._rigidbody2d.position, _endPosition) >= 10f)
                    {
                        _enemy._rigidbody2d.constraints = RigidbodyConstraints2D.FreezeAll;
                    }
                }
            }
        }
    }

}

