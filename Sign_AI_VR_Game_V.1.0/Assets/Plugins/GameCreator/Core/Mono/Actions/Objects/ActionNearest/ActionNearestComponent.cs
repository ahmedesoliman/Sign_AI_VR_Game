namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	[AddComponentMenu("")]
	public class ActionNearestComponent : IActionNearest
	{
        [Space] public string componentName = "Light";

        protected override bool FilterCondition(GameObject item)
        {
            return item.GetComponent(this.componentName) != null;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Object/Nearest with Component";
        private const string NODE_TITLE = "Get nearest object with component {0}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, this.componentName);
        }

        #endif
    }
}
