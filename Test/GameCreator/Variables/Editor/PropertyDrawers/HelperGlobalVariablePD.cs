namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(HelperGlobalVariable))]
    public class HelperGlobalVariablePD : HelperGenericVariablePD
    {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
            this.spAllowTypesMask = property.FindPropertyRelative(PROP_ALLOW_TYPES_MASK);

            this.PaintVariables(
                position,
                property.FindPropertyRelative(PROP_NAME),
                label
            );
		}

		protected override GenericVariableSelectWindow GetWindow(Rect ctaRect)
        {
            return new GlobalVariableSelectWindow(
                ctaRect, 
                this.Callback,
                (this.spAllowTypesMask == null ? 0 : spAllowTypesMask.intValue)
            );
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(PROP_NAME));
        }
    }
}