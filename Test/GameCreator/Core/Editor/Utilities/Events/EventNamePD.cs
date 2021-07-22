namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System.Runtime.InteropServices.WindowsRuntime;

    [CustomPropertyDrawer(typeof(EventNameAttribute))]
    public class EventNamePD : PropertyDrawer
    {
        private const int SUGGESTIONS = 4;
        private const string KEY_FMT = "gamecreator-event-name-suggestion-{0}";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect rectText = new Rect(
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            string prevText = property.stringValue;
            string nextText = EditorGUI.DelayedTextField(rectText, label, prevText);

            List<string> suggestions = this.GetSuggestions();

            Rect[] rects = this.GetSuggestionRects(rectText);
            for (int i = 0; i < rects.Length; ++i)
            {
                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(suggestions[i]));
                if (GUI.Button(rects[i], suggestions[i])) nextText = suggestions[i];
                EditorGUI.EndDisabledGroup();
            }

            if (prevText != nextText)
            {
                property.stringValue = nextText;
                this.RecordSuggestion(nextText);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (
                EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing +
                EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing +
                EditorGUIUtility.singleLineHeight
            );
        }

        private Rect[] GetSuggestionRects(Rect rectText)
        {
            float suggestionWidth = (rectText.width - EditorGUIUtility.labelWidth) / 2f;
            Rect rect1 = new Rect(
                rectText.x + EditorGUIUtility.labelWidth,
                rectText.y + rectText.height + EditorGUIUtility.standardVerticalSpacing,
                suggestionWidth,
                EditorGUIUtility.singleLineHeight
            );
            Rect rect2 = new Rect(
                rect1.x + rect1.width + EditorGUIUtility.standardVerticalSpacing,
                rect1.y,
                suggestionWidth - EditorGUIUtility.standardVerticalSpacing,
                EditorGUIUtility.singleLineHeight
            );
            Rect rect3 = new Rect(
                rect1.x,
                rect1.y + rect1.height + EditorGUIUtility.standardVerticalSpacing,
                suggestionWidth,
                EditorGUIUtility.singleLineHeight
            );
            Rect rect4 = new Rect(
                rect3.x + rect3.width + EditorGUIUtility.standardVerticalSpacing,
                rect3.y,
                suggestionWidth - EditorGUIUtility.standardVerticalSpacing,
                EditorGUIUtility.singleLineHeight
            );

            return new Rect[] { rect1, rect2, rect3, rect4 };
        }

        private void RecordSuggestion(string suggestion)
        {
            if (string.IsNullOrEmpty(suggestion)) return;

            List<string> suggestions = this.GetSuggestions();
            if (suggestion.Contains(suggestion))
            {
                suggestions.Remove(suggestion);
            }

            while (suggestions.Count >= SUGGESTIONS)
            {
                suggestions.RemoveAt(suggestions.Count - 1);
            }

            suggestions.Insert(0, suggestion);
            for (int i = 0; i < SUGGESTIONS; ++i)
            {
                EditorPrefs.SetString(string.Format(KEY_FMT, i), suggestions[i]);
            }
        }

        private List<string> GetSuggestions()
        {
            List<string> suggestions = new List<string>();
            for (int i = 0; i < SUGGESTIONS; ++i)
            {
                string suggestion = EditorPrefs.GetString(string.Format(KEY_FMT, i), string.Empty);
                if (!string.IsNullOrEmpty(suggestion)) suggestions.Add(suggestion);
            }

            while (suggestions.Count < SUGGESTIONS)
            {
                suggestions.Add(string.Empty);
            }

            return suggestions;
        }
    }
}