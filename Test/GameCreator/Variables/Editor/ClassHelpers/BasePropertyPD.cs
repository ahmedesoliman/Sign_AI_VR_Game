namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class BasePropertyPD : PropertyDrawer
    {
        private const string PROP_OPTION = "optionIndex";
        private const string PROP_VALUE = "value";
        private const string PROP_GLOBAL = "global";
        private const string PROP_LOCAL = "local";
        private const string PROP_LIST = "list";

        private static readonly GUIContent GUICONTENT_EMPTY = new GUIContent(" ");

        private bool init = false;

        // PAINT METHODS: -------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
            if (!this.init)
            {
                int allowTypesMask = this.GetAllowTypesMask();
                property
                    .FindPropertyRelative(PROP_GLOBAL)
                    .FindPropertyRelative(HelperGenericVariablePD.PROP_ALLOW_TYPES_MASK)
                    .intValue = allowTypesMask;

                property
                    .FindPropertyRelative(PROP_LOCAL)
                    .FindPropertyRelative(HelperGenericVariablePD.PROP_ALLOW_TYPES_MASK)
                    .intValue = allowTypesMask;

                this.init = true;
            }

            Rect rectOption = new Rect(
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
            );
            Rect rectContent = new Rect(
                position.x,
                rectOption.y + rectOption.height,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            SerializedProperty spOption = property.FindPropertyRelative(PROP_OPTION);
            EditorGUI.PropertyField(rectOption, spOption, label);

            int option = spOption.intValue;
            switch (option)
            {
                case 0 : this.PaintContent(property, rectContent, PROP_VALUE); break;
                case 1 : this.PaintContent(property, rectContent, PROP_GLOBAL); break;    
                case 2 : this.PaintContent(property, rectContent, PROP_LOCAL); break;
                case 3 : this.PaintContent(property, rectContent, PROP_LIST); break;
            }
		}

        private void PaintContent(SerializedProperty property, Rect rect, string prop)
        {
            SerializedProperty spValue = property.FindPropertyRelative(prop);
            EditorGUI.PropertyField(rect, spValue, GUICONTENT_EMPTY);
        }

        // HEIGHT METHOD: -------------------------------------------------------------------------

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
            SerializedProperty spOption = property.FindPropertyRelative(PROP_OPTION);

            int option = spOption.intValue;
            float height = EditorGUI.GetPropertyHeight(spOption);

            switch (option)
            {
                case 0: height += this.GetHeight(property, PROP_VALUE); break;
                case 1: height += this.GetHeight(property, PROP_GLOBAL); break;
                case 2: height += this.GetHeight(property, PROP_LOCAL); break;
                case 3: height += this.GetHeight(property, PROP_LIST); break;
            }

            return height;
		}

        private float GetHeight(SerializedProperty property, string name)
        {
            SerializedProperty spName = property.FindPropertyRelative(name);
            return EditorGUI.GetPropertyHeight(spName);
        }

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected virtual int GetAllowTypesMask()
        {
            return ~0;
        }
	}
}