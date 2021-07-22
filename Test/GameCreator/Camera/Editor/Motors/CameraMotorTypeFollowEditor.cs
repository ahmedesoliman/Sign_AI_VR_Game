namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(CameraMotorTypeFollow))]
	public class CameraMotorTypeFollowEditor : ICameraMotorTypeEditor 
	{
        private const string PROP_ANCHOR = "anchor";
        private const string PROP_ANCHOR_OFFSET = "anchorOffset";
        private const string PROP_LOOKAT = "lookAt";

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private SerializedProperty spAnchor;
        private SerializedProperty spAnchorOffset;
        private SerializedProperty spLookAt;

		// INITIALIZE: -------------------------------------------------------------------------------------------------

		protected override void OnSubEnable()
		{
            this.spAnchor = serializedObject.FindProperty(PROP_ANCHOR);
            this.spAnchorOffset = serializedObject.FindProperty(PROP_ANCHOR_OFFSET);
            this.spLookAt = serializedObject.FindProperty(PROP_LOOKAT);
		}

        public override bool ShowPreviewCamera()
        {
            return false;
        }

        // INSPECTOR GUI: ----------------------------------------------------------------------------------------------

        protected override bool OnSubInspectorGUI (Transform cameraMotorTransform)
		{
			serializedObject.Update();

            EditorGUILayout.PropertyField(this.spAnchor);
            EditorGUILayout.PropertyField(this.spAnchorOffset);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spLookAt);

			serializedObject.ApplyModifiedProperties();
			return false;
		}
	}
}