namespace GameCreator.Variables
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using GameCreator.Core;

    [CustomEditor(typeof(GlobalTags))]
    public class GlobalTagsEditor : Editor
    {
        private const int MAX_TAGS = 32;
        private const float COLOR_PROP_WIDTH = 80f;
        private const float LABEL_PROP_WIDTH = 50f;
        private const float PADDING = 2f;
        private const string TITLE_FORMAT = "Tag {0}:";

        private static GlobalTags GLOBAL_TAGS_INSTANCE;

        public const string PATH_ASSET = "Assets/Plugins/GameCreatorData/Core/Variables/";
        public const string NAME_ASSET = "Tags.asset";

        [Serializable]
        private class SerializedPropertyTag
        {
            public SerializedProperty spName;
            public SerializedProperty spColor;

            public SerializedPropertyTag(SerializedProperty spName, SerializedProperty spColor)
            {
                this.spName = spName;
                this.spColor = spColor;
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTags;
        private SerializedPropertyTag[] spTagElements;

		// INITIALIZERS: --------------------------------------------------------------------------

		private void OnEnable()
		{
            this.spTags = serializedObject.FindProperty("tags");
            if (this.spTags.arraySize != MAX_TAGS)
            {
                this.spTags.arraySize = MAX_TAGS;
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }

            this.spTagElements = new SerializedPropertyTag[MAX_TAGS];
            for (int i = 0; i < MAX_TAGS; ++i)
            {
                SerializedProperty spTag = this.spTags.GetArrayElementAtIndex(i);
                this.spTagElements[i] = new SerializedPropertyTag(
                    spTag.FindPropertyRelative("name"),
                    spTag.FindPropertyRelative("color")
                );
            }
		}

		// PAINT METHODS: -------------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
            serializedObject.Update();

            for (int i = 0; i < MAX_TAGS; ++i)
            {
                this.PaintTag(
                    i,
                    this.spTagElements[i].spName,
                    this.spTagElements[i].spColor
                );
            }

            serializedObject.ApplyModifiedProperties();
		}

        private void PaintTag(int index, SerializedProperty spName, SerializedProperty spColor)
        {
            Rect rect = GUILayoutUtility.GetRect(
                EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth,
                EditorGUIUtility.singleLineHeight
            );
            Rect rectLabel = new Rect(
                rect.x,
                rect.y,
                LABEL_PROP_WIDTH,
                rect.height
            );
            Rect rectName = new Rect(
                rectLabel.x + rectLabel.width + PADDING,
                rectLabel.y,
                rect.width - rectLabel.width - COLOR_PROP_WIDTH - PADDING,
                rectLabel.height
            );
            Rect rectColor = new Rect(
                rectName.x + rectName.width + PADDING,
                rectName.y,
                COLOR_PROP_WIDTH - PADDING,
                rectName.height
            );

            EditorGUI.LabelField(rectLabel, new GUIContent(string.Format(TITLE_FORMAT, index)));
            EditorGUI.PropertyField(rectName, spName, GUIContent.none);
            spColor.intValue = EditorGUI.Popup(rectColor, spColor.intValue, Tag.COLOR_NAMES);
            GUILayout.Space(PADDING);
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static Tag[] GetTags()
        {
            if (GLOBAL_TAGS_INSTANCE == null)
            {
                GLOBAL_TAGS_INSTANCE = AssetDatabase.LoadAssetAtPath<GlobalTags>(Path.Combine(
                    GlobalTagsEditor.PATH_ASSET,
                    GlobalTagsEditor.NAME_ASSET
                ));
            }

            if (GLOBAL_TAGS_INSTANCE == null) return new Tag[0];
            return GLOBAL_TAGS_INSTANCE.tags;
        }

        public static string[] GetTagNames()
        {
            Tag[] tagInstances = GlobalTagsEditor.GetTags();
            if (tagInstances.Length == 0) return new string[0];

            string[] tags = new string[tagInstances.Length];
            for (int i = 0; i < tagInstances.Length; ++i)
            {
                tags[i] = tagInstances[i].name;
            }

            return tags;
        }
	}
}