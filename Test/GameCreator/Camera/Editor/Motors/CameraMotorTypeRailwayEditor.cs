namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using GameCreator.Core.Hooks;

	[CustomEditor(typeof(CameraMotorTypeRailway))]
	public class CameraMotorTypeRailwayEditor : ICameraMotorTypeEditor 
	{
		private const string PROP_CAMERA_ENDPOINT = "cameraEndPoint";
		private const string PROP_TARGET_POINTS = "targetPoints";
		private const string PROP_TARGET = "target";
        private const string PROP_LOOKAT = "lookAt";

        private static readonly GUIContent GUICONTENT_CAM_POINT1 = new GUIContent("Camera Start Point");
		private static readonly GUIContent GUICONTENT_CAM_POINT2 = new GUIContent("Camera End Point");
		private static readonly GUIContent GUICONTENT_TAR_POINT1 = new GUIContent("Target Start Point");
		private static readonly GUIContent GUICONTENT_TAR_POINT2 = new GUIContent("Target End Point");

		private static readonly Color HANDLE_COLOR_CAMERA = Color.white;
		private static readonly Color HANDLE_COLOR_TARGET = Color.cyan;
		private const float HANDLE_SIZE_CAMERA = 0.30f;
		private const float HANDLE_SIZE_TARGET = 0.15f;
		private const float DOTTED_LINE_SEPARATION = 10.0f;

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spCameraEndPoint;
		private SerializedProperty spTargetPoints1;
		private SerializedProperty spTargetPoints2;

		private SerializedProperty spTarget;
        private SerializedProperty spLookAt;

        private int handlesControlIndex = 0;

		// INITIALIZE: ----------------------------------------------------------------------------

		protected override void OnSubEnable()
		{
			this.spCameraEndPoint = serializedObject.FindProperty(PROP_CAMERA_ENDPOINT);
			this.spTargetPoints1 = serializedObject.FindProperty(PROP_TARGET_POINTS).GetArrayElementAtIndex(0);
			this.spTargetPoints2 = serializedObject.FindProperty(PROP_TARGET_POINTS).GetArrayElementAtIndex(1);

            this.spTarget = serializedObject.FindProperty(PROP_TARGET);
            this.spLookAt = serializedObject.FindProperty(PROP_LOOKAT);

        }

		public override void OnCreate (Transform cameraMotorTransform)
		{
			serializedObject.Update();
            if (this.spCameraEndPoint == null || this.spTargetPoints1 == null || this.spTargetPoints2 == null) return;
			this.spCameraEndPoint.vector3Value += cameraMotorTransform.position;
			this.spTargetPoints1.vector3Value  += cameraMotorTransform.position;
			this.spTargetPoints2.vector3Value  += cameraMotorTransform.position;
			serializedObject.ApplyModifiedProperties();
		}

		// INSPECTOR GUI: -------------------------------------------------------------------------

		protected override bool OnSubInspectorGUI (Transform cameraMotorTransform)
		{
			serializedObject.Update();
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.Vector3Field(GUICONTENT_CAM_POINT1, cameraMotorTransform.position);
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.PropertyField(this.spCameraEndPoint, GUICONTENT_CAM_POINT2);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.spTargetPoints1, GUICONTENT_TAR_POINT1);
			EditorGUILayout.PropertyField(this.spTargetPoints2, GUICONTENT_TAR_POINT2);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.spTarget);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spLookAt);

            serializedObject.ApplyModifiedProperties();
			return false;
		}

        public override bool ShowPreviewCamera()
        {
            return false;
        }

        // SCENE GUI: -----------------------------------------------------------------------------

        public override bool OnSubSceneGUI(Transform cameraMotorTransform)
		{
			serializedObject.Update();

			Vector3 camPosition1 = cameraMotorTransform.position;
			Vector3 camPosition2 = this.spCameraEndPoint.vector3Value;
			Vector3 tarPosition1 = this.spTargetPoints1.vector3Value;
			Vector3 tarPosition2 = this.spTargetPoints2.vector3Value;

			bool dataDirty = true;
			Handles.color = HANDLE_COLOR_CAMERA;

			Handles.DrawLine(camPosition1, camPosition2);
			Handles.DrawDottedLine(camPosition1, tarPosition1, DOTTED_LINE_SEPARATION);
			Handles.DrawDottedLine(camPosition2, tarPosition2, DOTTED_LINE_SEPARATION);

			Handles.color = HANDLE_COLOR_TARGET;
			Handles.DrawLine(tarPosition1, tarPosition2);

			if (Tools.current != Tool.None) this.handlesControlIndex = 0;

			this.OnSceneGUIDrawHandlesOrigin(camPosition1, HANDLE_COLOR_CAMERA, HANDLE_SIZE_CAMERA, 0);
			this.OnSceneGUIDrawHandles(camPosition2, HANDLE_COLOR_CAMERA, HANDLE_SIZE_CAMERA, 1);
			this.OnSceneGUIDrawHandles(tarPosition1, HANDLE_COLOR_TARGET, HANDLE_SIZE_TARGET, 2);
			this.OnSceneGUIDrawHandles(tarPosition2, HANDLE_COLOR_TARGET, HANDLE_SIZE_TARGET, 3);

			switch (this.handlesControlIndex)
			{
			case 1 : camPosition2 = Handles.DoPositionHandle(camPosition2, Quaternion.identity); break;
			case 2 : tarPosition1 = Handles.DoPositionHandle(tarPosition1, Quaternion.identity); break;
			case 3 : tarPosition2 = Handles.DoPositionHandle(tarPosition2, Quaternion.identity); break;
			default: dataDirty = false; break;
			}

			cameraMotorTransform.position = camPosition1;
			this.spCameraEndPoint.vector3Value = camPosition2;
			this.spTargetPoints1.vector3Value = tarPosition1;
			this.spTargetPoints2.vector3Value = tarPosition2;

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