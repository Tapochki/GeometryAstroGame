using System;
using UnityEngine;

namespace TandC.RunIfYouWantToLive 
{
    public class Blow
    {
        private GameObject _selfObject;
        private float _damage;
        private int _dropChance;

        public Action<GameObject, float, int> OnGetEnemy;
        private OnBehaviourHandler _onBehaviourHandler;


        public Blow(GameObject selfObject, float damage, int dropChance) 
        {
            _selfObject = selfObject;
            _onBehaviourHandler = _selfObject.GetComponent<OnBehaviourHandler>();
            _damage = damage;
            _dropChance = dropChance;
            _onBehaviourHandler.Trigger2DEntered += OnColliderEnter;
        }

        private void OnColliderEnter(GameObject collider)
        {
            if (collider.tag != "Enemy") 
            {
                return;
            }

            OnGetEnemy?.Invoke(collider, _damage, _dropChance);
        }
    }
}

