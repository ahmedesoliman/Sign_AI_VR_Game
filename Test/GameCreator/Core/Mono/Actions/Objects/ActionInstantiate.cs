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
	public class ActionInstantiate : IAction 
	{
        public TargetGameObject prefab = new TargetGameObject();
        public TargetPosition initLocation = new TargetPosition();

        [Space]
        public VariableProperty storeInstance = new VariableProperty();

		// EXECUTABLE: ----------------------------------------------------------------------------
		
        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject prefabValue = this.prefab.GetGameObject(target);
            if (prefabValue != null)
            {
                Vector3 position = this.initLocation.GetPosition(target, Space.Self);
                Quaternion rotation = this.initLocation.GetRotation(target);

                GameObject instance = Instantiate(prefabValue, position, rotation);
                if (instance != null) this.storeInstance.Set(instance, target);
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Instantiate";
		private const string NODE_TITLE = "Instantiate {0}";

        private static readonly GUIContent GC_STORE = new GUIContent("Store (optional)");

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spPrefab;
		private SerializedProperty spInitLocation;
        private SerializedProperty spStore;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.prefab);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spPrefab = this.serializedObject.FindProperty("prefab");
			this.spInitLocation = this.serializedObject.FindProperty("initLocation");
            this.spStore = this.serializedObject.FindProperty("storeInstance");
        }

		protected override void OnDisableEditorChild ()
		{
			this.spPrefab = null;
			this.spInitLocation = null;
            this.spStore = null;
        }

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spPrefab);
			EditorGUILayout.PropertyField(this.spInitLocation);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spStore, GC_STORE);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}