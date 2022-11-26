using System;
using UnityEngine;

namespace TandC.RunIfYouWantToLive.Helpers
{
    public class CounterAnimation
    {
        public float CurrentValue { get; private set; } = 0;
        public float StartValue { get; private set; } = 0;
        public float TargetValue { get; private set; } = 0;
        public bool IsAnimating { get; private set; } = false;

        private float _currentDuration, _targetDuration;
        private Action _callback, _finishCallback;

        public void Animate(float from, float to, float duration, Action callback = null, Action finishCallback = null)
        {
            IsAnimating = true;
            CurrentValue = from;
            StartValue = from;
            TargetValue = to;
            _targetDuration = duration;
            _currentDuration = 0;
            _callback = callback;
            _finishCallback = finishCallback;
        }

        public void Stop()
        {
            IsAnimating = false;
            CurrentValue = TargetValue;
            _finishCallback?.Invoke();
        }

        public void Update()
        {
            if (IsAnimating)
            {
                if (_currentDuration >= _targetDuration)
                {
                    Stop();
                }
                else
                {
                    CurrentValue = Mathf.SmoothStep(StartValue, TargetValue, _currentDuration / _targetDuration);
                    _callback?.Invoke();
                    _currentDuration += Time.fixedDeltaTime;
                }
            }
        }
    }
}