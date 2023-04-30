using System;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public class Border
    {
        private GameObject _selfObject;
        private GameObject _playerModel;

        private OnBehaviourHandler _onBehaviourHandler;

        public Action PlayerLeaveBorderEvent;
        public Action PlayerBackGameLocationEvent;

        public Border(GameObject selfObject, GameObject playerModel)
        {
            _selfObject = selfObject;
            _playerModel = playerModel;
            _onBehaviourHandler = _selfObject.GetComponent<OnBehaviourHandler>();
            _onBehaviourHandler.Trigger2DExited += PlayerLeaveBorder;
            _onBehaviourHandler.Trigger2DEntered += PlayerBackToBorder;
        }

        private void PlayerLeaveBorder(GameObject collider)
        {
            if (collider != _playerModel)
                return;

            Debug.Log("Player leave");
            PlayerLeaveBorderEvent?.Invoke();
        }

        private void PlayerBackToBorder(GameObject collider)
        {
            if (collider != _playerModel)
                return;


            Debug.Log("Player back");
            PlayerBackGameLocationEvent?.Invoke();
        }
    }
}