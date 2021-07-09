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
	public class ActionPhysics : IAction 
	{
		public Vector3 force = Vector3.zero;
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.GameObject);

		public ForceMode forceMode = ForceMode.Impulse;
		public Space spaceMode = Space.Self;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject targetGO = this.target.GetGameObject(target);
            if (targetGO == null) return true;

            Rigidbody targetRB = targetGO.GetComponent<Rigidbody>();
            if (targetRB != null)
            {
                Vector3 directionForce = Vector3.zero;

                if (spaceMode == Space.World) directionForce = this.force;
                else directionForce = targetRB.transform.TransformDirection(this.force);

                targetRB.AddForce(directionForce, this.forceMode);
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Physics";
		private const string NODE_TITLE = "Add {0} ({1:0.0},{2:0.0},{3:0.0}) to {4}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spForce;
		private SerializedProperty spTarget;
		private SerializedProperty spForceMode;
		private SerializedProperty spSpaceMode;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE, 
				this.forceMode.ToString(),
				this.force.x,
				this.force.y,
				this.force.z,
                this.target
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spForce = this.serializedObject.FindProperty("force");
			this.spTarget = this.serializedObject.FindProperty("target");
			this.spForceMode = this.serializedObject.FindProperty("forceMode");
			this.spSpaceMode = this.serializedObject.FindProperty("spaceMode");
		}

		protected override void OnDisableEditorChild ()
		{
			return;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spTarget);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(this.spForce);
			EditorGUILayout.PropertyField(this.spForceMode);
			EditorGUILayout.PropertyField(this.spSpaceMode);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}