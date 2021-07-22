namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class IgniterMouseExit : Igniter 
	{
		#if UNITY_EDITOR
        public new static string NAME = "Input/On Mouse Exit";
		public new static bool REQUIRES_COLLIDER = true;
		#endif

		private void OnMouseExit()
		{
            this.ExecuteTrigger(gameObject);
		}
	}
}