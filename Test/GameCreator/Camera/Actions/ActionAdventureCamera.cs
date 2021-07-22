namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionAdventureCamera : IAction
	{
        public bool mainCameraMotor = false;
        public CameraMotor cameraMotor;

        public bool allowOrbitInput = true;
        public bool allowZoom = true;
        public bool avoidWallClip = true;

        public bool changeTargetOffset = false;
        public bool changePivotOffset = false;
        public float duration = 0.2f;

        [Indent] public Vector3 targetOffset = Vector3.zero;
        [Indent] public Vector3 pivotOffset = Vector3.zero;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            CameraMotor motor = (this.mainCameraMotor ? CameraMotor.MAIN_MOTOR : this.cameraMotor);
            if (motor != null && motor.cameraMotorType.GetType() == typeof(CameraMotorTypeAdventure))
            {
                CameraMotorTypeAdventure adventureMotor = (CameraMotorTypeAdventure)motor.cameraMotorType;
                adventureMotor.allowOrbitInput = this.allowOrbitInput;
                adventureMotor.allowZoom = this.allowZoom;
                adventureMotor.avoidWallClip = this.avoidWallClip;
            }

            return !this.changeTargetOffset && !this.changePivotOffset;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            CameraMotor motor = (this.mainCameraMotor ? CameraMotor.MAIN_MOTOR : this.cameraMotor);
            if (motor != null && motor.cameraMotorType.GetType() == typeof(CameraMotorTypeAdventure))
            {
                float initTime = Time.time;
                CameraMotorTypeAdventure adventureMotor = (CameraMotorTypeAdventure)motor.cameraMotorType;

                Vector3 aTargetOffset = adventureMotor.targetOffset;
                Vector3 bTargetOffset = this.changeTargetOffset ? this.targetOffset : aTargetOffset;

                Vector3 aPivotOffset = adventureMotor.pivotOffset;
                Vector3 bPivotOffset = this.changePivotOffset ? this.pivotOffset : aPivotOffset;

                while (initTime + this.duration >= Time.time)
                {
                    float t = Mathf.Clamp01((Time.time - initTime) / this.duration);
                    adventureMotor.targetOffset = Vector3.Lerp(aTargetOffset, bTargetOffset, t);
                    adventureMotor.pivotOffset = Vector3.Lerp(aPivotOffset, bPivotOffset, t);

                    yield return null;
                }

                adventureMotor.targetOffset = bTargetOffset;
                adventureMotor.pivotOffset = bPivotOffset;
            }

            yield return 0;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Camera/Adventure Camera Settings";
        private const string NODE_TITLE = "Change {0} Adventure Camera settings";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spMainCameraMotor;
        private SerializedProperty spCameraMotor;

        private SerializedProperty spAllowOrbitInput;
        private SerializedProperty spAllowZoom;
        private SerializedProperty spAvoidWallClip;

        private SerializedProperty spChangeTargetOffset;
        private SerializedProperty spChangePivotOffset;
        private SerializedProperty spDuration;
        private SerializedProperty spTargetOffset;
        private SerializedProperty spPivotOffset;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
		{
            string motor = (this.mainCameraMotor
                ? "[Main Camera]"
                : (this.cameraMotor == null ? "none" : this.cameraMotor.gameObject.name)
            );
            
			return string.Format(NODE_TITLE, motor);
		}

		protected override void OnEnableEditorChild ()
		{
            this.spMainCameraMotor = this.serializedObject.FindProperty("mainCameraMotor");
            this.spCameraMotor = this.serializedObject.FindProperty("cameraMotor");
            this.spAllowOrbitInput = this.serializedObject.FindProperty("allowOrbitInput");
            this.spAllowZoom = this.serializedObject.FindProperty("allowZoom");
            this.spAvoidWallClip = this.serializedObject.FindProperty("avoidWallClip");

            this.spChangeTargetOffset = this.serializedObject.FindProperty("changeTargetOffset");
            this.spChangePivotOffset = this.serializedObject.FindProperty("changePivotOffset");
            this.spDuration = this.serializedObject.FindProperty("duration");
            this.spTargetOffset = this.serializedObject.FindProperty("targetOffset");
            this.spPivotOffset = this.serializedObject.FindProperty("pivotOffset");
        }

		protected override void OnDisableEditorChild ()
		{
            this.spMainCameraMotor = null;
            this.spCameraMotor = null;
            this.spAllowOrbitInput = null;
            this.spAllowZoom = null;
            this.spAvoidWallClip = null;
            this.spChangeTargetOffset = null;
            this.spChangePivotOffset = null;
            this.spDuration = null;
            this.spTargetOffset = null;
            this.spPivotOffset = null;
        }

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spMainCameraMotor);
            EditorGUI.BeginDisabledGroup(this.spMainCameraMotor.boolValue);
            EditorGUILayout.PropertyField(this.spCameraMotor);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(this.spAllowOrbitInput);
            EditorGUILayout.PropertyField(this.spAllowZoom);
            EditorGUILayout.PropertyField(this.spAvoidWallClip);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spDuration);
            EditorGUILayout.PropertyField(this.spChangeTargetOffset);
            if (this.spChangeTargetOffset.boolValue)
            {
                EditorGUILayout.PropertyField(this.spTargetOffset);
            }

            EditorGUILayout.PropertyField(this.spChangePivotOffset);
            if (this.spChangePivotOffset.boolValue)
            {
                EditorGUILayout.PropertyField(this.spPivotOffset);
            }

            this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
