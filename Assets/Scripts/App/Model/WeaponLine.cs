using UnityEngine;


namespace TandC.RunIfYouWantToLive 
{
    public class WeaponLine
    {
        private GameObject _selfObjcet;

        public bool IsEnemyOnLine { get; private set; }

        private OnBehaviourHandler _onBehaviourHandler;


        public WeaponLine(GameObject selfObject) 
        {
            _selfObjcet = selfObject;

            _onBehaviourHandler = _selfObjcet.GetComponent<OnBehaviourHandler>();
            _onBehaviourHandler.Trigger2DEntered += OnHandlerEnter;
            _onBehaviourHandler.Trigger2DStay += OnHandlerStay;
            _onBehaviourHandler.Trigger2DExited += OnHandlerExit;
            IsEnemyOnLine = false;
        }

        private void OnHandlerEnter(GameObject collider) 
        {
            if(collider.tag == "Enemy") 
            {
                IsEnemyOnLine = true;
            }
        }
        private void OnHandlerStay(GameObject collider)
        {
            if (collider.tag == "Enemy")
            {
                IsEnemyOnLine = true;
            }
        }
        private void OnHandlerExit(GameObject collider) 
        {
            if (collider.tag == "Enemy")
            {
                IsEnemyOnLine = false;
            }
        }
    }
}

