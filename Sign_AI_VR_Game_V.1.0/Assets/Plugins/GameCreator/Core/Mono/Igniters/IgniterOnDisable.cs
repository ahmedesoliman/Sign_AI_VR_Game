namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class IgniterOnDisable : Igniter 
	{
		#if UNITY_EDITOR
		public new static string NAME = "General/On Disable";
		#endif

		private void OnDisable()
		{
            this.ExecuteTrigger(gameObject);
		}
	}
}