namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;

	[AddComponentMenu("")]
    public class IgniterMouseHoldLeft : Igniter, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
	{
		#if UNITY_EDITOR
        public new static string NAME = "Input/On Mouse Left Hold";
		public new static bool REQUIRES_COLLIDER = true;
        #endif

        private const PointerEventData.InputButton MOUSE_BUTTON = PointerEventData.InputButton.Left;

        public enum ReleaseType
        {
            OnMouseUp,
            OnTimeout
        }

        public float holdTime = 0.5f;
        public ReleaseType execute = ReleaseType.OnMouseUp;

        private float downTime = -999.0f;
        private bool isPressing = false;

        private void Start()
        {
            EventSystemManager.Instance.Wakeup();
        }

        private void Update()
        {
            if (this.execute == ReleaseType.OnTimeout && this.isPressing)
            {
                if (Input.GetMouseButton((int)MOUSE_BUTTON) && 
                    (Time.time - this.downTime) > this.holdTime)
                {
                    this.isPressing = false;
                    this.ExecuteTrigger(gameObject);
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.isPressing = true;
                this.downTime = Time.time;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.isPressing = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != MOUSE_BUTTON) return;
            if (!this.isPressing) return;

            this.isPressing = false;

            if (this.execute == ReleaseType.OnMouseUp && (Time.time - this.downTime) > this.holdTime)
            {
                this.ExecuteTrigger(gameObject);
            }
        }
    }
}