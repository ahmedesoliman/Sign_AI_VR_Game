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
    public class ActionFixedCamera : IAction
	{
        public bool mainCameraMotor = false;
        public CameraMotor cameraMotor;

        public TargetDirection lookAt = new TargetDirection(TargetDirection.Target.CurrentDirection);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            CameraMotor motor = (this.mainCameraMotor ? CameraMotor.MAIN_MOTOR : this.cameraMotor);
            if (motor != null && motor.cameraMotorType.GetType() == typeof(CameraMotorTypeFixed))
            {
                CameraMotorTypeFixed fixedMotor = (CameraMotorTypeFixed)motor.cameraMotorType;
                fixedMotor.lookAt = this.lookAt;
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    public static new string NAME = "Camera/Fixed Camera Settings";
        private const string NODE_TITLE = "Change {0} Fixed Camera settings";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spMainCameraMotor;
        private SerializedProperty spCameraMotor;

        private SerializedProperty spLookAt;

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
            this.spLookAt = this.serializedObject.FindProperty("lookAt");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spMainCameraMotor = null;
            this.spCameraMotor = null;
            this.spLookAt = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spMainCameraMotor);
            EditorGUI.BeginDisabledGroup(this.spMainCameraMotor.boolValue);
            EditorGUILayout.PropertyField(this.spCameraMotor);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(this.spLookAt);
			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
