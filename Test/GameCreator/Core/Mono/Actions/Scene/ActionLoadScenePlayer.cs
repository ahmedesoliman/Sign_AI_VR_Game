using GameCreator.Characters;
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
	public class ActionLoadScenePlayer : IAction 
	{
        public StringProperty sceneName = new StringProperty();

        public Vector3 playerPosition = Vector3.zero;
        public Quaternion playerRotation = Quaternion.identity;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            PlayerCharacter.ON_LOAD_SCENE_DATA = new Character.OnLoadSceneData(
                this.playerPosition,
                this.playerRotation
            );

            SceneManager.LoadScene(this.sceneName.GetValue(target), LoadSceneMode.Single);
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Scene/Load Scene with Player";
		private const string NODE_TITLE = "Load scene {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spSceneName;
        private SerializedProperty spPosition;
        private SerializedProperty spRotation;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.sceneName);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spSceneName = this.serializedObject.FindProperty("sceneName");
            this.spPosition = this.serializedObject.FindProperty("playerPosition");
            this.spRotation = this.serializedObject.FindProperty("playerRotation");
        }

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spSceneName);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spPosition);

            Vector3 rotation = EditorGUILayout.Vector3Field(
                this.spRotation.displayName,
                this.spRotation.quaternionValue.eulerAngles
            );

            this.spRotation.quaternionValue = Quaternion.Euler(rotation);

            this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}