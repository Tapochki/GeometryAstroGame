using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Helpers;
using UnityEngine;


namespace TandC.RunIfYouWantToLive 
{
    public class LaserWeapon : Weapon
    {

        public LaserWeapon()
        {
           
        }

        protected override void RegisterNewWeapon()
        {
            _weaponTransform = _selfObject.transform;
            _weaponDirection = _selfObject.transform.Find("ShootDirection");
        }
        protected override void ShotGetReady()
        {
            Shoot();
        }
    }
}

