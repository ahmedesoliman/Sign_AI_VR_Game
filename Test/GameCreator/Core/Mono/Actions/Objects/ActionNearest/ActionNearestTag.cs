namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	[AddComponentMenu("")]
	public class ActionNearestTag : IActionNearest
	{
        [Space][TagSelector] public string tagName = "";

        protected override bool FilterCondition(GameObject item)
        {
            return item.CompareTag(this.tagName);
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Object/Nearest with Tag";
        private const string NODE_TITLE = "Get nearest object with tag {0}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, this.tagName);
        }

        #endif
    }
}
