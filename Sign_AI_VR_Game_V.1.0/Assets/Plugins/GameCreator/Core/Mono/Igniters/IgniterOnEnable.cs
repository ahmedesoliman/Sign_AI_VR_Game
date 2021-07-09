namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class IgniterOnEnable : Igniter 
	{
		#if UNITY_EDITOR
		public new static string NAME = "General/On Enable";
		#endif

		private new void OnEnable()
		{
            base.OnEnable();
            this.ExecuteTrigger(gameObject);
		}
	}
}