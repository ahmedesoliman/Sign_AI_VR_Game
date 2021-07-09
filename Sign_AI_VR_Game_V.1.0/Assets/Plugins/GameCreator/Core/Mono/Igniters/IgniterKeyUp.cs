namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class IgniterKeyUp : Igniter 
	{
		public KeyCode keyCode = KeyCode.Space;

		#if UNITY_EDITOR
        public new static string NAME = "Input/On Key Up";
		#endif

		private void Update()
		{
			if (Input.GetKeyUp(this.keyCode))
			{
                this.ExecuteTrigger(gameObject);
			}
		}
	}
}