using System;
using UnityEngine;

namespace TandC.RunIfYouWantToLive 
{
    public class Mask
    {
        public bool IsMaskActive;
        private float _maskTimer;
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider;
        public Mask(Collider2D collider, SpriteRenderer spriteRenderer) 
        {
            _collider = collider;
            _spriteRenderer = spriteRenderer;
        }
        public void Update() 
        {
            if (IsMaskActive)
            {
                _maskTimer -= Time.deltaTime;
                if (_maskTimer <= 0)
                {
                    EndMask();
                }
            }
        }
        public void StartMask(float maskTimer = 3f)
        {
            _maskTimer = maskTimer;
            _collider.isTrigger = true;
            _spriteRenderer.color = new Color(1f, 1f, 1f, 0.7f);
            IsMaskActive = true;
        }
        private void EndMask()
        {
            _collider.isTrigger = false;
            _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            IsMaskActive = false;
        }
    }
}

