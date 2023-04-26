using TandC.RunIfYouWantToLive.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
	[CreateAssetMenu(fileName = "SoundData", menuName = "TandC/SoundData", order = 1)]
	public class SoundData : ScriptableObject
	{
		[SerializeField]
		public List<SoundInfo> sounds;
		public float crossFadeInTime = 2f;
		public float crossFadeOutTime = 1f;


		[Serializable]
		public class SoundInfo
		{
			public Enumerators.SoundType type;
			public AudioClip clip;
			public float volume = 1f;
			public bool loop;
			public bool sfx = true;
			public bool crossFade;
		}
	}
}