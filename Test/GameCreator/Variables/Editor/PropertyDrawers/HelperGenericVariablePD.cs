namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.IMGUI;
    using UnityEditor.IMGUI.Controls;

    public abstract class HelperGenericVariablePD : PropertyDrawer
    {
        public const string PROP_ALLOW_TYPES_MASK = "allowTypesMask";
        protected const string PROP_NAME = "name";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spName;
        protected SerializedProperty spAllowTypesMask;

		// PAINT METHODS: -------------------------------------------------------------------------

		protected void PaintVariables(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            this.spName = property;
            Rect rectLabel = this.GetRectLabel(position);
            Rect rectField = this.GetRectField(position);

            EditorGUI.PrefixLabel(rectLabel, label);
            if (EditorGUI.DropdownButton(rectField, new GUIContent(property.stringValue), FocusType.Passive))
            {
                GenericVariableSelectWindow window = this.GetWindow(rectField);
                if (window != null) PopupWindow.Show(rectField, window);
            }

            EditorGUI.EndProperty();
        }

        // VIRTUAL AND ABSTRACT METHODS: ----------------------------------------------------------

        protected abstract GenericVariableSelectWindow GetWindow(Rect ctaRect);

        // PRIVATE METHODS: -----------------------------------------------------------------------

        protected void Callback(string name)
        {
            if (this.spName == null) return;
            this.spName.stringValue = name;

            this.spName.serializedObject.ApplyModifiedProperties();
            this.spName.serializedObject.Update();
        }

        private Rect GetRectLabel(Rect rect)
        {
            return new Rect(
                rect.x,
                rect.y,
                (EditorGUIUtility.labelWidth),
                rect.height
            );
        }

        private Rect GetRectField(Rect rect)
        {
            return new Rect(
                rect.x + (EditorGUIUtility.labelWidth),
                rect.y,
                rect.width - (EditorGUIUtility.labelWidth),
                rect.height
            );
        }
    }
}