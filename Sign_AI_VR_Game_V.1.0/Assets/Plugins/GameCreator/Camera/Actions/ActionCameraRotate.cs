namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using GameCreator.Core;
    using GameCreator.Camera;
    using GameCreator.Variables;
    using UnityEngine;
	using UnityEngine.Events;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("")]
	public class ActionCameraRotate : IAction
	{
        public bool mainCamera = true;
        public CameraMotor cameraMotor;

		public NumberProperty pitch = new NumberProperty(10f);
        public NumberProperty yaw = new NumberProperty(0f);

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            CameraMotor motor = this.mainCamera ? CameraMotor.MAIN_MOTOR : this.cameraMotor;
            if (motor == null) return true;

            CameraMotorTypeAdventure motorTPS = motor.cameraMotorType as CameraMotorTypeAdventure;
            CameraMotorTypeFirstPerson motorFPS = motor.cameraMotorType as CameraMotorTypeFirstPerson;

            float x = this.pitch.GetValue(target);
            float y = this.yaw.GetValue(target);
            if (motorTPS != null) motorTPS.AddRotation(y, x);
            if (motorFPS != null) motorFPS.AddRotation(y, x);

            return true;
        }

		#if UNITY_EDITOR

        public static new string NAME = "Camera/Camera Rotate";
        private const string NODE_TITLE = "Change {0} camera Rotation";

        private const string MSG = "Only Adventure & FPS camera motors accepted";

        public override string GetNodeTitle()
        {
            string cameraName = (this.mainCamera
                ? "Main"
                : (this.cameraMotor == null ? "(none)" : this.cameraMotor.gameObject.name)
            );

            return string.Format(NODE_TITLE, cameraName);
        }

        private SerializedProperty spMainCamera;
        private SerializedProperty spCameraMotor;

        private SerializedProperty spX;
        private SerializedProperty spY;

        protected override void OnEnableEditorChild()
        {
            this.spMainCamera = this.serializedObject.FindProperty("mainCamera");
            this.spCameraMotor = this.serializedObject.FindProperty("cameraMotor");

            this.spX = this.serializedObject.FindProperty("pitch");
            this.spY = this.serializedObject.FindProperty("yaw");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            EditorGUILayout.HelpBox(MSG, MessageType.Info);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(this.spMainCamera);
            EditorGUI.BeginDisabledGroup(this.spMainCamera.boolValue);
            EditorGUILayout.PropertyField(this.spCameraMotor);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spX);
            EditorGUILayout.PropertyField(this.spY);

            this.serializedObject.ApplyModifiedProperties();
        }

        #endif
    }
}
