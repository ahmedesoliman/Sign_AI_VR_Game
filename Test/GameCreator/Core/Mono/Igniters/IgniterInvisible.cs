namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class IgniterInvisible : Igniter 
	{
		#if UNITY_EDITOR
		public new static string NAME = "General/On Become Invisible";
        #endif

        private void OnBecameInvisible()
        {
            this.ExecuteTrigger(gameObject);
        }
    }
}