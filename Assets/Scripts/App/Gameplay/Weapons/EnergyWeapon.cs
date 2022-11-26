using System;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Helpers;
using UnityEngine;


namespace TandC.RunIfYouWantToLive 
{
    public class EnergyWeapon : Weapon
    {
        private PlayerController _playerController;
        private ObjectsController _objectsController;
        private EnemyController _enemyController;
        private int _beamcount;
        private List<Beam> _useWeapons;
        private float _hitTargetDelayTimer;
        public EnergyWeapon()
        {
            _enemyController = GameClient.Instance.GetService<IGameplayManager>().GetController<EnemyController>();
            _playerController = GameClient.Instance.GetService<IGameplayManager>().GetController<PlayerController>();
            _objectsController = GameClient.Instance.GetService<IGameplayManager>().GetController<ObjectsController>();
        }
        protected override void LocalInit()
        {
            _useWeapons = new List<Beam>();
            _beamcount = 0;
            _hitTargetDelayTimer = 1f;
            _weaponTransform = _selfObject.transform;
        }

        public void BeamHitTimeDecrease(float value) 
        {
            _hitTargetDelayTimer -= value;
            foreach(var beam in _useWeapons) 
            {
                beam.UpdateHitTimer(_hitTargetDelayTimer);
            }
        }

        public void UpgradeShotCount()
        {
            if (_beamcount == 5)
            {
                return;
            }
            RegisterNewWeapon();
        }

        protected override void RegisterNewWeapon()
        {
            _beamcount++;
            Beam beam = new Beam(MonoBehaviour.Instantiate(_bulletData.ButlletObject, _objectsController.BulletContainer), _hitTargetDelayTimer, _playerController.Player.SelfTransform);
            beam.id = _beamcount;
            beam.OnTargetHitEvent += OnEnemyHitEnentHandler;
            _useWeapons.Add(beam);
        }
        public override void Update()
        {
            if (!IsActive) 
            {
                return; 
            }
            for(int i = 0; i < _useWeapons.Count; i++)
            {
                _useWeapons[i].Update();
            }
        }

        //private GameObject FindClosetEnemy()
        //{
        //    if (_enemyController.ClosetEnemyObjects.Count == 0)
        //    {
        //        return null;
        //    }
        //    GameObject closetEnemy = _enemyController.ClosetEnemyObjects[0];
        //    bool isEnemyTargetAlready = false;
        //    for (int i = 0; i <= _enemyController.ClosetEnemyObjects.Count - 1; i++)
        //    {
        //        if (closetEnemy == null)
        //        {
        //            return null;
        //        }

        //        float closetEnemyDistance =
        //               Vector2.Distance(closetEnemy.transform.position, _weaponTransform.position);

        //        if (_enemyController.ClosetEnemyObjects[i] == null)
        //        {
        //            return null;
        //        }

        //        var enemyPlayerDistance = Vector2.Distance(_enemyController.ClosetEnemyObjects[i].transform.position,
        //           _weaponTransform.position);
        //        if (enemyPlayerDistance < closetEnemyDistance)
        //        {

        //            closetEnemy = _enemyController.ClosetEnemyObjects[i];
        //        }
        //        isEnemyTargetAlready = false;
        //        foreach (var beam in _useWeapons)
        //        {
        //            if (beam.TargetEnemy != null)
        //            {
        //                if (beam.TargetEnemy == closetEnemy.transform)
        //                {
        //                    isEnemyTargetAlready = true;
        //                    break;
        //                }
        //            }
        //        }
        //        if (isEnemyTargetAlready)
        //        {
        //            continue;
        //        }
        //    }
        //    if (isEnemyTargetAlready)
        //    {
        //        return null;
        //    }
        //    return closetEnemy;
        //}

        public override void Dispose()
        {
            foreach(var weapon in _useWeapons) 
            {
                weapon.OnTargetHitEvent -= OnEnemyHitEnentHandler;
                weapon.Dispose();
            }
            _useWeapons.Clear();
            base.Dispose();
        }

        protected override void ShotGetReady()
        {

        }

        private void OnEnemyHitEnentHandler(Transform enemy) 
        {
            _enemyController.HitEnemy(enemy.gameObject, _baseDamage, _dropChance);
        }

        
    }

