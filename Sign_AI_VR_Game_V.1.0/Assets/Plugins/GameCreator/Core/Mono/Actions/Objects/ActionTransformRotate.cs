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
    public class ActionTransformRotate : IAction
    {
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.Player);

        [Space]
        public Space space = Space.Self;
        public Vector3 rotation = new Vector3(90f, 0f, 0f);

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
                Quaternion rotation1 = targetTrans.rotation;
                Quaternion rotation2 = Quaternion.identity;
                switch (this.space)
                {
                    case Space.Self:
                        rotation2 = (
                            targetTrans.rotation *
                            Quaternion.Euler(this.rotation)
                        );
                        break;

                    case Space.World: 
                        rotation2 = Quaternion.Euler(this.rotation); 
                        break;
                }

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

        public static new string NAME = "Object/Transform Rotate";
        private const string NODE_TITLE = "Rotate to {0} in {1} Space";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spSpace;
        private SerializedProperty spRotation;
        private SerializedProperty spDuration;
        private SerializedProperty spEasing;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, this.rotation, this.space);
        }

        protected override void OnEnableEditorChild()
        {
            this.spTarget = serializedObject.FindProperty("target");
            this.spSpace = serializedObject.FindProperty("space");
            this.spRotation = serializedObject.FindProperty("rotation");
            this.spDuration = serializedObject.FindProperty("duration");
            this.spEasing = serializedObject.FindProperty("easing");
        }

        protected override void OnDisableEditorChild()
        {
            this.spTarget = null;
            this.spSpace = null;
            this.spRotation = null;
            this.spDuration = null;
            this.spEasing = null;
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spTarget);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spSpace);
            EditorGUILayout.PropertyField(this.spRotation);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spDuration);
            EditorGUILayout.PropertyField(this.spEasing);

            this.serializedObject.ApplyModifiedProperties();
        }

        #endif
    }
}