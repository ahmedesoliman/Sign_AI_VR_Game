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
	public class ConditionActiveObject : ICondition
	{
        public TargetGameObject target = new TargetGameObject();
        public BoolProperty isActive = new BoolProperty(true);

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool Check(GameObject target)
		{
            GameObject targetValue = this.target.GetGameObject(target);
            if (targetValue == null) return false;

            bool checkState = this.isActive.GetValue(target);
            
            if (checkState) return targetValue.activeInHierarchy;
            return !targetValue.activeInHierarchy;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/GameObject Active";
		private const string NODE_TITLE = "Is {0} active state {1}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;
		private SerializedProperty spIsActive;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE, 
				this.target,
				this.isActive
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTarget = this.serializedObject.FindProperty("target");
			this.spIsActive = this.serializedObject.FindProperty("isActive");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spTarget);
			EditorGUILayout.PropertyField(this.spIsActive);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
