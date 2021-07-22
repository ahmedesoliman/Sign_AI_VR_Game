namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Variables;
    using UnityEngine.EventSystems;

    [AddComponentMenu("")]
    public class IgniterOnMouseEnterUI : Igniter, IPointerEnterHandler
	{
		#if UNITY_EDITOR
		public new static string NAME = "UI/On Mouse Enter UI";
        #endif

        private void Start()
        {
            EventSystemManager.Instance.Wakeup();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.ExecuteTrigger(this.gameObject);
        }
    }
}