namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(CameraMotorTypeFirstPerson))]
	public class CameraMotorTypeFirstPersonEditor : ICameraMotorTypeEditor 
	{
		private static readonly Color HANDLE_COLOR_BACKG = new Color(256f,256f,256f, 0.2f);
		private static readonly Color HANDLE_COLOR_PITCH_P = new Color(256f,0f,0f, 0.2f);
		private static readonly Color HANDLE_COLOR_PITCH_C = new Color(256f,0f,0f, 0.8f);

        private static readonly GUIContent GC_HEADBOB_PERIOD = new GUIContent("Period");
        private static readonly GUIContent GC_HEADBOB_AMOUNT = new GUIContent("Amount");

		private const string PROP_POSITION_OFFSET = "positionOffset";
        private const string PROP_ROTATE_INPUT = "rotateInput";
		private const string PROP_SENSITIVITY = "sensitivity";
		private const string PROP_MAX_PITCH = "maxPitch";
		private const string PROP_SMOOTH_ROTATION = "smoothRotation";

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private SerializedProperty spPositionOffset;
        private SerializedProperty spRotateInput;
		private SerializedProperty spSensitivity;
		private SerializedProperty spMaxPitch;
		private SerializedProperty spSmoothRotation;

        private SerializedProperty spHeadbobPeriod;
        private SerializedProperty spHeadbobAmount;

        private SerializedProperty spModelManipulator;

		// INITIALIZE: -------------------------------------------------------------------------------------------------

		protected override void OnSubEnable()
		{
			this.spPositionOffset = serializedObject.FindProperty(PROP_POSITION_OFFSET);
            this.spRotateInput = serializedObject.FindProperty(PROP_ROTATE_INPUT);
			this.spSensitivity = serializedObject.FindProperty(PROP_SENSITIVITY);
			this.spMaxPitch = serializedObject.FindProperty(PROP_MAX_PITCH);

			this.spSmoothRotation = serializedObject.FindProperty(PROP_SMOOTH_ROTATION);

            this.spHeadbobPeriod = serializedObject.FindProperty("headbobPeriod");
            this.spHeadbobAmount = serializedObject.FindProperty("headbobAmount");
            this.spModelManipulator = serializedObject.FindProperty("modelManipulator");
		}

		// INSPECTOR GUI: ----------------------------------------------------------------------------------------------

		protected override bool OnSubInspectorGUI (Transform cameraMotorTransform)
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(this.spPositionOffset);
			EditorGUILayout.PropertyField(this.spSensitivity);
			EditorGUILayout.PropertyField(this.spMaxPitch);

			EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spRotateInput);
			EditorGUILayout.PropertyField(this.spSmoothRotation);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Head Bobbing", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(this.spHeadbobPeriod, GC_HEADBOB_PERIOD);
            EditorGUILayout.PropertyField(this.spHeadbobAmount, GC_HEADBOB_AMOUNT);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Experimental", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(this.spModelManipulator);

			serializedObject.ApplyModifiedProperties();
			return false;
		}

		// SCENE GUI: --------------------------------------------------------------------------------------------------

		public override bool OnSubSceneGUI(Transform cameraMotorTransform)
		{
			serializedObject.Update();

			Handles.color = HANDLE_COLOR_BACKG;
			Handles.DrawSolidArc(
				cameraMotorTransform.position, 
				cameraMotorTransform.TransformDirection(Vector3.right),
				cameraMotorTransform.TransformDirection(Vector3.up),
				180f,
				HandleUtility.GetHandleSize(cameraMotorTransform.position)
			);

			float angle = this.spMaxPitch.floatValue;
			Vector3 direction = Quaternion.AngleAxis(-angle/2.0f, Vector3.right) * Vector3.forward;

			Handles.color = HANDLE_COLOR_PITCH_P;
			Handles.DrawSolidArc(
				cameraMotorTransform.position, 
				cameraMotorTransform.TransformDirection(Vector3.right),
				cameraMotorTransform.TransformDirection(direction),
				angle,
				HandleUtility.GetHandleSize(cameraMotorTransform.position)
			);

			Handles.color = HANDLE_COLOR_PITCH_C;
			Handles.DrawWireArc(
				cameraMotorTransform.position, 
				cameraMotorTransform.TransformDirection(Vector3.right),
				cameraMotorTransform.TransformDirection(direction),
				angle,
				HandleUtility.GetHandleSize(cameraMotorTransform.position)
			);

			serializedObject.ApplyModifiedProperties();
			return false;
		}

        public override bool ShowPreviewCamera()
        {
            return false;
        }
    }
}