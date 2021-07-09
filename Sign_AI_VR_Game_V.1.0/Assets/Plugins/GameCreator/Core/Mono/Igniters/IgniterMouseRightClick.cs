namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;

	[AddComponentMenu("")]
	public class IgniterMouseRightClick : Igniter, IPointerClickHandler
	{
		public enum ClickType
		{
			SingleClick,
			DoubleClick
		}

        #if UNITY_EDITOR
		public new static string NAME = "Input/On Mouse Right Click";
		public new static bool REQUIRES_COLLIDER = true;
        #endif

		public ClickType clickType = ClickType.SingleClick;
		private float lastClickTime = -100f;

		private void Start()
        {
            EventSystemManager.Instance.Wakeup();
        }

        public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				switch (this.clickType)
				{
					case ClickType.SingleClick: this.ExecuteTrigger(gameObject); break;
					case ClickType.DoubleClick:
						if (Time.time - this.lastClickTime < 0.5f) this.ExecuteTrigger(gameObject);
						else this.lastClickTime = Time.time;
						break;
				}
			}
		}
	}
}