namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class IgniterWhileKeyDown : Igniter 
	{
        public KeyCode key = KeyCode.Space;

		#if UNITY_EDITOR
        public new static string NAME = "Input/While Key Down";
		#endif

		private void Update()
		{
			if (Input.GetKey(this.key))
			{
                this.ExecuteTrigger(gameObject);
			}
		}
	}
}