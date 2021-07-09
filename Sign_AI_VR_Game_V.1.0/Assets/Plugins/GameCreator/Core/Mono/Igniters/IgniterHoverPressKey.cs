namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class IgniterHoverPressKey : Igniter 
	{
		#if UNITY_EDITOR
        public new static string NAME = "Input/On Hover Press Key";
		public new static bool REQUIRES_COLLIDER = true;
		#endif

		public KeyCode keyCode = KeyCode.E;
		private bool isMouseOver = false;

		private void Update()
		{
			if (this.isMouseOver && Input.GetKeyDown(this.keyCode))
			{
                this.ExecuteTrigger(gameObject);
			}
		}

		private void OnMouseExit()
		{
			this.isMouseOver = false;
		}

		private void OnMouseEnter()
		{
			this.isMouseOver = true;
		}
	}
}