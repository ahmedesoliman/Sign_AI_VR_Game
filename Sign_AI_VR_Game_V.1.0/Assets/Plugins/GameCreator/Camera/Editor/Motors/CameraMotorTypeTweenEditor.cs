namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using GameCreator.Core.Hooks;

	[CustomEditor(typeof(CameraMotorTypeTween))]
	public class CameraMotorTypeTweenEditor : ICameraMotorTypeEditor 
	{
		private const string PROP_CAMERA_ENDPOINT = "cameraEndPoint";
		private const string PROP_DURATION = "duration";
		private const string PROP_EASING = "easing";
        private const string PROP_LOOKAT = "lookAt";

		private static readonly GUIContent GUICONTENT_CAM_POINT1 = new GUIContent("Camera Start Point");
		private static readonly GUIContent GUICONTENT_CAM_POINT2 = new GUIContent("Camera End Point");
		private static readonly GUIContent GUICONTENT_DURATION = new GUIContent("Duration (s)");

		private static readonly Color HANDLE_COLOR_CAMERA = Color.white;
		private const float HANDLE_SIZE_CAMERA = 0.30f;

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private SerializedProperty spCameraEndPoint;
		private SerializedProperty spDuration;
		private SerializedProperty spEasing;
        private SerializedProperty spLookAt;

		private int handlesControlIndex = 0;

		// INITIALIZE: -------------------------------------------------------------------------------------------------

		protected override void OnSubEnable()
		{
			this.spCameraEndPoint = serializedObject.FindProperty(PROP_CAMERA_ENDPOINT);
			this.spDuration = serializedObject.FindProperty(PROP_DURATION);
			this.spEasing = serializedObject.FindProperty(PROP_EASING);
            this.spLookAt = serializedObject.FindProperty(PROP_LOOKAT);
		}

		public override void OnCreate (Transform cameraMotorTransform)
		{
			serializedObject.Update();
            SerializedProperty spCameraEnd = this.serializedObject.FindProperty(PROP_CAMERA_ENDPOINT);
            spCameraEnd.vector3Value += cameraMotorTransform.position;
			serializedObject.ApplyModifiedProperties();
		}

		// INSPECTOR GUI: ----------------------------------------------------------------------------------------------

		protected override bool OnSubInspectorGUI (Transform cameraMotorTransform)
		{
			serializedObject.Update();
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.Vector3Field(GUICONTENT_CAM_POINT1, cameraMotorTransform.position);
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.PropertyField(this.spCameraEndPoint, GUICONTENT_CAM_POINT2);
			EditorGUILayout.PropertyField(this.spDuration, GUICONTENT_DURATION);
			EditorGUILayout.PropertyField(this.spEasing);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spLookAt);

            serializedObject.ApplyModifiedProperties();
			return false;
		}

		// SCENE GUI: --------------------------------------------------------------------------------------------------

		public override bool OnSubSceneGUI(Transform cameraMotorTransform)
		{
			serializedObject.Update();

			Vector3 camPosition1 = cameraMotorTransform.position;
			Vector3 camPosition2 = this.spCameraEndPoint.vector3Value;

			bool dataDirty = true;
			Handles.color = HANDLE_COLOR_CAMERA;
			Handles.DrawLine(camPosition1, camPosition2);

			if (Tools.current != Tool.None) this.handlesControlIndex = 0;

			this.OnSceneGUIDrawHandlesOrigin(camPosition1, HANDLE_COLOR_CAMERA, HANDLE_SIZE_CAMERA, 0);
			this.OnSceneGUIDrawHandles(camPosition2, HANDLE_COLOR_CAMERA, HANDLE_SIZE_CAMERA, 1);

			switch (this.handlesControlIndex)
			{
			case 1 : camPosition2 = Handles.DoPositionHandle(camPosition2, Quaternion.identity); break;
			default: dataDirty = false; break;
			}

			cameraMotorTransform.position = camPosition1;
			this.spCameraEndPoint.vector3Value = camPosition2;

			serializedObject.ApplyModifiedProperties();
			return dataDirty;
		}

		private void OnSceneGUIDrawHandles(Vector3 position, Color color, float size, int controlIndex)
		{
			Handles.color = color;
			if (Handles.Button(position, Quaternion.identity, size, size * 1.5f, Handles.SphereHandleCap))
			{
				this.handlesControlIndex = controlIndex;
				Tools.current = Tool.None;
			}
		}

		private void OnSceneGUIDrawHandlesOrigin(Vector3 position, Color color, float size, int controlIndex)
		{
			Handles.color = color;
			if (Handles.Button(position, Quaternion.identity, size, size * 1.5f, Handles.SphereHandleCap))
			{
				this.handlesControlIndex = controlIndex;
				Tools.current = Tool.Move;
			}
		}
	}
}