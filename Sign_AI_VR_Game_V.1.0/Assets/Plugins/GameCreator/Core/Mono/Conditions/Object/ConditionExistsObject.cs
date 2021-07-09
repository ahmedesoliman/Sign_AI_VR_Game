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
	public class ConditionExistsObject : ICondition
	{
        public TargetGameObject target = new TargetGameObject();

		// EXECUTABLE: ----------------------------------------------------------------------------
		
        public override bool Check(GameObject target)
		{
            return (this.target.GetGameObject(target) != null);
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Exists GameObject";
		private const string NODE_TITLE = "Does {0} exist";

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

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spTarget);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}