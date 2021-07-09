namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class IgniterMouseDown : Igniter 
	{
        public enum MouseButton
        {
            Left = KeyCode.Mouse0,
            Right = KeyCode.Mouse1,
            Middle = KeyCode.Mouse2
        }

        public MouseButton mouseButton = MouseButton.Left;

		#if UNITY_EDITOR
        public new static string NAME = "Input/On Mouse Down";
		#endif

		private void Update()
		{
			if (Input.GetKeyDown((KeyCode)this.mouseButton))
			{
                this.ExecuteTrigger(gameObject);
			}
		}
	}
}