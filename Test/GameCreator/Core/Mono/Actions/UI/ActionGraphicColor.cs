namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.UI;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionGraphicColor : IAction
	{
        public Graphic graphic;

        [Range(0.0f, 10.0f)]
        public float duration = 0.0f;

        public ColorProperty color = new ColorProperty(Color.white);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.duration <= 0.0f)
            {
                if (this.graphic != null) this.graphic.color = this.color.GetValue(target);
                return true;
            }

            return false;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
		{
            if (this.graphic != null)
            {
                Color currentColor = this.graphic.color;
                Color targetColor = this.color.GetValue(target);

                float startTime = Time.unscaledTime;
                WaitUntil waitUntil = new WaitUntil(() =>
                {
                    float t = (Time.unscaledTime - startTime) / this.duration;
                    this.graphic.color = Color.Lerp(currentColor, targetColor, t);

                    return t > 1.0f;
                });

                yield return waitUntil;
                this.graphic.color = targetColor;
            }

			yield return 0;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    public static new string NAME = "UI/Graphic Color";
		private const string NODE_TITLE = "Change Graphic Color";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spGraphic;
        private SerializedProperty spDuration;
        private SerializedProperty spColor;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return NODE_TITLE;
		}

		protected override void OnEnableEditorChild ()
		{
            this.spGraphic = this.serializedObject.FindProperty("graphic");
            this.spDuration = this.serializedObject.FindProperty("duration");
            this.spColor = this.serializedObject.FindProperty("color");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spGraphic = null;
            this.spDuration = null;
            this.spColor = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spGraphic);
            EditorGUILayout.PropertyField(this.spDuration);
            EditorGUILayout.PropertyField(this.spColor);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
