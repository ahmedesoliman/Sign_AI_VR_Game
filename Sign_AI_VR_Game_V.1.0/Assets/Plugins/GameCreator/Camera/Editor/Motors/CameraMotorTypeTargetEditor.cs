namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

    [CustomEditor(typeof(CameraMotorTypeTarget))]
	public class CameraMotorTypeTargetEditor : ICameraMotorTypeEditor 
	{
        private const string PROP_ANCHOR_DISTANCE = "anchorDistance";
        private const string PROP_HORIZONTAL_OFFSET = "horizontalOffset";
        private const string PROP_ANCHOR = "anchor";
        private const string PROP_ANCHOR_OFFSET = "anchorOffset";
        private const string PROP_TARGET = "target";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spAnchorDistance;
        private SerializedProperty spHorizontalOffset;
		private SerializedProperty spAnchor;
        private SerializedProperty spAnchorOffset;
        private SerializedProperty spTarget;

		// INITIALIZE: -------------------------------------------------------------------------------------------------

		protected override void OnSubEnable()
		{
            this.spAnchorDistance = serializedObject.FindProperty(PROP_ANCHOR_DISTANCE);
            this.spHorizontalOffset = serializedObject.FindProperty(PROP_HORIZONTAL_OFFSET);
            this.spAnchor = serializedObject.FindProperty(PROP_ANCHOR);
            this.spAnchorOffset = serializedObject.FindProperty(PROP_ANCHOR_OFFSET);
            this.spTarget = serializedObject.FindProperty(PROP_TARGET);
		}

        public override bool ShowPreviewCamera()
        {
            return false;
        }

        // INSPECTOR GUI: ----------------------------------------------------------------------------------------------

        protected override bool OnSubInspectorGUI (Transform cameraMotorTransform)
		{
			serializedObject.Update();

            EditorGUILayout.PropertyField(this.spAnchorDistance);
            EditorGUILayout.PropertyField(this.spHorizontalOffset);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spAnchor);
            EditorGUILayout.PropertyField(this.spAnchorOffset);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spTarget);

			serializedObject.ApplyModifiedProperties();
			return false;
		}
	}
}