namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using UnityEngine.Audio;
    using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
    #endif

    [AddComponentMenu("")]
	public class ActionAudioSnapshot : IAction 
	{
        public AudioMixerSnapshot snapshot;
        public float duration = 0.5f;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.snapshot != null)
            {
                this.snapshot.TransitionTo(this.duration);
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Audio/Audio Snapshot";
		private const string NODE_TITLE = "Transition to {0} in {1}";

		public override string GetNodeTitle()
		{
            return string.Format(
                NODE_TITLE,
                this.snapshot != null ? this.snapshot.name : "(none)",
                this.duration
            );
		}

		#endif
	}
}