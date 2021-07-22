namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class IgniterStart : Igniter 
	{
		#if UNITY_EDITOR
		public new static string NAME = "General/On Start";
		#endif

		private void Start()
		{
            this.ExecuteTrigger(gameObject);
		}
	}
}