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
	public class ActionStopSound : IAction 
	{
		public AudioClip audioClip;

        [Range(0f, 5f)]
        public float fadeOut;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            AudioManager.Instance.StopSound(this.audioClip, fadeOut);
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Audio/Stop Sound";
		private const string NODE_TITLE = "Stop Sound {0} {1}";

        public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE,
				this.audioClip == null ? "unknown" : this.audioClip.name,
				this.fadeOut > 0f ? "(" + this.fadeOut.ToString() + "s)" : ""
			);
		}

		#endif
	}
}