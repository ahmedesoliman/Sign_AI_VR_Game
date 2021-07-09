namespace GameCreator.Localization
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using GameCreator.Core;

	[CustomPropertyDrawer (typeof (LocStringBigTextAttribute))]
	public class LocStringBigTextPropertyDrawer : PropertyDrawer
	{
		private const float TRANSLATION_BUTTON_WIDTH = 25f;
		private const float HORIZONTAL_SEPARATION = -1f;
		private static readonly GUIContent GUICONTENT_TRANSLATION = new GUIContent(
			"Enable Translation",
			"Enable to add a translation slot"
		);

		private SerializedProperty spDatabasePlaceholder;
        private GUIStyle textAreaStyle;

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();

            SerializedProperty spContent = property.FindPropertyRelative("content");
            SerializedProperty spPostProcess = property.FindPropertyRelative("postProcess");
            SerializedProperty spTranslationID = property.FindPropertyRelative("translationID");

            Rect spTranslationIDRect = new Rect(
                position.x, position.y,
                position.width, EditorGUIUtility.singleLineHeight
            );

            bool translationOn = spTranslationID.intValue != 0;
            bool nextTranslationOn = EditorGUI.Toggle(spTranslationIDRect, GUICONTENT_TRANSLATION, translationOn);
            if (this.PaintPostProcess()) EditorGUILayout.PropertyField(spPostProcess);

            if (this.textAreaStyle == null)
            {
                this.textAreaStyle = new GUIStyle();
                this.textAreaStyle.margin = new RectOffset(5, 5, 0, 0);
                this.textAreaStyle.wordWrap = true;
            }

            Rect textAreaRect = GUILayoutUtility.GetRect(
                EditorGUIUtility.fieldWidth, EditorGUIUtility.fieldWidth,
                EditorGUIUtility.singleLineHeight * 3f,
                EditorGUIUtility.singleLineHeight * 3f,
                textAreaStyle
            );

            spContent.stringValue = EditorGUI.TextArea(
                textAreaRect,
                spContent.stringValue
            );

            if (translationOn != nextTranslationOn)
            {
                if (nextTranslationOn)
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

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
		}

        protected virtual bool PaintPostProcess()
        {
            return true;
        }

		// STATIC METHODS: ---------------------------------------------------------------------------------------------

		public static string GenerateID()
		{
			return Guid.NewGuid().ToString("N");
		}
	}
}
