namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("")]
	public class IgniterKeyHold : Igniter 
	{
        public enum ReleaseType
        {
            OnKeyUp,
            OnTimeout
        }

		public KeyCode keyCode = KeyCode.Space;
        public float holdTime = 0.5f;
        public ReleaseType execute = ReleaseType.OnKeyUp;

        private float downTime = -9999.0f;
        private bool isPressing = false;

		#if UNITY_EDITOR
        public new static string NAME = "Input/On Key Hold";
        #endif

		private void Update()
		{
            if (this.isPressing && (Time.time - this.downTime) > this.holdTime)
            {
                switch (this.execute)
                {
                    case ReleaseType.OnKeyUp:
                        if (Input.GetKeyUp(this.keyCode))
                        {
                            this.isPressing = false;
                            this.ExecuteTrigger(gameObject);
                        }
                        break;

                    case ReleaseType.OnTimeout:
                        if (Input.GetKey(this.keyCode))
                        {
                            this.isPressing = false;
                            this.ExecuteTrigger(gameObject);
                        }
                        break;
                }
            }

            if (Input.GetKeyDown(this.keyCode))
            {
                this.downTime = Time.time;
                this.isPressing = true;
            }

            if (Input.GetKeyUp(this.keyCode))
            {
                this.isPressing = false;
            }
		}
	}
}