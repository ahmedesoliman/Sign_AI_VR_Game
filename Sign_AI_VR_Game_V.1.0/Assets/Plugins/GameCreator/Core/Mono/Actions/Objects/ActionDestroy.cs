namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionDestroy : IAction 
	{
        public TargetGameObject target = new TargetGameObject();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject go = this.target.GetGameObject(target);
            if (go != null) Destroy(go);

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		private static readonly GUIContent GUICONTENT_DESTROY = new GUIContent("Target");

		public static new string NAME = "Object/Destroy";
		private const string NODE_TITLE = "Destroy {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.target);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTarget = this.serializedObject.FindProperty("target");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spTarget = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spTarget, GUICONTENT_DESTROY);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}