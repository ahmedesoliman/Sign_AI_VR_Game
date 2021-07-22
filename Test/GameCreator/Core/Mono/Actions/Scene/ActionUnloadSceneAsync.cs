namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using UnityEngine.SceneManagement;
	using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionUnloadSceneAsync : IAction
	{
        public StringProperty sceneName = new StringProperty();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            string scene = this.sceneName.GetValue(target);
            AsyncOperation async = SceneManager.UnloadSceneAsync(scene);

            yield return async;
            yield return 0;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Scene/Unload Scene Async";
		private const string NODE_TITLE = "Unload scene {0} async";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spSceneName;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.sceneName);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spSceneName = this.serializedObject.FindProperty("sceneName");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spSceneName = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spSceneName);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
