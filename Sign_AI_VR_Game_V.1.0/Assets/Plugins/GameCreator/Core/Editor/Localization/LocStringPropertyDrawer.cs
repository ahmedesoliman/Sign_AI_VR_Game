namespace GameCreator.Localization
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using GameCreator.Core;

	[CustomPropertyDrawer (typeof (LocString))]
	public class LocStringPropertyDrawer : PropertyDrawer 
	{
        protected const float TRANSLATION_BUTTON_WIDTH = 25f;
        protected const float HORIZONTAL_SEPARATION = -1f;
        protected static readonly GUIContent GUICONTENT_TRANSLATION = new GUIContent("さ", "Enable to add a translation slot");

		protected SerializedProperty spDatabasePlaceholder;

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
            property.serializedObject.ApplyModifiedProperties();
			property.serializedObject.Update();

			SerializedProperty spContent = property.FindPropertyRelative("content");
			SerializedProperty spPostProcess = property.FindPropertyRelative("postProcess");
			SerializedProperty spTranslationID = property.FindPropertyRelative("translationID");

			Rect spContentRect = new Rect(
				position.x, position.y, 
				position.width - TRANSLATION_BUTTON_WIDTH - HORIZONTAL_SEPARATION, 
				position.height
			);

			Rect spTranslationIDRect = new Rect(
				spContentRect.x + spContentRect.width + HORIZONTAL_SEPARATION, position.y, 
				TRANSLATION_BUTTON_WIDTH, position.height
			);

			EditorGUI.PropertyField(spContentRect, spContent, label);

			GUIStyle translationStyle = (spTranslationID.intValue == 0
				? CoreGUIStyles.GetGridButtonRightOff()
				: CoreGUIStyles.GetGridButtonRightOn()
			);
				
			if (GUI.Button(spTranslationIDRect, GUICONTENT_TRANSLATION, translationStyle))
			{
				if (spTranslationID.intValue == 0)
				{
					spTranslationID.intValue = LocStringPropertyDrawer.GenerateID();

					Editor editorLocalization = DatabaseLocalizationEditor.CreateEditor(DatabaseLocalization.Load());
					((DatabaseLocalizationEditor)editorLocalization).AddTranslation(spTranslationID.intValue, spContent.stringValue);
				}
				else
				{
					if (spTranslationID.intValue != 0)
					{
						Editor editorLocalization = DatabaseLocalizationEditor.CreateEditor(DatabaseLocalization.Load());
						((DatabaseLocalizationEditor)editorLocalization).RemoveTranslation(spTranslationID.intValue);
					}

					spTranslationID.intValue = 0;
				}
			}

			if (spTranslationID.intValue != 0)
			{
				if (this.spDatabasePlaceholder == null)
				{
					Editor editorLocalization = DatabaseLocalizationEditor.CreateEditor(DatabaseLocalization.Load());
					this.spDatabasePlaceholder = ((DatabaseLocalizationEditor)editorLocalization).GetPlaceholder(
						spTranslationID.intValue
					);
				}

				if (this.spDatabasePlaceholder != null)
				{
					this.spDatabasePlaceholder.stringValue = spContent.stringValue;
					this.spDatabasePlaceholder.serializedObject.ApplyModifiedProperties();
				}
			}

            if (this.PaintPostProcess())
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spPostProcess);
                EditorGUI.indentLevel--;
            }

			property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
		}

        protected virtual bool PaintPostProcess()
        {
            return true;
        }

		// STATIC METHODS: ---------------------------------------------------------------------------------------------

		public static int GenerateID()
		{
			return Guid.NewGuid().GetHashCode();
		}
	}
}