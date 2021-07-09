namespace GameCreator.Localization
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

	public class DatabaseLocalization : IDatabase
	{
		[System.Serializable]
		public class TranslationContent
		{
			public SystemLanguage language;
			public string text;
		}

		[System.Serializable]
		public class TranslationStrings
		{
			public int key;
			public string placeholder;
			public List<TranslationContent> content;

			public TranslationStrings(int key)
			{
				this.key = key;
				this.placeholder = "";
				this.content = new List<TranslationContent>();
			}
		}

		[System.Serializable]
		public class TranslationLanguage
		{
			public SystemLanguage language;
			public string updateDate;
		}

		// PROPERTIES: ----------------------------------------------------------------------------

		private Dictionary<int, Dictionary<SystemLanguage, string>> _content;

		public List<TranslationLanguage> languages = new List<TranslationLanguage>();
		public List<TranslationStrings> translationList = new List<TranslationStrings>();

		// PUBLIC METHODS: ------------------------------------------------------------------------

		private void BuildContentDictionary()
		{
			this._content = new Dictionary<int, Dictionary<SystemLanguage, string>>();

			int translationListCount = this.translationList.Count;
			for (int i = 0; i < translationListCount; ++i)
			{
				int key = this.translationList[i].key;
				this._content.Add(key, new Dictionary<SystemLanguage, string>());

				int contentCount = this.translationList[i].content.Count;
				for (int j = 0; j < contentCount; ++j)
				{
					SystemLanguage language = this.translationList[i].content[j].language;
					string text = this.translationList[i].content[j].text;
					this._content[key].Add(language, text);
				}
			}
		}

		public string GetText(LocString locString, SystemLanguage language = SystemLanguage.Unknown)
		{
			if (this._content == null) this.BuildContentDictionary();

			if (locString.translationID == 0) return locString.content;
			if (language == SystemLanguage.Unknown) language = this.GetMainLanguage();
			if (language == this.GetMainLanguage()) return locString.content;

			if (!this._content.ContainsKey(locString.translationID)) 
			{
				Debug.LogWarningFormat("Can't find localization key {0}", locString.translationID);
				return locString.content;
			}

			if (!this._content[locString.translationID].ContainsKey(language))
			{
				Debug.LogWarningFormat("Can't find localization language {0} for key {1}", language, locString.translationID);
				return locString.content;
			}

			return this._content[locString.translationID][language];
		}

		public SystemLanguage GetMainLanguage()
		{
			if (this.languages.Count > 0) return this.languages[0].language;
			return SystemLanguage.Unknown;
		}

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static DatabaseLocalization Load()
        {
            return IDatabase.LoadDatabase<DatabaseLocalization>();
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        #if UNITY_EDITOR

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            IDatabase.Setup<DatabaseLocalization>();
        }

        public override int GetSidebarPriority()
        {
            return 2;
        }

        #endif
    }
}