namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using UnityEngine.Playables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionTimeline : IAction
	{
        public enum Operation
        {
            Play,
            Pause,
            Stop
        }

        public PlayableDirector director;
        public Operation operation = Operation.Play;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.director != null)
            {
                switch (this.operation)
                {
                    case Operation.Play: this.director.Play(); break;
                    case Operation.Pause: this.director.Pause(); break;
                    case Operation.Stop: this.director.Stop(); break;
                }
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Timeline";
        private const string NODE_TITLE = "{0} Timeline {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spDirector;
        private SerializedProperty spOperation;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(
                NODE_TITLE, 
                this.operation.ToString(),
                (this.director == null ? "none" : this.director.gameObject.name)
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spDirector = this.serializedObject.FindProperty("director");
            this.spOperation = this.serializedObject.FindProperty("operation");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spDirector = null;
            this.spOperation = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spDirector);
            EditorGUILayout.PropertyField(this.spOperation);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
