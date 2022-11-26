using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TandC.RunIfYouWantToLive
{
    public class OnBehaviourHandler : MonoBehaviour
    {
        public event Action<GameObject> Trigger2DEntered;

        public event Action<GameObject> Trigger2DStay;

        public event Action<GameObject> Trigger2DExited;

        public event Action<GameObject> Destroying;

        public event Action<GameObject> OnParticleCollisionEvent;

        public event Action<string> OnAnimationStringEvent;


        private void OnTriggerExit2D(Collider2D collider)
        {
            Trigger2DExited?.Invoke(collider.gameObject);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Trigger2DEntered?.Invoke(collision.gameObject);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            Trigger2DStay?.Invoke(collision.gameObject);
        }

        private void OnDestroy()
        {
            Destroying?.Invoke(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Trigger2DEntered?.Invoke(collision.gameObject);
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            Trigger2DStay?.Invoke(collision.gameObject);
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            Trigger2DExited?.Invoke(collision.gameObject);
        }

        private void OnParticleCollision(GameObject other)
        {
            OnParticleCollisionEvent?.Invoke(other);
        }

        private void OnAnimationEvent(string parameter)
        {
            OnAnimationStringEvent?.Invoke(parameter);
        }
    }
}
