namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Variables;

    [AddComponentMenu("")]
    public class IgniterEventReceive : Igniter 
	{
		#if UNITY_EDITOR
        public new static string NAME = "General/On Event Receive";
        #endif

        [EventName] public string eventName = "my-event";

        [Space] 
        public VariableProperty storeInvoker = new VariableProperty();

        private void Start()
        {
            EventDispatchManager.Instance.Subscribe(this.eventName, this.OnReceiveEvent);
        }

        private void OnDestroy()
        {
            if (EventDispatchManager.IS_EXITING) return;
            EventDispatchManager.Instance.Unsubscribe(this.eventName, this.OnReceiveEvent);
        }

        private void OnReceiveEvent(GameObject invoker)
        {
            this.storeInvoker.Set(invoker, invoker);
            this.ExecuteTrigger(invoker);
        }
    }
}