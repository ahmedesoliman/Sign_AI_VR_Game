namespace GameCreator.Localization
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;

	[AddComponentMenu("Game Creator/Managers/LocalizationManager", 100)]
	public class LocalizationManager : Singleton<LocalizationManager>, IGameSave
	{
		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private DatabaseLocalization databaseLocalization;
		private SystemLanguage currentLanguage;

		public UnityAction onChangeLanguage;

		// INITIALIZER: ------------------------------------------------------------------------------------------------

		protected override void OnCreate ()
		{
			this.databaseLocalization = DatabaseLocalization.LoadDatabaseCopy<DatabaseLocalization>();
			this.currentLanguage = databaseLocalization.GetMainLanguage();

			SaveLoadManager.Instance.Initialize(this);
		}

		public SystemLanguage GetCurrentLanguage()
		{
			return this.currentLanguage;
		}

		public void ChangeLanguage(SystemLanguage language)
		{
			int languagesCount = this.databaseLocalization.languages.Count;
			for (int i = 0; i < languagesCount; ++i)
			{
				if (this.databaseLocalization.languages[i].language == language)
				{
					this.currentLanguage = language;
					if (this.onChangeLanguage != null) this.onChangeLanguage.Invoke();

					return;
				}
			}

			Debug.LogWarningFormat("Language {0} not available", language);
		}

		// INTERFACE ISAVELOAD: ----------------------------------------------------------------------------------------

		public string GetUniqueName()
		{
			return "localization";
		}

		public System.Type GetSaveDataType()
		{
			return typeof(SystemLanguage);
		}

		public System.Object GetSaveData()
		{
			return currentLanguage;
		}

		public void ResetData()
		{
			this.currentLanguage = this.databaseLocalization.GetMainLanguage();
		}

		public void OnLoad(System.Object generic)
		{
			SystemLanguage language = (SystemLanguage)generic;
			this.currentLanguage = language;
		}
	}
}