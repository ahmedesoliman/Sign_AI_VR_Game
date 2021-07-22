namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.SceneManagement;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionLoadScene : IAction 
	{
        public StringProperty sceneName = new StringProperty();
        public LoadSceneMode mode = LoadSceneMode.Single;
        public bool async = false;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            if (this.async)
            {
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(
                    this.sceneName.GetValue(target), 
                    this.mode
                );

                yield return asyncOperation;
            }
            else
            {
                SceneManager.LoadScene(this.sceneName.GetValue(target), this.mode);
                yield return null;
            }

            yield return 0;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Scene/Load Scene";
		private const string NODE_TITLE = "Load scene {0}{1}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spSceneName;
        private SerializedProperty spMode;
        private SerializedProperty spAsync;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
		{
			return string.Format(
                NODE_TITLE, 
                this.sceneName,
                (this.async ? " (Async)" : "")
            );
		}

		protected override void OnEnableEditorChild ()
		{
			this.spSceneName = this.serializedObject.FindProperty("sceneName");
            this.spMode = this.serializedObject.FindProperty("mode");
            this.spAsync = this.serializedObject.FindProperty("async");
        }

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spSceneName);
            EditorGUILayout.PropertyField(this.spMode);
            EditorGUILayout.PropertyField(this.spAsync);

            this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}