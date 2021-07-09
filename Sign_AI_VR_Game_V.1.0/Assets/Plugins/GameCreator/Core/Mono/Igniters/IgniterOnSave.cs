namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;

	[AddComponentMenu("")]
	public class IgniterOnSave : Igniter
	{
		#if UNITY_EDITOR
        public new static string NAME = "General/On Save";
        #endif

        private void Start()
        {
            SaveLoadManager.Instance.onSave += this.OnSave;
        }

        private void OnDestroy()
        {
            if (this.isExitingApplication) return;
            SaveLoadManager.Instance.onSave -= this.OnSave;
        }

        private void OnSave(int profile)
		{
            this.ExecuteTrigger(gameObject);
		}
	}
}