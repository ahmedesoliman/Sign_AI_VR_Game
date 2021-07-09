namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public abstract class TargetGenericPD : PropertyDrawer
    {
        public const string PROP_TARGET = "target";

        private static readonly GUIContent GUICONTENT_EMPTY = new GUIContent(" ");

        // ABSTRACT & VIRTUAL METHODS: ------------------------------------------------------------

        protected abstract SerializedProperty GetProperty(int option, SerializedProperty property);

        protected virtual SerializedProperty GetExtraProperty(int option, SerializedProperty property) 
        {
            return null;
        }

        protected virtual void Initialize(SerializedProperty property)
        {
            return;
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool init = false;

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (!this.init)
            {
                this.init = true;
                this.Initialize(property);
            }

            SerializedProperty spTarget = property.FindPropertyRelative(PROP_TARGET);

            Rect rect = new Rect(
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            EditorGUI.PropertyField(rect, spTarget, label);
            SerializedProperty spValue = this.GetProperty(spTarget.intValue, property);
            if (spValue != null)
            {
                rect = new Rect(
                    position.x,
                    rect.y + rect.height + EditorGUIUtility.standardVerticalSpacing,
                    position.width,
                    EditorGUI.GetPropertyHeight(spValue)
                );

                EditorGUI.PropertyField(rect, spValue, GUICONTENT_EMPTY);
            }

            SerializedProperty spExtra = this.GetExtraProperty(spTarget.intValue, property);
            if (spExtra != null)
            {
                rect = new Rect(
                    position.x,
                    rect.y + rect.height + EditorGUIUtility.standardVerticalSpacing,
                    position.width,
                    EditorGUI.GetPropertyHeight(spExtra)
                );

                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(rect, spExtra);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty spTarget = property.FindPropertyRelative(PROP_TARGET);
            float height = EditorGUI.GetPropertyHeight(spTarget);

            SerializedProperty spValue = this.GetProperty(spTarget.intValue, property);
            if (spValue != null)
            {
                height += (
                    EditorGUI.GetPropertyHeight(spValue) +
                    EditorGUIUtility.standardVerticalSpacing
                );
            }

            SerializedProperty spExtra = this.GetExtraProperty(spTarget.intValue, property);
            if (spExtra != null)
            {
                height += (
                    EditorGUI.GetPropertyHeight(spExtra) +
                    EditorGUIUtility.standardVerticalSpacing
                );
            }

            return height;
        }
    }
}