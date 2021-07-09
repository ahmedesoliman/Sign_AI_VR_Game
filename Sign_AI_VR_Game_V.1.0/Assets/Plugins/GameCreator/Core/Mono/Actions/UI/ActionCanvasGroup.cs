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
	public class ActionCanvasGroup : IAction
	{
        public CanvasGroup canvasGroup;

        [Range(0.0f, 5.0f)]
        public float duration = 0.5f;


        public NumberProperty alpha = new NumberProperty(0.0f);
        public BoolProperty interactible = new BoolProperty(true);
        public BoolProperty blockRaycasts = new BoolProperty(true);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.duration <= 0.0f)
            {
                this.canvasGroup.alpha = this.alpha.GetValue(target);
                this.canvasGroup.interactable = this.interactible.GetValue(target);
                this.canvasGroup.blocksRaycasts = this.blockRaycasts.GetValue(target);
                return true;
            }

            return false;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
		{
            if (this.canvasGroup != null)
            {
                this.canvasGroup.interactable = this.interactible.GetValue(target);
                this.canvasGroup.blocksRaycasts = this.blockRaycasts.GetValue(target);
                float targetAlpha = this.alpha.GetValue(target);

                if (this.duration > 0.0f)
                {
                    float currentAlpha = this.canvasGroup.alpha;
                    float startTime = Time.unscaledTime;

                    WaitUntil waitUntil = new WaitUntil(() =>
                    {
                        float t = (Time.unscaledTime - startTime) / this.duration;
                        this.canvasGroup.alpha = Mathf.Lerp(
                            currentAlpha,
                            targetAlpha, 
                            t
                        );

                        return t > 1.0f;
                    });

                    yield return waitUntil;
                }

                this.canvasGroup.alpha = targetAlpha;
            }

			yield return 0;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "UI/Canvas Group";
		private const string NODE_TITLE = "Change CanvasGroup settings";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spCanvasGroup;
        private SerializedProperty spDuration;
        private SerializedProperty spAlpha;
        private SerializedProperty spInteractible;
        private SerializedProperty spBlockRaycasts;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return NODE_TITLE;
		}

		protected override void OnEnableEditorChild ()
		{
            this.spCanvasGroup = this.serializedObject.FindProperty("canvasGroup");
            this.spDuration = this.serializedObject.FindProperty("duration");
            this.spAlpha = this.serializedObject.FindProperty("alpha");
            this.spInteractible = this.serializedObject.FindProperty("interactible");
            this.spBlockRaycasts = this.serializedObject.FindProperty("blockRaycasts");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spCanvasGroup = null;
            this.spDuration = null;
            this.spAlpha = null;
            this.spInteractible = null;
            this.spBlockRaycasts = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spCanvasGroup);
            EditorGUILayout.PropertyField(this.spDuration);
            EditorGUILayout.PropertyField(this.spAlpha);
            EditorGUILayout.PropertyField(this.spInteractible);
            EditorGUILayout.PropertyField(this.spBlockRaycasts);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
