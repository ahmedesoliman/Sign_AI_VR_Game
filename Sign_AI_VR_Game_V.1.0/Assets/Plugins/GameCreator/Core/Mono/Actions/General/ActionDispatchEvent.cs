namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	[AddComponentMenu("")]
	public class ActionDispatchEvent : IAction
	{
        [EventName] public string eventName = "my-event";

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            EventDispatchManager.Instance.Dispatch(this.eventName, target);
            return true;
        }

		#if UNITY_EDITOR
        public static new string NAME = "General/Dispatch Event";
        private const string NODE_TITLE = "Dispatch {0}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, this.eventName);
        }
        #endif
    }
}
