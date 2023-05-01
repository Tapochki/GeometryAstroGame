using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TandC.RunIfYouWantToLive 
{
    public partial class Enemy
    {
        public class DefaultAction : IEnemyAction
        {
            private Enemy _enemy;
           
            public void Init(Enemy enemy)
            {
                _enemy = enemy;
            }

            public void Action()
            {
                _enemy._actualCooldown = _enemy._attackCooldown;
                _enemy.IsCanAction = true;
                
            }

            public void Update() 
            {
                _enemy._actualCooldown -= Time.deltaTime;
                if (_enemy._actualCooldown <= 0)
                {
                    _enemy.Action();
                }
                
            }
        }
    }

}

