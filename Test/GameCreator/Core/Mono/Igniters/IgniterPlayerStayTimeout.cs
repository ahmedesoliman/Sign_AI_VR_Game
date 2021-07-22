namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;

	[AddComponentMenu("")]
	public class IgniterPlayerStayTimeout : Igniter 
	{
		#if UNITY_EDITOR
        public new static string NAME = "Object/On Player Stay Timeout";
		public new static bool REQUIRES_COLLIDER = true;
        #endif

        public float duration = 2.0f;
        private float startTime = 0.0f;
        private bool hasBeenExecuted = false;

		private void OnTriggerEnter(Collider c)
		{
            if (this.IsColliderPlayer(c))
			{
                this.startTime = Time.time;
                this.hasBeenExecuted = false;
			}
		}

        private void OnTriggerExit(Collider c)
        {
            if (this.IsColliderPlayer(c))
            {
                this.startTime = Time.time;
            }
        }

        private void OnTriggerStay(Collider c)
        {
            bool timeout = this.startTime + this.duration < Time.time;
            if (this.IsColliderPlayer(c) && timeout && !this.hasBeenExecuted)
            {
                this.hasBeenExecuted = true;
                this.ExecuteTrigger(c.gameObject);
            }
        }
    }
}