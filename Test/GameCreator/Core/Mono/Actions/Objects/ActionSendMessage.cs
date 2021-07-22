namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	[AddComponentMenu("")]
	public class ActionSendMessage : IAction
	{
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.GameObject);
        [Space] public string method = "MyMethod";

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject go = this.target.GetGameObject(target);
            if (go != null) go.SendMessage(this.method);

            return true;
        }

		#if UNITY_EDITOR

        public static new string NAME = "Object/Send Message";
        private const string NODE_TITLE = "Send {0} message: {1}";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                this.target.ToString(),
                this.method
            );
        }

        #endif
    }
}
