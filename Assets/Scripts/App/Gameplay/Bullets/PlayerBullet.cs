using System;
using UnityEngine;

namespace TandC.RunIfYouWantToLive 
{
    public class PlayerBullet : Bullet
    {
        public Common.Enumerators.WeaponType BulletType { get; private set; }
        public int DropChance { get; private set; }
        public Action<PlayerBullet, GameObject> OnColliderEvent;
        public PlayerBullet(Transform parent, PlayerBulletData data, Vector2 direction, float damage, int dropChance, Vector2 startPosition, int bulletLife = 1) : base(parent, data, direction, damage, startPosition, bulletLife)
        {
            DropChance = dropChance;
            BulletType = data.type;

        }

        public override void EndMove(GameObject collider)
        {
            {
                if (IsLife)
                {
                    if (collider.tag == "Enemy")
                    {
                        _bulletLife--;
                        OnColliderEvent?.Invoke(this, collider);
                    }
                }
            }
        }
    }
}

