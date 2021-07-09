namespace GameCreator.Core
{
    using System;
    using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	[AddComponentMenu("")]
	public class ActionAddComponent : IAction
	{
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.GameObject);

        [Space]
        public string componentName = "UnityEngine.Light, UnityEngine";
        public bool unique = false;

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject targetGo = this.target.GetGameObject(target);
            if (targetGo != null)
            {
                Type componentType = Type.GetType(this.componentName);
                if (componentType == null) return true;

                if (!this.unique || targetGo.GetComponent(componentType) == null)
                {
                    targetGo.AddComponent(componentType);
                }
            }

            return true;
        }

		#if UNITY_EDITOR
        public static new string NAME = "Object/Add Component";
        private const string NODE_TITLE = "Add Component {0} {1}";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                this.componentName,
                (this.unique ? "(unique)" : "")
            );
        }

		#endif
	}
}
