namespace GameCreator.Core
{
	using System;
    using System.IO;
    using System.Reflection;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(IAction), true)]
	public class IActionEditor : Editor 
	{
        private const BindingFlags BINDING_FLAGS = (
            BindingFlags.Public |
            BindingFlags.Static |
            BindingFlags.FlattenHierarchy
        );

		public SerializedProperty spActions;
		public IAction action;

		private SerializedProperty spAction;
        private Texture2D icon;

		// INITIALIZERS: --------------------------------------------------------------------------

		private void OnEnable()
		{
            if (target == null || serializedObject == null) return;

            this.action = (IAction)target;
            this.action.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            this.action.OnEnableEditor(this.action);

            Type actionType = this.action.GetType();
            string actionName = (string)actionType.GetField("NAME", BINDING_FLAGS).GetValue(null);

            FieldInfo customIconsFieldInfo = actionType.GetField(
                SelectTypePanel.CUSTOM_ICON_PATH_VARIABLE
            );

            string iconsPath = SelectTypePanel.ICONS_ACTIONS_PATH;
            if (customIconsFieldInfo != null)
            {
                string customIconsPath = (string)customIconsFieldInfo.GetValue(null);
                if (!string.IsNullOrEmpty(customIconsPath))
                {
                    iconsPath = customIconsPath;
                }
            }

            string actionIconPath = Path.Combine(
                iconsPath,
                SelectTypePanel.GetName(actionName) + ".png"
            );

            this.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(actionIconPath);
            if (icon == null) icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(iconsPath, "Default.png"));
            if (icon == null) icon = EditorGUIUtility.FindTexture("GameObject Icon");
		}

		private void OnDisable()
		{
			if (this.action == null) return;
			this.action.OnDisableEditor();
		}

		public void Setup(SerializedProperty spActions, int index)
		{
            this.action = (IAction)target;
			this.action.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
			this.action.OnEnableEditor(target);
		}

        public Texture2D GetIcon()
        {
            return this.icon;
        }

		// INSPECTOR: -----------------------------------------------------------------------------

		public override void OnInspectorGUI ()
		{
            if (this.target == null || this.serializedObject == null) return;
            this.action.OnInspectorGUIEditor();
		}
	}
}