namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;

	[AddComponentMenu("")]
	public class IgniterPlayerEnterPressKey : Igniter 
	{
		#if UNITY_EDITOR
        public new static string NAME = "Object/On Player Enter Key";
		public new static bool REQUIRES_COLLIDER = true;
		#endif

		public KeyCode keyCode = KeyCode.E;
		private bool playerInside = false;

		private void Update()
		{
			if (this.playerInside && Input.GetKeyDown(this.keyCode))
			{
                if (HookPlayer.Instance == null) return;
                this.ExecuteTrigger(HookPlayer.Instance.gameObject);
			}
		}

		private void OnTriggerEnter(Collider c)
		{
			int cInstanceID = c.gameObject.GetInstanceID();
			if (HookPlayer.Instance != null && HookPlayer.Instance.gameObject.GetInstanceID() == cInstanceID)
			{
				this.playerInside = true;
			}
		}

		private void OnTriggerExit(Collider c)
		{
			int cInstanceID = c.gameObject.GetInstanceID();
			if (HookPlayer.Instance != null && HookPlayer.Instance.gameObject.GetInstanceID() == cInstanceID)
			{
				this.playerInside = false;
			}
		}
	}
}