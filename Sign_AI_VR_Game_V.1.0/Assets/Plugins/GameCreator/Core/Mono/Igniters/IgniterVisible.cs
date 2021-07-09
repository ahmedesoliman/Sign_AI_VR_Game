namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class IgniterVisible : Igniter 
	{
		#if UNITY_EDITOR
		public new static string NAME = "General/On Become Visible";
        #endif

        private void OnBecameVisible()
        {
            this.ExecuteTrigger(gameObject);
        }
    }
}