namespace GameCreator.Localization
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AnimatedValues;
	using UnityEditor.SceneManagement;
	using UnityEditorInternal;
	using System.Linq;
	using System.Reflection;
	using GameCreator.Core;

	[CustomEditor(typeof(DatabaseLocalization))]
	public class DatabaseLocalizationEditor : IDatabaseEditor
	{
		private const float LANGUAGES_NAME_WIDTH = 80f;
		private const string MSG_DELETE_TITLE = "Deleting a language will permanently remove all its translations";
		private const string MSG_DELETE_CONTN = "Are you sure you want to continue?";
		private const string MSG_SAVE = "Select where to export Translation file";
		private const string MSG_LOAD = "Select the translation file to load";

		[System.Serializable]
		public class Translations
		{
			[System.Serializable]
			public class Content
			{
				public int key;
				public string text;

				public Content(int key, string text)
				{
					this.key = key;
					this.text = text;
				}
			}

			public SystemLanguage language;
			public List<Content> translations;

			public Translations(SystemLanguage language)
			{
				this.language = language;
				this.translations = new List<Content>();
			}

			public void AddTranslation(int key, string text)
			{
				this.translations.Add(new Content(key, text));
			}
		}

		private static readonly string[] OPTIONS = new string[]
		{
			"Languages",
			"Texts"
		};

		// PROPERTIES: -------------------------------------------------------------------------------------------------
		
		private int option = 0;

		private SerializedProperty spLanguages;
		private SerializedProperty spTranslationList;

		private ReorderableList languages;
		private GUIStyle styleLanguagesPadding;

		private bool stylesInitialized = false;
		private GUIStyle styleLabelText;

		// INITIALIZE: -------------------------------------------------------------------------------------------------

		private void OnEnable()
		{
            if (target == null || serializedObject == null) return;
			this.spLanguages = serializedObject.FindProperty("languages");
			this.spTranslationList = serializedObject.FindProperty("translationList");

			this.languages = new ReorderableList(
				serializedObject, 
				this.spLanguages,
				true, false, true, true
			);

			this.languages.drawElementCallback = this.DrawLanguagesElement;
			this.languages.onCanRemoveCallback = this.DrawLanguageCanRemove;
			this.languages.onRemoveCallback = this.DrawLanguageRemove;
			this.languages.onAddDropdownCallback = this.DrawLanguageAdd;

			this.styleLanguagesPadding = new GUIStyle();
			this.styleLanguagesPadding.padding = new RectOffset(5,5,2,2);
		}

		// OVERRIDE METHODS: -------------------------------------------------------------------------------------------

		public override string GetDocumentationURL ()
		{
			return "https://docs.gamecreator.io/manual/localization.html";
		}

		public override string GetName ()
		{
			return "Localization";
		}

        public override bool CanBeDecoupled()
        {
            return true;
        }

        // GUI METHODS: ------------------------------------------------------------------------------------------------

        public override void OnInspectorGUI()
		{
			if (!this.stylesInitialized)
			{
				this.InitializeStyles();
				this.stylesInitialized = true;
			}

            this.serializedObject.Update();

			this.option = GUILayout.Toolbar(this.option, OPTIONS);
			switch (this.option)
			{
			case 0 : this.PaintLanguages(); break;
			case 1 : this.PaintTexts(); break;
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		// LANGUAGES: --------------------------------------------------------------------------------------------------

		private void PaintLanguages()
		{
			EditorGUILayout.BeginVertical(this.styleLanguagesPadding);
			this.languages.DoLayoutList();
			EditorGUILayout.EndVertical();
		}

		private void DrawLanguagesElement(Rect rect, int index, bool active, bool focus)
		{
			Rect labelRect = new Rect(
				rect.x, 
				rect.y + (rect.height/2.0f) - (EditorGUIUtility.singleLineHeight/2.0f),
				LANGUAGES_NAME_WIDTH,
				EditorGUIUtility.singleLineHeight
			);

			SerializedProperty property = this.languages.serializedProperty.GetArrayElementAtIndex(index);
			string label = ((SystemLanguage)property.FindPropertyRelative("language").intValue).ToString();
			EditorGUI.LabelField(labelRect, label);

			Rect restRect = new Rect(
				rect.x + labelRect.width,
				rect.y + (rect.height/2.0f) - (EditorGUIUtility.singleLineHeight/2.0f),
				rect.width - labelRect.width,
				EditorGUIUtility.singleLineHeight
			);

			this.DrawLanguagesElementData(restRect, index);
		}

		private void DrawLanguagesElementData(Rect rect, int index)
		{
			Rect labelRect = new Rect(rect.x, rect.y, rect.width - (2 * LANGUAGES_NAME_WIDTH), rect.height);

			if (index == 0)
			{
				EditorGUI.LabelField(labelRect, "Main Language", EditorStyles.boldLabel);
			}
			else
			{
				string update = this.spLanguages.GetArrayElementAtIndex(index).FindPropertyRelative("updateDate").stringValue;
				if (!string.IsNullOrEmpty(update))
				{
					update = string.Format("Last Update: {0}", update);
					EditorGUI.LabelField(labelRect, update);
				}
			}

			EditorGUI.BeginDisabledGroup(index == 0);

			Rect btnRect1 = new Rect(
				rect.x + (rect.width - 2 * LANGUAGES_NAME_WIDTH), rect.y, 
				LANGUAGES_NAME_WIDTH, rect.height
			);

			Rect btnRect2 = new Rect(
				rect.x + (rect.width - 1 * LANGUAGES_NAME_WIDTH), rect.y, 
				LANGUAGES_NAME_WIDTH, rect.height
			);

			if (GUI.Button(btnRect1, "Import...", EditorStyles.miniButtonLeft))
			{
				this.ImportLanguage(index);
			}

			if (GUI.Button(btnRect2, "Export...", EditorStyles.miniButtonRight))
			{
				this.ExportLanguage(index);
			}

			EditorGUI.EndDisabledGroup();
		}

		private bool DrawLanguageCanRemove(ReorderableList list)
		{
			return list.serializedProperty.arraySize > 1;
		}

		private void DrawLanguageRemove(ReorderableList list)
		{
			if (list.index < 0 || list.index >= list.count) return;

			if (EditorUtility.DisplayDialog(MSG_DELETE_TITLE, MSG_DELETE_CONTN, "Yes", "Cancel"))
			{
				list.serializedProperty.DeleteArrayElementAtIndex(list.index);
				serializedObject.ApplyModifiedProperties();
			}
		}

		private void DrawLanguageAdd(Rect buttonRect, ReorderableList list)
		{
			GenericMenu menu = new GenericMenu();
			foreach (SystemLanguage language in Enum.GetValues(typeof(SystemLanguage)))
			{
				int languagesSize = this.spLanguages.arraySize;
				bool languageFound = false;
				for (int i = 0; !languageFound && i < languagesSize; ++i)
				{
					if (this.spLanguages.GetArrayElementAtIndex(i).FindPropertyRelative("language").intValue == (int)language)
					{
						menu.AddDisabledItem(new GUIContent(language.ToString()));
						languageFound = true;
					}
				}

				if (!languageFound)
				{
					menu.AddItem(
						new GUIContent(language.ToString()), false,
						this.AddLanguage, language
					);
				}
			}

			menu.ShowAsContext();
		}

		// TEXTS METHODS: ----------------------------------------------------------------------------------------------

		private const string MSG_DELETE = "Delete";
		private const string MSG_TEXTS = "List of all the Localized texts. Delete those unused from here.";

		private void PaintTexts()
		{
			EditorGUILayout.HelpBox(MSG_TEXTS, MessageType.Info);
			int translationsCount = this.spTranslationList.arraySize;
			int removeIndex = -1;

			for (int i = 0; i < translationsCount; ++i)
			{
				EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

				SerializedProperty spText = this.spTranslationList.GetArrayElementAtIndex(i);
				EditorGUILayout.LabelField(spText.FindPropertyRelative("placeholder").stringValue, this.styleLabelText);

				if (GUILayout.Button(MSG_DELETE, EditorStyles.miniButton, GUILayout.Width(50f)))
				{
					removeIndex = i;
				}

				EditorGUILayout.EndHorizontal();
			}

			if (removeIndex >= 0 && removeIndex < translationsCount)
			{
				this.spTranslationList.DeleteArrayElementAtIndex(removeIndex);
				serializedObject.ApplyModifiedProperties();
				serializedObject.Update();
			}
		}

		// PRIVATE METHODS: --------------------------------------------------------------------------------------------

		private void ExportLanguage(int index)
		{
			SerializedProperty spLanguages = this.languages.serializedProperty.GetArrayElementAtIndex(index);
			int language = spLanguages.FindPropertyRelative("language").intValue;
			int size = this.spTranslationList.arraySize;
			Translations translations = new Translations((SystemLanguage)language);

			for (int i = 0; i < size; ++i)
			{
				SerializedProperty property = this.spTranslationList.GetArrayElementAtIndex(i);
				int key = property.FindPropertyRelative("key").intValue;

				if (key == 0) continue;

				bool translationFound = false;
				SerializedProperty content = property.FindPropertyRelative("content");
				for (int j = 0; !translationFound && j < content.arraySize; ++j)
				{
					SerializedProperty contentItem = content.GetArrayElementAtIndex(j);
					if (contentItem.FindPropertyRelative("language").intValue == language)
					{
						string text = contentItem.FindPropertyRelative("text").stringValue;
						if (!string.IsNullOrEmpty(text))
						{
							translationFound = true;
							translations.AddTranslation(key, text);
						}
					}
				}

				if (!translationFound)
				{
					string placeholder = property.FindPropertyRelative("placeholder").stringValue;
					translations.AddTranslation(key, placeholder);
				}
			}

			string fileName = string.Format("{0}.json", ((SystemLanguage)language).ToString());
			string savePath = EditorUtility.SaveFilePanel(MSG_SAVE, "", fileName, "json"); 

			if (string.IsNullOrEmpty(savePath))
			{
				EditorApplication.Beep();
				return;
			}

			File.WriteAllText(savePath, EditorJsonUtility.ToJson(translations, true));
		}

		private void ImportLanguage(int index)
		{
			string filePath = EditorUtility.OpenFilePanelWithFilters(MSG_LOAD, "", new string[]{"JSON", "json"});
			if (string.IsNullOrEmpty(filePath)) 
			{
				EditorApplication.Beep();
				return;
			}

			string json = File.ReadAllText(filePath);
			Translations translations = JsonUtility.FromJson<Translations>(json);

			if (translations == null)
			{
				Debug.LogErrorFormat("Unable to parse translation file {0}: {1}", filePath, json);
				EditorApplication.Beep();
				return;
			}

			int languagesCount = this.spLanguages.arraySize;
			for (int i = 0; i < languagesCount; ++i)
			{
				SerializedProperty language = this.spLanguages.GetArrayElementAtIndex(i);
				if (language.FindPropertyRelative("language").intValue == (int)translations.language)
				{
					language.FindPropertyRelative("updateDate").stringValue = DateTime.Now.ToString("g");
				}
			}

			int translationsCount = translations.translations.Count;
			for (int i = 0; i < translationsCount; ++i)
			{
				int key = translations.translations[i].key;
				string text = translations.translations[i].text;

				int size = this.spTranslationList.arraySize;
				for (int j = 0; j < size; ++j)
				{
					SerializedProperty item = this.spTranslationList.GetArrayElementAtIndex(j);
					if (item.FindPropertyRelative("key").intValue == key)
					{
						SerializedProperty itemContent = item.FindPropertyRelative("content");
						int itemContentSize = itemContent.arraySize;
						bool languageFound = false;
						for (int k = 0; k < itemContentSize; ++k)
						{
							SerializedProperty itemContentK = itemContent.GetArrayElementAtIndex(k);
							if (itemContentK.FindPropertyRelative("language").intValue == (int)translations.language)
							{
								languageFound = true;
								itemContentK.FindPropertyRelative("text").stringValue = text;
							}
						}

						if (!languageFound)
						{
							itemContent.InsertArrayElementAtIndex(itemContentSize);
							SerializedProperty newItem = itemContent.GetArrayElementAtIndex(itemContentSize);
							newItem.FindPropertyRelative("language").intValue = (int)translations.language;
							newItem.FindPropertyRelative("text").stringValue = text;
						}
					}
				}
			}
		}

		private void AddLanguage(object data)
		{
			int addIndex = this.spLanguages.arraySize;
			string date = DateTime.Now.ToString("g");

			this.spLanguages.InsertArrayElementAtIndex(addIndex);
			this.spLanguages.GetArrayElementAtIndex(addIndex).FindPropertyRelative("language").intValue = (int)data;
			this.spLanguages.GetArrayElementAtIndex(addIndex).FindPropertyRelative("updateDate").stringValue = date;

			this.languages.index = addIndex;

			this.serializedObject.ApplyModifiedProperties();
			this.serializedObject.Update();
		}

		private void InitializeStyles()
		{
			this.styleLabelText = new GUIStyle(EditorStyles.label);
			this.styleLabelText.wordWrap = true;
			this.styleLabelText.richText = true;
		}

		// PUBLIC METHODS: ---------------------------------------------------------------------------------------------

		public void AddTranslation(int key, string placeholder = "")
		{
			this.OnEnable();
			int index = this.spTranslationList.arraySize;
			this.spTranslationList.InsertArrayElementAtIndex(index);
			SerializedProperty spTranslationElem = this.spTranslationList.GetArrayElementAtIndex(index);

			spTranslationElem.FindPropertyRelative("key").intValue = key;
			spTranslationElem.FindPropertyRelative("placeholder").stringValue = placeholder;

			serializedObject.ApplyModifiedProperties();
			serializedObject.Update();
		}

		public void RemoveTranslation(int key)
		{
			this.OnEnable();
			int size = this.spTranslationList.arraySize;

			for (int i = size - 1; i >= 0; --i)
			{
				if (this.spTranslationList.GetArrayElementAtIndex(i).FindPropertyRelative("key").intValue == key)
				{
					this.spTranslationList.DeleteArrayElementAtIndex(i);
				}
			}

			serializedObject.ApplyModifiedProperties();
			serializedObject.Update();
		}

		public SerializedProperty GetPlaceholder(int key)
		{
			int translationsSize = this.spTranslationList.arraySize;
			for (int i = 0; i < translationsSize; ++i)
			{
				SerializedProperty item = this.spTranslationList.GetArrayElementAtIndex(i);
				if (item.FindPropertyRelative("key").intValue == key)
				{
					return item.FindPropertyRelative("placeholder");
				}
			}

			return null;
		}
	}
}