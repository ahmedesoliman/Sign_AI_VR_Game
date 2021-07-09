namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.Video;

	[AddComponentMenu("")]
	public class ActionVideoPlay : IAction
	{
		public enum Option
		{
			Play,
			Pause,
			Stop
		}
		
		public VideoPlayer videoPlayer;
		public Option option = Option.Play;

		public override bool InstantExecute(GameObject target, IAction[] actions, int index)
		{
			if (this.videoPlayer == null) return true;
			
			switch (this.option)
			{
				case Option.Play:
					this.videoPlayer.Play(); 
					break;
				
				case Option.Pause: 
					this.videoPlayer.Pause();
					break;
				
				case Option.Stop:
					this.videoPlayer.Stop(); 
					break;
			}
			
			return true;
        }

		#if UNITY_EDITOR
        public static new string NAME = "Application/Video Player";
        private const string NODE_TITLE = "{0} video {1}";
        
        public override string GetNodeTitle()
        {
	        return string.Format(
		        NODE_TITLE,
		        this.option,
		        this.videoPlayer != null ? this.videoPlayer.gameObject.name : "(none)"
	        );
        }
        
		#endif
	}
}
