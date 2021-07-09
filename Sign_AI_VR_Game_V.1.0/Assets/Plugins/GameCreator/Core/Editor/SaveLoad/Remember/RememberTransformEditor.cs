namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

    [CustomEditor(typeof(RememberTransform))]
    public class RememberTransformEditor : RememberEditor
    {
        private SerializedProperty spRememberPosition;
        private SerializedProperty spRememberRotation;
        private SerializedProperty spRememberScale;

        protected override string Comment()
        {
            return "Automatically save and restore Transform properties when loading the game";
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            this.spRememberPosition = this.serializedObject.FindProperty("rememberPosition");
            this.spRememberRotation = this.serializedObject.FindProperty("rememberRotation");
            this.spRememberScale = this.serializedObject.FindProperty("rememberScale");
        }

        protected override void OnPaint()
        {
            EditorGUILayout.PropertyField(this.spRememberPosition);
            EditorGUILayout.PropertyField(this.spRememberRotation);
            EditorGUILayout.PropertyField(this.spRememberScale);
        }
    }
}