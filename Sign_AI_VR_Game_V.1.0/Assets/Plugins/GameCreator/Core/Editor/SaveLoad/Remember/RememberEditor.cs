namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

    public abstract class RememberEditor : Editor
	{
        private const string MSG_ACTIVE1 = "Disabled components can not be initialized on start.";
        private const string MSG_ACTIVE2 = "This component won't work until its first enabled.";

        protected RememberBase remember;

        protected virtual void OnEnable()
        {
            this.remember = this.target as RememberBase;
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            if (!Application.isPlaying && !this.remember.isActiveAndEnabled)
            {
                EditorGUILayout.HelpBox(
                    string.Format("{0} {1}", MSG_ACTIVE1, MSG_ACTIVE2),
                    MessageType.Warning
                );
            }

            string comment = this.Comment();
            if (!string.IsNullOrEmpty(comment))
            {
                EditorGUILayout.HelpBox(comment, MessageType.None);
            }
            
            this.OnPaint();

            this.serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            GlobalEditorID.Paint(this.remember);
        }

        protected abstract void OnPaint();
        protected abstract string Comment();
    }
}