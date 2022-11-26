using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

namespace TandC.RunIfYouWantToLive
{
	public class AnimationSequence : MonoBehaviour
	{
		public event Action LoopPointReachedEvent;

		public event Action<string, bool> AnimationEndedEvent;

		private float _startTime;

		private Image _uiImage;

		private SpriteRenderer _spriteRenderer;

		public Sprite[] frames;

		public float framesPerSecond;

		public bool loop;

		public int currentFrame;

		public bool wasPaused;

		public bool isPlaying;

		public bool playAtStart;

		public string animationName;

		public int Length => frames.Length;

		private void Awake()
		{
			_uiImage = GetComponent<Image>();
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		private void OnEnable()
		{
			if (playAtStart)
			{
				Play(framesPerSecond, loop, currentFrame);
			}
		}

		private void OnDisable()
		{
			if (playAtStart)
			{
				Stop();
			}
		}

		private void Update()
		{
			if(!gameObject.activeInHierarchy && isPlaying && !wasPaused)
			{
				Stop();
			}

			if (isPlaying && !wasPaused)
			{
				currentFrame = Mathf.CeilToInt((Time.time - _startTime) * framesPerSecond);

				if (currentFrame >= frames.Length)
				{
					LoopPointReachedEvent?.Invoke();

					if (loop)
					{
						currentFrame = 0;
						_startTime = Time.time;
					}
					else
					{
						Stop(false);
						return;
					}
				}

				if (_uiImage != null)
				{
					_uiImage.sprite = frames[currentFrame];
				}

				if (_spriteRenderer != null)
				{
					_spriteRenderer.sprite = frames[currentFrame];
				}
			}
		}

		public void LoadFrames(Sprite[] frames, bool reverse = false)
		{
			if (reverse)
			{
				this.frames = frames.Reverse().ToArray();
			}
			else
			{
				this.frames = frames;
			}
			currentFrame = -1;
		}

		public void Play(float framesPerSecond = 60, bool loop = true, int frame = -1)
		{
			if (frames == null || frames.Length == 0)
				return;

			if (isPlaying)
			{
				Stop();
			}

			this.framesPerSecond = framesPerSecond;
			this.loop = loop;
			this.currentFrame = Mathf.Clamp(frame, -1, frames.Length);

			_startTime = Time.time;

			wasPaused = false;

			isPlaying = true;
		}

		public void Stop(bool force = true)
		{
			if (!isPlaying)
				return;

			isPlaying = false;
			wasPaused = false;

			AnimationEndedEvent?.Invoke(animationName, force);
		}

		public void Pause()
		{
			if (!isPlaying)
				return;

			wasPaused = true;
		}

		public void UnPause()
		{
			if (!isPlaying)
				return;

			wasPaused = false;
		}

		public void ResetAnimation()
		{
			if (frames == null || frames.Length == 0)
				return;

			currentFrame = 0;

			if (_uiImage != null)
			{
				_uiImage.sprite = frames[currentFrame];
			}

			if (_spriteRenderer != null)
			{
				_spriteRenderer.sprite = frames[currentFrame];
			}
		}
	}
}