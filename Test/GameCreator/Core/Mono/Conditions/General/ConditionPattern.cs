using System.Runtime.CompilerServices;
namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

    #if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;
    #endif

    [AddComponentMenu("")]
	public class ConditionPattern : ICondition
	{
        public enum Value
        {
            True,
            False
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public List<Value> pattern = new List<Value>()
        {
            Value.True,
            Value.False
        };

        private int patternIndex = 0;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check()
        {
            if (this.pattern.Count == 0) return false;
            bool result = this.pattern[this.patternIndex] == Value.True;

            this.patternIndex = ++this.patternIndex >= this.pattern.Count ? 0 : this.patternIndex;
            return result;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "General/Pattern";
        private const string NODE_TITLE = "Follow Pattern";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spPattern;
        private ReorderableList list;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        protected override void OnEnableEditorChild ()
        {
            this.spPattern = this.serializedObject.FindProperty("pattern");
            this.list = new ReorderableList(
                this.serializedObject, this.spPattern, 
                true, true, true, true
            );

            this.list.drawHeaderCallback += PaintHeader;
            this.list.drawElementCallback += PaintElement;
        }

        private void PaintHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Pattern");
        }

        private void PaintElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty property = this.spPattern.GetArrayElementAtIndex(index);
            Rect rectElement = new Rect(
                rect.x,
                rect.y + 1f,
                rect.width,
                rect.height - 2f
            );

            EditorGUI.PropertyField(
                rectElement,
                property,
                new GUIContent(index.ToString())
            );
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            this.list.DoLayoutList();

            this.serializedObject.ApplyModifiedProperties();
        }

        #endif
    }
}
