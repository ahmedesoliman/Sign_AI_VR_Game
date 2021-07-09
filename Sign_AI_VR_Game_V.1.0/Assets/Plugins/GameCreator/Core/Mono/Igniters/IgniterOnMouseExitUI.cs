namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Variables;
    using UnityEngine.EventSystems;

    [AddComponentMenu("")]
    public class IgniterOnMouseExitUI : Igniter, IPointerExitHandler
	{
		#if UNITY_EDITOR
		public new static string NAME = "UI/On Mouse Exit UI";
        #endif

        private void Start()
        {
            EventSystemManager.Instance.Wakeup();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.ExecuteTrigger(this.gameObject);
        }
    }
}