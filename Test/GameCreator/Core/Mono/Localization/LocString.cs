namespace GameCreator.Localization
{
	using System;
	using System.Text;
	using System.Globalization;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[System.Serializable]
	public class LocString
	{
		public enum PostProcess
		{
			None,
			FirstUppercase,
			TitleCase,
			AllUppercase,
			AllLowercase
		}

		// PROPERTIES: ----------------------------------------------------------------------------

		private static DatabaseLocalization LOCAL_DATABASE;

		public string content = "";
		public PostProcess postProcess = PostProcess.None;
		public int translationID = 0;

        // INITIALIZERS: --------------------------------------------------------------------------

        public LocString()
        {
            this.content = "";
        }

        public LocString(string content)
        {
            this.content = content;
        }

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public string GetText()
		{
			if (string.IsNullOrEmpty(this.content)) return "";
			if (LOCAL_DATABASE == null) LOCAL_DATABASE = DatabaseLocalization.Load();

			string content = LOCAL_DATABASE.GetText(this, LocalizationManager.Instance.GetCurrentLanguage());
			return this.PostProcessContent(content);
		}

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private string PostProcessContent(string content)
		{
			if (string.IsNullOrEmpty(content)) return content;

			content = content.Replace("\\n", "\n");
			content = content.Replace("\\\"", "\"");

			switch (this.postProcess)
			{
			case PostProcess.TitleCase :
				content = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(content);
				break;

			case PostProcess.AllLowercase :
				content = CultureInfo.InvariantCulture.TextInfo.ToLower(content);
				break;

			case PostProcess.AllUppercase :
				content = CultureInfo.InvariantCulture.TextInfo.ToUpper(content);
				break;

			case PostProcess.FirstUppercase :
				int firstCharIndex = this.GetFirstCharacterIndex(content);
				if (firstCharIndex == -1) break;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder
					.Append(content.Substring(0, firstCharIndex))
					.Append(CultureInfo.InvariantCulture.TextInfo.ToUpper(content[firstCharIndex]))
					.Append(content.Substring(firstCharIndex + 1));
				break;
			}

			return content;
		}

		private int GetFirstCharacterIndex(string input) 
		{
			for (int i = 0; i < input.Length; ++i)
			{
				if (char.IsLetter(input[i])) return i;
			}
			 
			return -1;
		}
	}
}