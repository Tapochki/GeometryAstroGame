using System;
using UnityEngine;


namespace TandC.RunIfYouWantToLive 
{
    public abstract class Bullet
    {
        private GameObject _selfObject;
        public GameObject SelfObject => _selfObject;
        private OnBehaviourHandler _onBehaviourHandler;

        private float _lifeTimer = 5f;
        private Vector2 _targetDirection;
        public float Damage { get; set; }
        private float _speed;
        private Rigidbody2D _rigidbody2D;
        private bool _canStartMove = false;
        private GameObject _spriteBullet;

        public bool IsLife;

        protected int _bulletLife = 1;


        public Bullet(Transform parent, BulletData data, Vector2 direction, float damage, Vector2 startPosition, int bulletLife = 1) 
        {
            _selfObject = MonoBehaviour.Instantiate(data.ButlletObject, parent);
            _selfObject.transform.position = startPosition;
            _targetDirection = direction;
            _onBehaviourHandler = _selfObject.GetComponent<OnBehaviourHandler>();
            _spriteBullet = _selfObject.transform.Find("Model").gameObject;
            _rigidbody2D = _selfObject.GetComponent<Rigidbody2D>();
            _onBehaviourHandler.Trigger2DEntered += EndMove;
            Damage = damage;

            _speed = data.BulletSpeed;
            _spriteBullet.SetActive(false);
            _canStartMove = false;
            _lifeTimer = data.bulletLifeTime;
            IsLife = true;
            _bulletLife = bulletLife;
            SetRotation();         
        }

        protected void Move() 
        {
            _selfObject.transform.Translate(Vector2.up * _speed * Time.deltaTime);
        }
    
        public void Update()
        {
            _lifeTimer -= Time.deltaTime;

            if (_lifeTimer <= 0)
            {
                IsLife = false;
            }
            if (!_canStartMove)
            {
                return;
            }

            Move();
        }
        public abstract void EndMove(GameObject collider);
        //{
        //    if (IsLife) 
        //    {
        //        if (collider.tag == "Enemy")
        //        {
        //            _bulletLife--;
        //            
        //        }
        //    }
       // }

        public void Dispose() 
        {
            if(_bulletLife >= 1 && IsLife) 
            {
                return;
            }
            IsLife = false;
            _canStartMove = false;
            if(_selfObject != null) 
            {
                MonoBehaviour.Destroy(_selfObject);
            }
        }
        private void SetRotation()
        {
            Vector2 direction = _targetDirection - _rigidbody2D.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            _selfObject.transform.localEulerAngles = new Vector3(0, 0, angle);
            //_rigidbody2D.position = _targetDirection;
            _rigidbody2D.freezeRotation = true;
            _spriteBullet.SetActive(true);
            _canStartMove = true;
        }
    }
}

