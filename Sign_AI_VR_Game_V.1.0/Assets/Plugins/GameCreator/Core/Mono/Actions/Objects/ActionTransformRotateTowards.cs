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
    public class ActionTransformRotateTowards : IAction
    {
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.Player);
        public TargetPosition lookAt = new TargetPosition(TargetPosition.Target.Invoker);

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
                Vector3 targetRotation = this.lookAt.GetPosition(target);
                Quaternion rotation1 = targetTrans.rotation;
                Quaternion rotation2 = Quaternion.LookRotation(targetRotation - targetTrans.position);

                float vDuration = this.duration.GetValue(target);
                float initTime = Time.time;

                while (Time.time - initTime < vDuration && !this.forceStop)
                {
                    if (targetTrans == null) break;
                    float t = (Time.time - initTime)/vDuration;
                    float easeValue = Easing.GetEase(this.easing, 0.0f, 1.0f, t);

                    targetTrans.rotation = Quaternion.LerpUnclamped(
                        rotation1,
                        rotation2,
                        easeValue
                    );

                    yield return null;
                }

                if (!this.forceStop && targetTrans != null)
                {
                    targetTrans.rotation = rotation2;
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

        public static new string NAME = "Object/Transform Rotate Towards";
        private const string NODE_TITLE = "Rotate towards {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
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
            this.spLookAt = serializedObject.FindProperty("lookAt");
            this.spDuration = serializedObject.FindProperty("duration");
            this.spEasing = serializedObject.FindProperty("easing");
        }

        protected override void OnDisableEditorChild()
        {
            this.spTarget = null;
            this.spLookAt = null;
            this.spDuration = null;
            this.spEasing = null;
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spTarget);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spLookAt);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spDuration);
            EditorGUILayout.PropertyField(this.spEasing);

            this.serializedObject.ApplyModifiedProperties();
        }

        #endif
    }
}