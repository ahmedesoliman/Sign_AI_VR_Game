namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;

	[AddComponentMenu("")]
	public class IgniterPlayerExit : Igniter 
	{
		#if UNITY_EDITOR
        public new static string NAME = "Object/On Player Exit";
		public new static bool REQUIRES_COLLIDER = true;
		#endif

		private void OnTriggerExit(Collider c)
		{
			int cInstanceID = c.gameObject.GetInstanceID();
			if (HookPlayer.Instance != null && HookPlayer.Instance.gameObject.GetInstanceID() == cInstanceID)
			{
                this.ExecuteTrigger(HookPlayer.Instance.gameObject);
			}
		}
	}
}