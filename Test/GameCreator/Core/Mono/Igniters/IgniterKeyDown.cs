namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class IgniterKeyDown : Igniter 
	{
		public KeyCode keyCode = KeyCode.Space;

		#if UNITY_EDITOR
        public new static string NAME = "Input/On Key Down";
		#endif

		private void Update()
		{
			if (Input.GetKeyDown(this.keyCode))
			{
                this.ExecuteTrigger(gameObject);
			}
		}
	}
}