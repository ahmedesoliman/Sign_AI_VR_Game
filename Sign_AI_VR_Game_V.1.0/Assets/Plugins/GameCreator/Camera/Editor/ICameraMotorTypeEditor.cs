namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	public class ICameraMotorTypeEditor : Editor 
	{
		private const string PROP_LOOKAT = "lookAt";

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool initialize = true;

        private SerializedProperty spSetProperties;
        private SerializedProperty spProjection;

        private SerializedProperty spCameraSize;
        private SerializedProperty spFieldOfView;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            if (target == null) return;

            this.spSetProperties = serializedObject.FindProperty("setCameraProperties");
            this.spProjection = serializedObject.FindProperty("projection");

            this.spCameraSize = serializedObject.FindProperty("cameraSize");
            this.spFieldOfView = serializedObject.FindProperty("fieldOfView");
        }

        // SEALED METHODS: ------------------------------------------------------------------------

        public bool PaintInspectorMotor(Transform cameraMotorTransform)
		{
            serializedObject.Update();

            if (this.initialize)
            {
                this.OnSubEnable();
                this.initialize = false;
            }

            EditorGUILayout.PropertyField(this.spSetProperties);
            if (this.spSetProperties.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.spProjection);
                switch (this.spProjection.enumValueIndex)
                {
                    case 0:
                        EditorGUILayout.PropertyField(this.spCameraSize);
                        break;

                    case 1:
                        EditorGUILayout.PropertyField(this.spFieldOfView);
                        break;
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();

            bool forceRepaint = this.OnSubInspectorGUI(cameraMotorTransform);

            serializedObject.ApplyModifiedProperties();

            return forceRepaint;
		}

        public bool PaintSceneMotor(Transform cameraMotorTransform)
        {
            if (this.initialize)
            {
                this.OnSubEnable();
                this.initialize = false;
            }

            return this.OnSubSceneGUI(cameraMotorTransform);
        }

		// VIRTUAL METHODS: -----------------------------------------------------------------------

		public virtual void OnCreate(Transform cameraMotorTransform) {}

        protected virtual void OnSubEnable() 
        { 
            return; 
        }

        protected virtual bool OnSubInspectorGUI(Transform cameraMotorTransform)
        {
            return false;
        }

		public virtual bool OnSubSceneGUI(Transform cameraMotorTransform) 
		{ 
			return false; 
		}

        public virtual bool ShowPreviewCamera()
        {
            return true;
        }
    }
}