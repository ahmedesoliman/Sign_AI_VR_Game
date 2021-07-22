namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionWait : IAction 
	{
		public float waitTime = 0.0f;
        private bool forceStop = false;

		// EXECUTABLE: ----------------------------------------------------------------------------
		
        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
		{
            this.forceStop = false;

            float stopTime = Time.time + this.waitTime;
            WaitUntil waitUntil = new WaitUntil(() => Time.time > stopTime || this.forceStop);

            yield return waitUntil;
			yield return 0;
		}

        public override void Stop()
        {
            this.forceStop = true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "General/Wait";
		private const string NODE_TITLE = "Wait {0} second{1}";

		private static readonly GUIContent GUICONTENT_WAITTIME = new GUIContent("Time to wait (s)");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spWaitTime;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, this.waitTime, (this.waitTime == 1f ? "" : "s"));
		}

		protected override void OnEnableEditorChild()
		{
			this.spWaitTime = this.serializedObject.FindProperty("waitTime");
		}

		protected override void OnDisableEditorChild()
		{
			this.spWaitTime = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spWaitTime, GUICONTENT_WAITTIME);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}