namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
	using GameCreator.Core.Hooks;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionHeadTrack : IAction 
	{
		public enum TRACK_STATE
		{
			TrackTarget,
			Untrack
		}

        public TargetCharacter character = new TargetCharacter();
		public TRACK_STATE trackState = TRACK_STATE.TrackTarget;
        public TargetGameObject trackTarget = new TargetGameObject();
        public float speed = 0.5f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character sourceCharacter = this.character.GetCharacter(target);
            if (sourceCharacter != null)
            {
                CharacterHeadTrack headTrack = sourceCharacter.GetHeadTracker();
                if (headTrack != null)
                {
                    switch (this.trackState)
                    {
                        case TRACK_STATE.TrackTarget:
                            headTrack.Track(this.trackTarget.GetTransform(target), this.speed);
                            break;

                        case TRACK_STATE.Untrack:
                            headTrack.Untrack();
                            break;
                    }
                }
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Character/Head Track";
		private const string NODE_TITLE = "{0} {1} {2}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spCharacter;
		private SerializedProperty spTrackState;
		private SerializedProperty spTrackTarget;
        private SerializedProperty spSpeed;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            string source = this.character.ToString();

            string target = this.trackTarget.ToString();
            if (this.trackState == TRACK_STATE.Untrack) target = "";

            string track = "head ";
            if (this.trackState == TRACK_STATE.TrackTarget) track += "track";
            if (this.trackState == TRACK_STATE.Untrack) track += "untrack";


			return string.Format(
                NODE_TITLE, 
                source,
                track,
                target
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spCharacter = this.serializedObject.FindProperty("character");
			this.spTrackState = this.serializedObject.FindProperty("trackState");
			this.spTrackTarget = this.serializedObject.FindProperty("trackTarget");
            this.spSpeed = this.serializedObject.FindProperty("speed");

        }

		protected override void OnDisableEditorChild ()
		{
			this.spCharacter = null;
			this.spTrackState = null;
			this.spTrackTarget = null;
            this.spSpeed = null;
        }

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();
				
            EditorGUILayout.PropertyField(this.spCharacter);
            EditorGUILayout.PropertyField(this.spTrackState);
            EditorGUILayout.PropertyField(this.spSpeed);

            if (this.spTrackState.intValue == (int)TRACK_STATE.TrackTarget)
			{
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.spTrackTarget);
                EditorGUI.indentLevel--;
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}