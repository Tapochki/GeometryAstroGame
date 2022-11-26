using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TandC.RunIfYouWantToLive
{
    public class DroneParent
    {
        private bool _isActive;
        private GameObject _selfObject;
        private List<Drone> _drons;
        private float _speed;
        public int DronsDamage;
        private int _dronsActiveCount;
        public bool IsColliderOnEnemy;
        private Transform _player;
        public Action<GameObject, GameObject, float> OnDroneHandlerEnter;
        public GameObject _dronePrefab;

        public DroneParent(GameObject prefab, float speed, int damage, Transform player, GameObject dronePrefab)
        {
            _selfObject = prefab;
            _speed = speed;
            _player = player;
            DronsDamage = damage;
            _drons = new List<Drone>();
            _dronePrefab = dronePrefab;
            IsColliderOnEnemy = false;
            _dronsActiveCount = 0;
            HideAll();
        }

        public void UpgradeDamage(int value)
        {
            DronsDamage += value;
        }

        public void UpgradeDroneSpeed(float value)
        {
            _speed += value;
        }

        public void RegisterNewDrone()
        {
            if (_dronsActiveCount == 4)
            {
                Debug.LogError("So Much Drone");
                return;
            }
            if (_dronsActiveCount == 0)
            {
                _isActive = true;
            }
            Drone drone = new Drone(MonoBehaviour.Instantiate(_dronePrefab, _selfObject.transform.Find($"Drone_{_dronsActiveCount}")));
            drone.ActivateDrone();
            drone.OnHandlerEnter += OnDroneHandler;
            _drons.Add(drone);
            _dronsActiveCount++;

            if(_dronsActiveCount == 3) 
            {
                _drons[0].SetThirdPosition();
                _drons[1].SetThirdPosition();
            }
            if (_dronsActiveCount == 4)
            {
                _drons[0].SetNormalPosition();
                _drons[1].SetNormalPosition();
            }
        }
        private void OnDroneHandler(GameObject collider, GameObject drone) 
        {
            OnDroneHandlerEnter?.Invoke(collider, drone, DronsDamage);
        }
       

        public void HideAll()
        {
            foreach (var dron in _drons)
            {
                dron.DeactiveDrone();
            }
        }

        public void Update()
        {
            if (_isActive)
            {
                _selfObject.transform.position = _player.transform.position;
                _selfObject.transform.Rotate(0, 0, _speed * Time.deltaTime);
            }
        }

        private class Drone 
        {
            private GameObject _selfObject;
            private OnBehaviourHandler _onBehaviourHandler;
            public Action<GameObject, GameObject> OnHandlerEnter;
            public Drone(GameObject gameObject) 
            {
                _selfObject = gameObject;
                _onBehaviourHandler = _selfObject.transform.GetComponent<OnBehaviourHandler>();
            }

            public void SetThirdPosition() 
            {
                _selfObject.transform.parent.localPosition = new Vector2(_selfObject.transform.parent.localPosition.x, _selfObject.transform.localPosition.y - 30f);
            }

            public void SetNormalPosition()
            {
                _selfObject.transform.parent.localPosition = new Vector2(_selfObject.transform.parent.localPosition.x, 0);
            }

            public void DeactiveDrone() 
            {
                _selfObject.SetActive(false);
                _onBehaviourHandler.Trigger2DEntered -= OnBehaviorHandlerEnter;
            }

            public void ActivateDrone() 
            {
                _selfObject.SetActive(true);
                _onBehaviourHandler.Trigger2DEntered += OnBehaviorHandlerEnter;
            }

            private void OnBehaviorHandlerEnter(GameObject collider)
            {
                if (collider.tag != "Enemy")
                {
                    return;
                }
                OnHandlerEnter?.Invoke(collider, _selfObject);
            }
        }
    }
}

