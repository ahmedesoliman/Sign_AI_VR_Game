namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	[AddComponentMenu("")]
	public class ActionDebugName : IAction
	{
        public TargetGameObject _object = new TargetGameObject(TargetGameObject.Target.Invoker);

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject instance = this._object.GetGameObject(target);
            if (instance == null) return true;

            Debug.Log(instance.name);
            return true;
        }

		#if UNITY_EDITOR

        public static new string NAME = "Debug/Debug Name";
        private const string NODE_TITLE = "Debug.Log: Name of {0}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, this._object);
        }

        #endif
    }
}
