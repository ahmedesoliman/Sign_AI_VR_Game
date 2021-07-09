namespace GameCreator.Core
{
    using System;
    using System.IO;
    using System.Reflection;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(ICondition), true)]
	public class IConditionEditor : Editor 
	{
        private const BindingFlags BINDING_FLAGS = (
            BindingFlags.Public |
            BindingFlags.Static |
            BindingFlags.FlattenHierarchy
        );

		public SerializedProperty spConditions;
		public ICondition condition;

		private SerializedProperty spCondition;
        private Texture2D icon;

		// INITIALIZERS: --------------------------------------------------------------------------

		private void OnEnable()
		{
            if (target == null || serializedObject == null) return;

            this.condition = (ICondition)target;
            this.condition.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            this.condition.OnEnableEditor(this.condition);

            Type conditionType = this.condition.GetType();
            string conditionName = (string)conditionType.GetField("NAME", BINDING_FLAGS).GetValue(null);

            FieldInfo customIconsFieldInfo = conditionType.GetField(
                SelectTypePanel.CUSTOM_ICON_PATH_VARIABLE,
                BINDING_FLAGS
            );

            string iconsPath = SelectTypePanel.ICONS_CONDITIONS_PATH;
            if (customIconsFieldInfo != null)
            {
                string customIconsPath = (string)customIconsFieldInfo.GetValue(null);
                if (!string.IsNullOrEmpty(customIconsPath))
                {
                    iconsPath = customIconsPath;
                }
            }

            string conditionIconPath = Path.Combine(
                iconsPath,
                SelectTypePanel.GetName(conditionName) + ".png"
            );

            this.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(conditionIconPath);
            if (icon == null) icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(iconsPath, "Default.png"));
            if (icon == null) icon = EditorGUIUtility.FindTexture("GameObject Icon");
		}

		public void Setup(SerializedProperty spConditions, int index)
		{
			this.spCondition = spConditions.GetArrayElementAtIndex(index);
            ((ICondition)target).OnEnableEditor(this.spCondition.objectReferenceValue);
		}

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Texture2D GetIcon()
        {
            return this.icon;
        }

		// INSPECTOR: -----------------------------------------------------------------------------

		public override void OnInspectorGUI ()
		{
            if (this.target == null || this.serializedObject == null) return;
            this.condition.OnInspectorGUIEditor();
		}
	}
}