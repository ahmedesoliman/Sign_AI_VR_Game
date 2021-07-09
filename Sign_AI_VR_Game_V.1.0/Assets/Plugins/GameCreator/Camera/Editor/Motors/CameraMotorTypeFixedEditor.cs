namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(CameraMotorTypeFixed))]
	public class CameraMotorTypeFixedEditor : ICameraMotorTypeEditor 
	{
        private const string PROP_LOOKAT = "lookAt";

        private SerializedProperty spLookAt;

		// PAINT METHODS: -------------------------------------------------------------------------

		protected override void OnSubEnable()
		{
            this.spLookAt = serializedObject.FindProperty(PROP_LOOKAT);
		}

		protected override bool OnSubInspectorGUI(Transform cameraMotorTransform)
		{
            EditorGUILayout.PropertyField(this.spLookAt);

            return false;
		}
	}
}