namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using GameCreator.Core.Hooks;
    using GameCreator.Variables;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("")]
    public class ActionTransformMove : IAction
    {
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.Player);

        public TargetPosition moveTo = new TargetPosition();

        public bool rotate = true;
        public TargetPosition lookAt = new TargetPosition(TargetPosition.Target.Invoker);
        public Space space = Space.World;

        public NumberProperty duration = new NumberProperty(1.0f);
        public Easing.EaseType easing = Easing.EaseType.QuadInOut;

        private bool forceStop = false;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            this.forceStop = false;
            Transform targetTrans = this.target.GetTransform(target);
            if (targetTrans != null)
            {
                Vector3 position1 = targetTrans.position;
                Vector3 position2 = this.moveTo.GetPosition(target);

                if (this.moveTo.target == TargetPosition.Target.Position &&
                    this.space == Space.Self)
                {
                    position2 = targetTrans.TransformPoint(position2);
                }

                Vector3 targetRotation = (this.lookAt.target == TargetPosition.Target.Invoker
                    ? position2
                    : this.lookAt.GetPosition(target)
                );

                Quaternion rotation1 = targetTrans.rotation;
                Quaternion rotation2 = Quaternion.LookRotation(targetRotation - targetTrans.position);

                float vDuration = this.duration.GetValue(target);
                float initTime = Time.time;

                while (Time.time - initTime < vDuration && !this.forceStop)
                {
                    if (targetTrans == null) break;
                    float t = (Time.time - initTime)/vDuration;
                    float easeValue = Easing.GetEase(this.easing, 0.0f, 1.0f, t);

                    targetTrans.position = Vector3.LerpUnclamped(
                        position1,
                        position2,
                        easeValue
                    );

                    if (this.rotate)
                    {
                        targetTrans.rotation = Quaternion.LerpUnclamped(
                            rotation1,
                            rotation2,
                            easeValue
                        );
                    }

                    yield return null;
                }

                if (!this.forceStop && targetTrans != null)
                {
                    targetTrans.position = position2;
                }

            }

            yield return 0;
        }

        public override void Stop()
        {
            this.forceStop = true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Object/Transform Move";
        private const string NODE_TITLE = "Move {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spMoveTo;
        private SerializedProperty spSpace;
        private SerializedProperty spRotate;
        private SerializedProperty spLookAt;
        private SerializedProperty spDuration;
        private SerializedProperty spEasing;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, this.target);
        }

        protected override void OnEnableEditorChild()
        {
            this.spTarget = serializedObject.FindProperty("target");
            this.spMoveTo = serializedObject.FindProperty("moveTo");
            this.spSpace = serializedObject.FindProperty("space");
            this.spRotate = serializedObject.FindProperty("rotate");
            this.spLookAt = serializedObject.FindProperty("lookAt");
            this.spDuration = serializedObject.FindProperty("duration");
            this.spEasing = serializedObject.FindProperty("easing");
        }

        protected override void OnDisableEditorChild()
        {
            this.spTarget = null;
            this.spMoveTo = null;
            this.spSpace = null;
            this.spRotate = null;
            this.spLookAt = null;
            this.spDuration = null;
            this.spEasing = null;
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spTarget);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spMoveTo);

            int targetIndex = this.spMoveTo.FindPropertyRelative("target").enumValueIndex;
            if (targetIndex == (int)TargetPosition.Target.Position)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.spSpace);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spRotate);
            EditorGUI.BeginDisabledGroup(!this.spRotate.boolValue);
            EditorGUILayout.PropertyField(this.spLookAt);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spDuration);
            EditorGUILayout.PropertyField(this.spEasing);

            this.serializedObject.ApplyModifiedProperties();
        }

        #endif
    }
}