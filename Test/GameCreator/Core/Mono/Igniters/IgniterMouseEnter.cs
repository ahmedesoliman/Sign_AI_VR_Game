namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class IgniterMouseEnter : Igniter 
	{
		#if UNITY_EDITOR
        public new static string NAME = "Input/On Mouse Enter";
		public new static bool REQUIRES_COLLIDER = true;
		#endif

		private void OnMouseEnter()
		{
            this.ExecuteTrigger(gameObject);
		}
	}
}