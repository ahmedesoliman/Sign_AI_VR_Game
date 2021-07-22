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
    public class ConditionRigidbody : ICondition
	{
        public enum Condition
        {
            IsKinematic,
            IsSleeping
        }

        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.GameObject);
        public Condition condition = Condition.IsKinematic;

		// EXECUTABLE: ----------------------------------------------------------------------------
		
        public override bool Check(GameObject target)
		{
            GameObject targetGO = this.target.GetGameObject(target);
            if (targetGO == null) return true;

            Rigidbody rb = targetGO.GetComponent<Rigidbody>();
            if (rb == null) return false;

            switch (this.condition)
            {
                case Condition.IsKinematic : return rb.isKinematic;
                case Condition.IsSleeping : return rb.IsSleeping();
            }

            return false;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Rigidbody";
        private const string NODE_TITLE = "Rigidbody: {0} {1}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;
        private SerializedProperty spCondition;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(
                NODE_TITLE, 
                this.target, 
                this.condition.ToString()
            );
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTarget = this.serializedObject.FindProperty("target");
            this.spCondition = this.serializedObject.FindProperty("condition");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spTarget);
            EditorGUILayout.PropertyField(this.spCondition);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}