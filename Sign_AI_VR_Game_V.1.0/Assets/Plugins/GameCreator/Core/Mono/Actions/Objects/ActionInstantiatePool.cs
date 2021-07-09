namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core.Hooks;
    using GameCreator.Variables;
    using GameCreator.Pool;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("")]
	public class ActionInstantiatePool : IAction 
	{
        public TargetGameObject prefab = new TargetGameObject();
        public TargetPosition initLocation = new TargetPosition();

		// EXECUTABLE: ----------------------------------------------------------------------------
		
        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject prefabValue = this.prefab.GetGameObject(target);
            if (prefabValue != null)
            {
                Vector3 position = this.initLocation.GetPosition(target, Space.Self);
                Quaternion rotation = this.initLocation.GetRotation(target);

                GameObject instance = PoolManager.Instance.Pick(prefabValue);
                instance.transform.SetPositionAndRotation(position, rotation);
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Instantiate from Pool";
		private const string NODE_TITLE = "Instantiate {0} from Pool";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spPrefab;
		private SerializedProperty spInitLocation;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.prefab);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spPrefab = this.serializedObject.FindProperty("prefab");
			this.spInitLocation = this.serializedObject.FindProperty("initLocation");
        }

		protected override void OnDisableEditorChild ()
		{
			this.spPrefab = null;
			this.spInitLocation = null;
        }

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spPrefab);
			EditorGUILayout.PropertyField(this.spInitLocation);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}