    public class Beam 
    {
		public event Action<Transform> OnTargetHitEvent;
        private EnemyController _enemyController;
		private GameObject _selfObject;

        public int id;

		private Transform _lineObject;

        private Transform _playerObject;

		public Transform TargetEnemy;

		private float _hitTargetDelay;

        private float _hitTargetDelayTimer;

        private float _reloadTargetDelay;

        private float _reloadDelayTimer;

        public bool IsShoot => TargetEnemy != null;

        private Transform _endLinePoint;


        public Beam(GameObject gameObject, float hitTargetDelayTimer, Transform playerObject)
		{
            _enemyController = GameClient.Get<IGameplayManager>().GetController<EnemyController>();
            _hitTargetDelayTimer = hitTargetDelayTimer;
            _reloadDelayTimer = 0.2f * id;
            _reloadTargetDelay = 0.1f;
            _selfObject = gameObject;
            _playerObject = playerObject;
            _lineObject = _selfObject.transform.Find("Line");
            _endLinePoint = _selfObject.transform.Find("LightingPoint");

            SetStatus(false);
		}

        public void UpdateHitTimer(float value) 
        {
            _hitTargetDelayTimer = value;
        }

        public void LostEnemy(Enemy enemy) 
        {
            if(enemy.EnemyTransform == TargetEnemy) 
            {
                SetStatus(false);
            }
        }


		public void SetStatus(bool status)
		{
            if (!status)
			{
                Vector3 objectScale = _lineObject.localScale;
                objectScale.y = 0;
                _lineObject.localScale = objectScale;
                TargetEnemy = null;
			}
            _selfObject.SetActive(status);
		}

        public void DeleteTarget() 
        {
            TargetEnemy = null;
            SetStatus(false);
        }

        public void RegisterEnemy() 
        {
            if (_enemyController.ClosetEnemyObjects.Count <= 0) 
            {
                _reloadTargetDelay = _hitTargetDelayTimer;
                return;
            }
            SetStatus(false);

            var targertEnemy = _enemyController.GetClosetEnemyForEnergy(_playerObject.transform);
            if (targertEnemy.IsTargetAlready)
            {
                return;
            }
            targertEnemy.IsTargetAlready = true;
            TargetEnemy = targertEnemy.SelfObject.transform;


            _reloadTargetDelay = _hitTargetDelayTimer;
            if (TargetEnemy == null) 
            {
                return;
            }
            _hitTargetDelay = _hitTargetDelayTimer;
           
            SetRotation();
            SetScale();
            SetRotation();
            SetScale();
            SetStatus(true);
        }

		public void Update()
		{
			if (TargetEnemy == null) 
            {
                SetStatus(false);
                _reloadTargetDelay -= Time.deltaTime;
                if(_reloadTargetDelay <= 0) 
                {
                    RegisterEnemy();
                }

                return;
            }

            SetRotation();
            SetScale();
            _hitTargetDelay -= Time.deltaTime;
			if (_hitTargetDelay <= 0f)
			{
				if (TargetEnemy != null)
				{
                    _hitTargetDelay = _hitTargetDelayTimer;
                    OnTargetHitEvent?.Invoke(TargetEnemy);

                }
			}
		}

        public void Dispose() 
        {
            MonoBehaviour.Destroy(_selfObject);
        }

        private void SetRotation()
        {
            if(TargetEnemy == null) 
            {
                return;
            }
            Vector3 direction = TargetEnemy.position - _selfObject.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

            //_selfObject.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            _selfObject.transform.localEulerAngles = new Vector3(0, 0, angle);
        }

        private void SetScale()
		{
            _selfObject.transform.position = _playerObject.transform.position;
            float distance = Vector2.Distance(_selfObject.transform.position, TargetEnemy.position);
            _endLinePoint.transform.position = TargetEnemy.position;
            float scale = distance / (250f / 2.4f) ;

			Vector3 objectScale = _lineObject.localScale;

            	objectScale.y = scale;

            if(objectScale.y > 2.2f) 
            {
                SetStatus(false);
            }
            _lineObject.localScale = objectScale;
		}

	}
}

