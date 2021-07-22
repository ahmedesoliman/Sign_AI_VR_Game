namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Variables;

    [AddComponentMenu("")]
	public class ActionNearestVariable : IActionNearest
	{
        [Space] public string variableName = "my-variable";

        protected override bool FilterCondition(GameObject item)
        {
            LocalVariables localVariables = item.GetComponent<LocalVariables>();
            if (localVariables == null) return false;

            this.variableName.Trim().Replace(" ", "-");
            return localVariables.Get(this.variableName) != null;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Object/Nearest with Local Variable";
        private const string NODE_TITLE = "Get nearest object with local[{0}]";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, this.variableName);
        }

        #endif
    }
}
