using System;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
	[CreateAssetMenu(fileName = "LocalizationData", menuName = "TandC/LocalizationData", order = 2)]
	public class LocalizationData : ScriptableObject
	{
		[SerializeField]
		public List<LocalizationLanguageData> languages;

		public Enumerators.Language defaultLanguage;

		public string localizationGoogleSpreadsheet;
		public bool refreshLocalizationAtStart = true;

		[Serializable]
		public class LocalizationLanguageData
		{
			public Enumerators.Language language;
			[SerializeField]
			public List<LocalizationDataInfo> localizedTexts;
		}

		[Serializable]
		public class LocalizationDataInfo
		{
			public string key;
			[TextArea(1, 9999)]
			public string value;
		}
	}

	public class LocalizationSheetData
	{
		public string Keys;
        public string English;
        public string Ukrainian;
        public string Russian;
	}
}