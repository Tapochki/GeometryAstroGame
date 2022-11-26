using System;
using UnityEngine;

namespace TandC.RunIfYouWantToLive 
{
    public class EnemyBullet : Bullet
    {
        public Action<EnemyBullet, GameObject> OnColliderEvent;
        public EnemyBullet(Transform parent, EnemyBulletData data, Vector2 direction, float damage, Vector2 startPosition, int bulletLife = 1) : base(parent, data, direction, damage, startPosition, bulletLife)
        {

        }

        public override void EndMove(GameObject collider)
        {
            {
                if (IsLife)
                {
                    if (collider.tag == "Player")
                    {
                        _bulletLife--;
                        OnColliderEvent?.Invoke(this, collider);
                    }
                }
            }
        }
    }
}

