namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(VariableGlobalProperty))]
    public class VariableGlobalPropertyPD : PropertyDrawer
    {
        private const string PROP_GLOBAL = "globalVariable";

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
                this.init = true;
            }

            this.PaintContent(property, position, PROP_GLOBAL);
		}

        private void PaintContent(SerializedProperty property, Rect rect, string prop)
        {
            SerializedProperty spValue = property.FindPropertyRelative(prop);
            EditorGUI.PropertyField(rect, spValue);
        }

        // HEIGHT METHOD: -------------------------------------------------------------------------

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
            return this.GetHeight(property, PROP_GLOBAL);
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