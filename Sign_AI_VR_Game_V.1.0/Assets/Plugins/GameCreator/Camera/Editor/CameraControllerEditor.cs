namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using GameCreator.Core;

	[CustomEditor(typeof(CameraController))]
	public class CameraControllerEditor : Editor 
	{
		private const string PROP_CURR_CAMERA_MOTOR = "currentCameraMotor";
		private const string PROP_SMOOTH_TIME = "cameraSmoothTime";
		private const string PROP_SMOOTH_TIME_TRANS = "positionDuration";
		private const string PROP_SMOOTH_TIME_ROTAT = "rotationDuration";
		
		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private SerializedProperty spCurrentCameraMotor;
		private SerializedProperty spCameraSmoothTimeTranslation;
		private SerializedProperty spCameraSmoothTimeRotation;

		// INITIALIZERS: -----------------------------------------------------------------------------------------------

		private void OnEnable()
		{
			this.spCurrentCameraMotor = serializedObject.FindProperty(PROP_CURR_CAMERA_MOTOR);

			SerializedProperty smoothTime = serializedObject.FindProperty(PROP_SMOOTH_TIME);
			this.spCameraSmoothTimeTranslation = smoothTime.FindPropertyRelative(PROP_SMOOTH_TIME_TRANS);
			this.spCameraSmoothTimeRotation = smoothTime.FindPropertyRelative(PROP_SMOOTH_TIME_ROTAT);
		}

		// INSPECTOR GUI: ----------------------------------------------------------------------------------------------

		public override void OnInspectorGUI ()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(this.spCurrentCameraMotor);
			EditorGUILayout.PropertyField(this.spCameraSmoothTimeTranslation);
			EditorGUILayout.PropertyField(this.spCameraSmoothTimeRotation);

			serializedObject.ApplyModifiedProperties();
		}
	}
}