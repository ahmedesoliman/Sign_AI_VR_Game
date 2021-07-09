namespace GameCreator.Variables
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionVariableSubtract : ActionVariableOperationBase
	{
        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            float current = (float)(this.variable.Get(target) ?? 0f);
            this.variable.Set(current - this.value, target);

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Variables/Variable Subtract";
        private const string NODE_TITLE = "{0} - {1}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, this.variable, this.value);
        }

		#endif
	}
}
