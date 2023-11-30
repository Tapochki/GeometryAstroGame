using TMPro;
using UnityEngine;

namespace TandC.RunIfYouWantToLive.Helpers
{
    public class FPSCounter
    {
        private int _frameLimit = 500;

        private int[] _fpsBuffer;
        private int _fpsIndex;

        private TextMeshProUGUI _fpsText;

        public FPSCounter(TextMeshProUGUI fpsText)
        {
            _fpsText = fpsText;
        }

        public int AvarageFPS { get; private set; }

        public void Update()
        {
            if (_fpsBuffer == null || _frameLimit != _fpsBuffer.Length)
                InitBuffer();

            UpdateBuffer();
            CalculateFPS();

            _fpsText.text = Mathf.Clamp(AvarageFPS, 0, _frameLimit).ToString();
        }

        private void InitBuffer()
        {
            if (_frameLimit <= 0)
                _frameLimit = 1;

            _fpsBuffer = new int[_frameLimit];
            _fpsIndex = 0;
        }

        private void UpdateBuffer()
        {
            _fpsBuffer[_fpsIndex++] = (int)(1f / Time.unscaledDeltaTime);

            if (_fpsIndex >= _frameLimit)
                _fpsIndex = 0;
        }

        private void CalculateFPS()
        {
            int sum = 0;

            for (int i = 0; i < _frameLimit; i++)
            {
                int fps = _fpsBuffer[i];
                sum += fps;
            }

            AvarageFPS = sum / _frameLimit;
        }
    }
}