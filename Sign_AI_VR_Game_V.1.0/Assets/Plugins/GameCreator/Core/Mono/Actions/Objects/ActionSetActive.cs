namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionSetActive : IAction 
	{
        public TargetGameObject target = new TargetGameObject();
		public bool active = true;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject targetValue = this.target.GetGameObject(target);
            if (targetValue != null) targetValue.SetActive(this.active);

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Set Active";
		private const string NODE_TITLE = "Set {0} object {1}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;
		private SerializedProperty spActive;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE, 
				(this.active ? "active" : "inactive"),
				this.target
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTarget = this.serializedObject.FindProperty("target");
			this.spActive = this.serializedObject.FindProperty("active");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spTarget = null;
			this.spActive = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spTarget);
			EditorGUILayout.PropertyField(this.spActive);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}