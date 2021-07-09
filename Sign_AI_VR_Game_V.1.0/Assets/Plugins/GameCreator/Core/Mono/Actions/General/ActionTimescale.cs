namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionTimescale : IAction 
	{
		public NumberProperty timeScale = new NumberProperty(1.0f);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            float timeScaleValue = this.timeScale.GetValue(target);
            TimeManager.Instance.SetTimeScale(timeScaleValue);
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR
        
		public static new string NAME = "General/Time Scale";
		private const string NODE_TITLE = "Change Time Scale to {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTimeScale;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.timeScale);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTimeScale = this.serializedObject.FindProperty("timeScale");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spTimeScale = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spTimeScale);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}