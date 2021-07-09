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
	public class ActionEnableComponent : IAction 
	{
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.GameObject);
        [Space] public string componentName = "";

        public BoolProperty enable = new BoolProperty(true);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (string.IsNullOrEmpty(this.componentName)) return true;

            GameObject targetGo = this.target.GetGameObject(target);
            Component component = targetGo.GetComponent(this.componentName);

            if (component != null)
            {
                bool value = this.enable.GetValue(target);
                if (component == null) return true;

                if (component is Renderer)
                {
                    (component as Renderer).enabled = value;
                }
                else if (component is Collider)
                {
                    (component as Collider).enabled = value;
                }
                else if (component is Animation)
                {
                    (component as Animation).enabled = value;
                }
                else if (component is Animator)
                {
                    (component as Animator).enabled = value;
                }
                else if (component is AudioSource)
                {
                    (component as AudioSource).enabled = value;
                }
                else if (component is MonoBehaviour)
                {
                    (component as MonoBehaviour).enabled = value;
                }
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Enable Component";
		private const string NODE_TITLE = "{0} component {1}";
        private const string SELECT_TEXT = "Select Target Component";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;
        private SerializedProperty spComponentName;
		private SerializedProperty spEnable;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE, 
                this.enable,
                this.componentName
            );
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTarget = this.serializedObject.FindProperty("target");
            this.spComponentName = this.serializedObject.FindProperty("componentName");
            this.spEnable = this.serializedObject.FindProperty("enable");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spTarget = null;
            this.spComponentName = null;
            this.spEnable = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spTarget);

            EditorGUILayout.PropertyField(this.spComponentName);
            this.spComponentName.stringValue = this.spComponentName.stringValue.Replace(
                " ", string.Empty
            );

            EditorGUILayout.PropertyField(this.spEnable);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}