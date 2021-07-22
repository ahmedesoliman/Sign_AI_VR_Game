namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
    public class ActionTargetCamera : IAction
	{
        public bool mainCameraMotor = false;
        public CameraMotor cameraMotor;

        public float anchorDistance = 3f;
        public float horizontalOffset = 0.5f;

        [Space] public TargetGameObject anchor = new TargetGameObject(TargetGameObject.Target.Player);
        [Space] public TargetPosition target = new TargetPosition(TargetPosition.Target.Invoker);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            CameraMotor motor = (this.mainCameraMotor ? CameraMotor.MAIN_MOTOR : this.cameraMotor);
            if (motor != null && motor.cameraMotorType.GetType() == typeof(CameraMotorTypeTarget))
            {
                CameraMotorTypeTarget targetMotor = (CameraMotorTypeTarget)motor.cameraMotorType;
                targetMotor.anchorDistance = this.anchorDistance;
                targetMotor.horizontalOffset = this.horizontalOffset;
                targetMotor.anchor = this.anchor;
                targetMotor.target = this.target;
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    public static new string NAME = "Camera/Target Camera Settings";
        private const string NODE_TITLE = "Change {0} Target Camera settings";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spMainCameraMotor;
        private SerializedProperty spCameraMotor;

        private SerializedProperty spDistance;
        private SerializedProperty spOffset;
        private SerializedProperty spAnchor;
        private SerializedProperty spTarget;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            string motor = (this.mainCameraMotor
                ? "[Main Camera]"
                : (this.cameraMotor == null ? "none" : this.cameraMotor.gameObject.name)
            );
            
			return string.Format(NODE_TITLE, motor);
		}

		protected override void OnEnableEditorChild ()
		{
            this.spMainCameraMotor = this.serializedObject.FindProperty("mainCameraMotor");
            this.spCameraMotor = this.serializedObject.FindProperty("cameraMotor");
            this.spDistance = this.serializedObject.FindProperty("anchorDistance");
            this.spOffset = this.serializedObject.FindProperty("horizontalOffset");
            this.spAnchor = this.serializedObject.FindProperty("anchor");
            this.spTarget = this.serializedObject.FindProperty("target");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spMainCameraMotor = null;
            this.spCameraMotor = null;
            this.spDistance = null;
            this.spOffset = null;
            this.spAnchor = null;
            this.spTarget = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spMainCameraMotor);
            EditorGUI.BeginDisabledGroup(this.spMainCameraMotor.boolValue);
            EditorGUILayout.PropertyField(this.spCameraMotor);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(this.spDistance);
            EditorGUILayout.PropertyField(this.spOffset);

            EditorGUILayout.PropertyField(this.spAnchor);
            EditorGUILayout.PropertyField(this.spTarget);
			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
