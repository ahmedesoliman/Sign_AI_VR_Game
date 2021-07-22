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
    public class ActionFPSCamera : IAction
	{
        public bool mainCameraMotor = false;
        public CameraMotor cameraMotor;

        public Vector3 positionOffset = new Vector3(0, 2, 0);

        [Header("Head Bobbing")]
        public float period = 0.5f;
        public Vector3 amount = new Vector3(0.05f, 0.05f, 0.01f);

        [Space]
        public CameraMotorTypeFirstPerson.ModelManipulator modelManipulator = (
            CameraMotorTypeFirstPerson.ModelManipulator.StiffSpineAnimation
        );


        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            CameraMotor motor = (this.mainCameraMotor ? CameraMotor.MAIN_MOTOR : this.cameraMotor);
            if (motor != null && motor.cameraMotorType.GetType() == typeof(CameraMotorTypeFirstPerson))
            {
                CameraMotorTypeFirstPerson fpsMotor = (CameraMotorTypeFirstPerson)motor.cameraMotorType;
                fpsMotor.positionOffset = this.positionOffset;
                fpsMotor.headbobPeriod = this.period;
                fpsMotor.headbobAmount = this.amount;
                fpsMotor.modelManipulator = this.modelManipulator; 
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    public static new string NAME = "Camera/FPS Camera Settings";
        private const string NODE_TITLE = "Change {0} FPS Camera settings";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spMainCameraMotor;
        private SerializedProperty spCameraMotor;

        private SerializedProperty spPositionOffset;
        private SerializedProperty spPeriod;
        private SerializedProperty spAmount;
        private SerializedProperty spModelManipulator;

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
            this.spPositionOffset = this.serializedObject.FindProperty("positionOffset");
            this.spPeriod = this.serializedObject.FindProperty("period");
            this.spAmount = this.serializedObject.FindProperty("amount");
            this.spModelManipulator = this.serializedObject.FindProperty("modelManipulator");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spMainCameraMotor = null;
            this.spCameraMotor = null;
            this.spPositionOffset = null;
            this.spPeriod = null;
            this.spAmount = null;
            this.spModelManipulator = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spMainCameraMotor);
            EditorGUI.BeginDisabledGroup(this.spMainCameraMotor.boolValue);
            EditorGUILayout.PropertyField(this.spCameraMotor);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(this.spPositionOffset);
            EditorGUILayout.PropertyField(this.spPeriod);
            EditorGUILayout.PropertyField(this.spAmount);
            EditorGUILayout.PropertyField(this.spModelManipulator);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
