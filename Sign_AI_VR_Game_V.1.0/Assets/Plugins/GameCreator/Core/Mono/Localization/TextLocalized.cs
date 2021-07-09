namespace GameCreator.Localization
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using GameCreator.Core;

    [AddComponentMenu("Game Creator/UI/Text (Localized)", 20)]
	public class TextLocalized : Text
	{
		[LocStringNoTextAttribute]
		public LocString locString = new LocString("My Text...");

        private bool exitingApplication = false;

        // INITIALIZERS: --------------------------------------------------------------------------

		protected override void Awake()
		{
			base.Awake();
			if (Application.isPlaying)
			{
				this.UpdateText();
				LocalizationManager.Instance.onChangeLanguage += this.UpdateText;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (Application.isPlaying)
			{
				LocalizationManager.Instance.onChangeLanguage += this.UpdateText;
				this.UpdateText();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
            if (Application.isPlaying && !this.exitingApplication)
			{
				if (LocalizationManager.Instance == null) return;
				LocalizationManager.Instance.onChangeLanguage -= this.UpdateText;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
            if (Application.isPlaying && !this.exitingApplication)
			{
				if (LocalizationManager.Instance == null) return;
				LocalizationManager.Instance.onChangeLanguage -= this.UpdateText;
			}
		}

        private void OnApplicationQuit()
        {
            this.exitingApplication = true;
        }

        // MAIN METHODS: --------------------------------------------------------------------------

        public void ChangeKey(string textKey)
		{
			this.locString.content = textKey;
			this.UpdateText();
		}

		private void UpdateText()
		{
			if (string.IsNullOrEmpty(this.locString.content)) return;
			this.text = this.locString.GetText();
		}
	}
}