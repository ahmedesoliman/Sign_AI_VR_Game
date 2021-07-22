namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionQualitySettings : IAction
	{
        public NumberProperty level = new NumberProperty(0);
        public bool applyExpensiveSettings = true;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            QualitySettings.SetQualityLevel(
                (int)this.level.GetValue(target), 
                this.applyExpensiveSettings
            );

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Application/Quality Settings";
		private const string NODE_TITLE = "Change quality settings";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spLevel;
        private SerializedProperty spApplyExpensive;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return NODE_TITLE;
		}

		protected override void OnEnableEditorChild ()
		{
            this.spLevel = this.serializedObject.FindProperty("level");
            this.spApplyExpensive = this.serializedObject.FindProperty("applyExpensiveSettings");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spLevel = null;
            this.spApplyExpensive = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spLevel);
            EditorGUILayout.PropertyField(this.spApplyExpensive);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
