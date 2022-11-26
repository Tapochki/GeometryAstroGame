using System;
using TMPro;
using UnityEngine;
using static TandC.RunIfYouWantToLive.Common.Enumerators;

namespace TandC.RunIfYouWantToLive 
{
    public class Item
    {
        public event Action<Item> ItemDestroyHandler;
        public GameObject SelfObject;
        public ItemType ItemType;
        private OnBehaviourHandler _behaviourHandler;
        public int ItemValue;
        private PlayerController _playerController;
        private Rigidbody2D _rigidbody2d;

        private bool _startMoving;

        public Item(GameObject prefab, Transform parent, Vector2 spawnPosition, ItemType type, int itemValue) 
        {
            _playerController = GameClient.Get<IGameplayManager>().GetController<PlayerController>();
            SelfObject = MonoBehaviour.Instantiate(prefab, parent);
            SelfObject.transform.position = spawnPosition;
            _rigidbody2d = SelfObject.transform.GetComponent<Rigidbody2D>();
            _behaviourHandler = SelfObject.GetComponent<OnBehaviourHandler>();
            _behaviourHandler.Trigger2DEntered += OnColliderHandler;
            ItemType = type;
            ItemValue = itemValue;

            _startMoving = false;
        }

        public void OnColliderHandler(GameObject collider) 
        {
            if(collider.tag == "PickUp")
            {
                if(ItemType == ItemType.RocketBox) 
                {
                    if (_playerController.IfRocketIsMax()) 
                    {
                        StopMoving();
                        return;
                    }
                }
                if (_playerController.Player.IsMaskActive) 
                {
                    StopMoving();
                    return;
                }
                StartMoving();
            }
            if (collider.tag == "Player")
            {
                if (ItemType == ItemType.RocketBox)
                {
                    if (_playerController.IfRocketIsMax())
                    {
                        return;
                    }
                }
                ItemDestroyHandler?.Invoke(this);
            }

        }

        public void StopMoving() 
        {
            _startMoving = false;
        }

        public void StartMoving() 
        {
            _startMoving = true;
        }

        public void Update() 
        {
            if (_startMoving) 
            {
                if(_playerController.Player != null) 
                {
                    var targetPosition = _playerController.Player.SelfTransform.position - SelfObject.transform.position;
                    targetPosition.Normalize();

                    _rigidbody2d.MovePosition(SelfObject.transform.position + (targetPosition * 300 * Time.fixedDeltaTime));
                }
            }

        }

        public void Dispose() 
        {
            MonoBehaviour.Destroy(SelfObject);
        }
    }
}

