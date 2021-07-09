namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core.Hooks;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionLookAt : IAction 
	{
        // PROPERTIES: ----------------------------------------------------------------------------

        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.GameObject);
        public TargetPosition lookAtPosition = new TargetPosition();
		
		[RotationConstraint] 
		public Vector3 freezeRotation = new Vector3(1,0,1);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Transform targetTrans = this.target.GetTransform(target);
            if (targetTrans != null)
            {
                targetTrans.LookAt(lookAtPosition.GetPosition(target), Vector3.up);

                Vector3 scalar = new Vector3(
                    (Mathf.Approximately(this.freezeRotation.x, 0f) ? 1 : 0),
                    (Mathf.Approximately(this.freezeRotation.y, 0f) ? 1 : 0),
                    (Mathf.Approximately(this.freezeRotation.z, 0f) ? 1 : 0)
                );
                    
                targetTrans.localRotation = Quaternion.Euler(Vector3.Scale(
                    targetTrans.localEulerAngles, scalar
                ));
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Look At";
		private const string NODE_TITLE = "{0} look at {1}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;
		private SerializedProperty spLookAtPosition;
		private SerializedProperty spConstraints;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE, 
				target.ToString(),
				this.lookAtPosition.ToString()
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTarget = this.serializedObject.FindProperty("target");
			this.spLookAtPosition = this.serializedObject.FindProperty("lookAtPosition");
			this.spConstraints = this.serializedObject.FindProperty("freezeRotation");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spTarget = null;
			this.spLookAtPosition = null;
			this.spConstraints = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spTarget);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.spLookAtPosition);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.spConstraints);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}