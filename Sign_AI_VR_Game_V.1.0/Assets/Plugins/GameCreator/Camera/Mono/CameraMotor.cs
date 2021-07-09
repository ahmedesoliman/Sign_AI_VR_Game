namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("Game Creator/Camera/Camera Motor", 100)]
	public class CameraMotor : MonoBehaviour 
	{
        public static CameraMotor MAIN_MOTOR;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool isMainCameraMotor = false;
		public ICameraMotorType cameraMotorType;

		// INITIALIZE: ----------------------------------------------------------------------------

		private void Awake()
		{
            if (this.isMainCameraMotor) MAIN_MOTOR = this;

            this.cameraMotorType.Initialize(this);
            Camera attachedCamera = GetComponent<Camera>();
            if (attachedCamera != null) attachedCamera.enabled = false;
		}

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.cameraMotorType == null) return;
            if (this.cameraMotorType.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
            {
                ICameraMotorType newCameraMotorType = gameObject.AddComponent(
                    this.cameraMotorType.GetType()
                ) as ICameraMotorType;

                EditorUtility.CopySerialized(this.cameraMotorType, newCameraMotorType);

                SerializedObject serializedObject = new SerializedObject(this);
                serializedObject.FindProperty("cameraMotorType").objectReferenceValue = newCameraMotorType;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
        #endif

        // GIZMOS: --------------------------------------------------------------------------------

        private void OnDrawGizmos()
		{
            Gizmos.DrawIcon(transform.position, "GameCreator/Camera/motor", true);
        }
	}
}