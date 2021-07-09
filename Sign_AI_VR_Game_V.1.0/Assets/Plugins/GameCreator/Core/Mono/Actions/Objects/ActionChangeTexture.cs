namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionChangeTexture : IAction
	{
        public TargetGameObject target = new TargetGameObject();
        public Texture2DProperty texture = new Texture2DProperty();

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject go = this.target.GetGameObject(target);
            if (go != null)
            {
                Renderer render = go.GetComponent<Renderer>();
                if (render != null) render.material.mainTexture = this.texture.GetValue(target);
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Change Texture";
        private const string NODE_TITLE = "Change texture";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spTexture;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return NODE_TITLE;
		}

		protected override void OnEnableEditorChild ()
		{
            this.spTarget = this.serializedObject.FindProperty("target");
            this.spTexture = this.serializedObject.FindProperty("texture");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spTarget = null;
            this.spTexture = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spTarget);
            EditorGUILayout.PropertyField(this.spTexture);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
